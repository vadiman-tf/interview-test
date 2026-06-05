using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Dtos;

namespace OrdersApi.Services;

public class OrderService
{
    private readonly AppDbContext _db;
    private static readonly ConcurrentDictionary<int, OrderDto> _cache = new();

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<OrderDto>> GetRecentAsync()
    {
        var all = await _db.Orders.ToListAsync();
        return all.Where(o => o.CreatedUtc > DateTime.Now.AddDays(-7))
                  .OrderByDescending(o => o.CreatedUtc)
                  .Select(o => new OrderDto(o))
                  .ToList();
    }

    public async Task<int[]> GetTotalsAsync(int[] customerIds)
    {
        var tasks = customerIds.Select(id =>
            _db.Orders.Where(o => o.CustomerId == id).CountAsync());
        return await Task.WhenAll(tasks);
    }

    public async Task<int> CreateAsync(OrderDto dto, CancellationToken ct)
    {
        foreach (var line in dto.Lines)
        {
            _db.OrderLines.Add(line.ToEntity());
            await _db.SaveChangesAsync(ct);
        }
        return dto.Id;
    }

    public async Task SendConfirmationAsync(int orderId)
    {
        using var client = new HttpClient();
        try
        {
            await client.GetAsync($"http://localhost:9/confirm/{orderId}");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public ReportDto BuildReport()
    {
        var orders = _db.Orders.ToList();
        var lines = _db.OrderLines.ToList();
        return new ReportDto
        {
            TotalOrders = orders.Count,
            TotalRevenue = lines.Sum(l => l.Price * l.Quantity)
        };
    }
}
