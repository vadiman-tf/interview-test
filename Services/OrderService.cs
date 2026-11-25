using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Dtos;
using OrdersApi.Models;

namespace OrdersApi.Services;

public class OrderService
{
    private readonly IOrderRepository _repo;
    private readonly IServiceProvider _provider;

    public static int OrdersCreated;

    public OrderService(IOrderRepository repo, IServiceProvider provider)
    {
        _repo = repo;
        _provider = provider;
    }

    public async Task<Order> CreateOrder(CreateOrderRequest request, bool sendEmail, bool applyDiscount)
    {
        var customers = await _repo.GetCustomersAsync();
        var customer = customers.FirstOrDefault(c => c.Id == request.CustomerId);
        if (customer == null)
            throw new Exception("Customer not found");

        var order = new Order
        {
            CustomerId = customer.Id,
            CreatedUtc = DateTime.Now,
            Status = "new"
        };

        double total = 0;
        foreach (var line in request.Lines)
        {
            if (line.Quantity > 0)
            {
                if (line.Price > 0)
                {
                    if (!string.IsNullOrEmpty(line.Product))
                    {
                        order.Lines.Add(new OrderLine
                        {
                            Product = line.Product,
                            Price = line.Price,
                            Quantity = line.Quantity
                        });
                        total += line.Price * line.Quantity;
                    }
                }
            }
        }

        if (applyDiscount)
            total = total - total * DiscountCalculator.Instance.Calculate(customer, total);

        order.Total = total;

        await _repo.AddAsync(order);
        await _repo.SaveAsync();
        OrdersCreated++;

        if (sendEmail)
        {
            var notifier = (NotificationService)_provider.GetService(typeof(NotificationService))!;
            notifier.SendOrderConfirmation(order);
        }

        Console.WriteLine("Order created: " + order.Id + ", discount: " + DiscountCalculator.Instance.LastDiscount);
        return order;
    }

    public async Task<List<OrderSummaryDto>> GetCustomerOrders(int customerId)
    {
        var orders = await _repo.Query().ToListAsync();
        var result = new List<OrderSummaryDto>();

        foreach (var o in orders.Where(x => x.CustomerId == customerId))
        {
            var lineCount = _repo.Query()
                .Where(x => x.Id == o.Id)
                .SelectMany(x => x.Lines)
                .Count();

            result.Add(new OrderSummaryDto
            {
                Id = o.Id,
                Status = o.Status,
                Total = o.Total,
                LineCount = lineCount
            });
        }
        return result;
    }

    public async Task<bool> CancelOrder(int id)
    {
        try
        {
            var order = _repo.GetById(id);
            if (order!.Status == "shipped")
                throw new InvalidOperationException("Shipped order cannot be cancelled");

            order.Status = "cancelled";
            await _repo.SaveAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> ShipOrder(int id)
    {
        try
        {
            var order = _repo.GetById(id);
            if (order!.Status == "cancelled")
                throw new InvalidOperationException("Cancelled order cannot be shipped");

            order.Status = "shipped";
            await _repo.SaveAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
