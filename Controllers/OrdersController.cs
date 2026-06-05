using Microsoft.AspNetCore.Mvc;
using OrdersApi.Dtos;
using OrdersApi.Services;

namespace OrdersApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;
    public OrdersController(OrderService service) => _service = service;

    [HttpGet("recent")]
    public async Task<IReadOnlyList<OrderDto>> GetRecent()
    {
        return await _service.GetRecentAsync();
    }

    [HttpGet("totals")]
    public async Task<int[]> GetTotals([FromQuery] int[] customerIds)
    {
        return await _service.GetTotalsAsync(customerIds);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetReport()
    {
        var report = await Task.Run(() => _service.BuildReport());
        return Ok(report);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(dto, ct);
        _ = _service.SendConfirmationAsync(id);

        return Ok(id);
    }
}
