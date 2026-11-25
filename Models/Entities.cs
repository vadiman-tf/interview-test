namespace OrdersApi.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";

    // "regular", "gold", "vip"
    public string Type { get; set; } = "regular";

    public List<Order> Orders { get; set; } = new();
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTime CreatedUtc { get; set; }

    // "new", "shipped", "cancelled"
    public string Status { get; set; } = "new";

    public double Total { get; set; }
    public List<OrderLine> Lines { get; set; } = new();
}

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Product { get; set; } = "";
    public double Price { get; set; }
    public int Quantity { get; set; }
}
