namespace OrdersApi.Dtos;

public class CreateOrderRequest
{
    public int CustomerId { get; set; }
    public List<OrderLineRequest> Lines { get; set; } = new();
}

public class OrderLineRequest
{
    public string Product { get; set; } = "";
    public double Price { get; set; }
    public int Quantity { get; set; }
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "";
    public double Total { get; set; }
    public int LineCount { get; set; }
}
