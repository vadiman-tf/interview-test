using Microsoft.EntityFrameworkCore;
using OrdersApi.Models;

namespace OrdersApi.Data;

public interface IOrderRepository
{
    IQueryable<Order> Query();
    Order? GetById(int id);
    Task<List<Customer>> GetCustomersAsync();
    Task AddAsync(Order order);
    Task SaveAsync();
    void ExportToCsv(string path);
}

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db) => _db = db;

    public IQueryable<Order> Query() => _db.Orders;

    public Order? GetById(int id) =>
        _db.Orders.FirstOrDefaultAsync(o => o.Id == id).Result;

    public Task<List<Customer>> GetCustomersAsync() => _db.Customers.ToListAsync();

    public async Task AddAsync(Order order) => await _db.Orders.AddAsync(order);

    public async Task SaveAsync()
    {
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Save failed: " + ex.Message);
            throw ex;
        }
    }

    public void ExportToCsv(string path) => throw new NotImplementedException();
}
