using Microsoft.EntityFrameworkCore;
using OrdersApi.Data;
using OrdersApi.Models;
using OrdersApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("orders"));
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

SeedData(app);

app.MapControllers();
app.Run();

static void SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Customers.Any()) return;

    var rnd = new Random(1);
    for (var c = 1; c <= 5; c++)
    {
        db.Customers.Add(new Customer { Id = c, Name = $"Customer {c}" });
        for (var o = 0; o < 10; o++)
        {
            db.Orders.Add(new Order
            {
                CustomerId = c,
                CreatedUtc = DateTime.UtcNow.AddDays(-rnd.Next(0, 14)),
                Lines = new List<OrderLine>
                {
                    new() { Product = "Widget", Price = 9.99m, Quantity = rnd.Next(1, 5) }
                }
            });
        }
    }
    db.SaveChanges();
}
