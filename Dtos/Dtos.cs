using OrdersApi.Models;

namespace OrdersApi.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreatedUtc { get; set; }
    public List<OrderLineDto> Lines { get; set; } = new();

    public OrderDto() { }

    public OrderDto(Order order)
    {
        Id = order.Id;
        CustomerId = order.CustomerId;
        CreatedUtc = order.CreatedUtc;
    }
}

public class OrderLineDto
{
    public string Product { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public OrderLine ToEntity() => new OrderLine
    {
        Product = Product,
        Price = Price,
        Quantity = Quantity
    };
}

public class ReportDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
