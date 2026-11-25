using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Models;
using OrdersApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("orders"));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddSingleton<NotificationService>();

var app = builder.Build();

SeedData(app);

app.MapControllers();
app.Run();

static void SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Customers.Any()) return;

    db.Customers.AddRange(
        new Customer { Id = 1, Name = "Alice", Email = "alice@example.com", Type = "regular" },
        new Customer { Id = 2, Name = "Bob",   Email = "bob@example.com",   Type = "Gold" },
        new Customer { Id = 3, Name = "Carol", Email = "carol@example.com", Type = "vip" });

    db.Orders.Add(new Order
    {
        CustomerId = 1,
        CreatedUtc = DateTime.UtcNow.AddDays(-2),
        Status = "new",
        Total = 29.97,
        Lines = new List<OrderLine> { new() { Product = "Widget", Price = 9.99, Quantity = 3 } }
    });
    db.SaveChanges();
}
