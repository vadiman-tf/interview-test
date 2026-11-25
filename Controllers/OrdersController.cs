using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Dtos;
using OrdersApi.Models;
using OrdersApi.Services;

namespace OrdersApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;
    private readonly AppDbContext _db;

    public OrdersController(OrderService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpGet("{id}")]
    public Order Get(int id)
    {
        return _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Lines)
            .First(o => o.Id == id);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<List<OrderSummaryDto>> GetForCustomer(int customerId)
    {
        return await _service.GetCustomerOrders(customerId);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        try
        {
            var order = await _service.CreateOrder(request, true, true);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var ok = await _service.CancelOrder(id);
        return ok ? Ok() : BadRequest();
    }

    [HttpPost("{id}/ship")]
    public async Task<IActionResult> Ship(int id)
    {
        var ok = await _service.ShipOrder(id);
        return ok ? Ok() : BadRequest();
    }

    [HttpGet("stats")]
    public IActionResult Stats()
    {
        return Ok(new { ordersCreated = OrderService.OrdersCreated });
    }
}
