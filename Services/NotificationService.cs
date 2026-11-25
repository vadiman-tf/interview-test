using System.Text;
using OrdersApi.Models;

namespace OrdersApi.Services;

public class NotificationService
{
    public async void SendOrderConfirmation(Order order)
    {
        try
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:9");

            var body = $"{{\"to\":\"{order.Customer!.Email}\",\"text\":\"Order {order.Id} confirmed. Total: {order.Total}\"}}";
            await client.PostAsync("/send", new StringContent(body, Encoding.UTF8, "application/json"));
        }
        catch (Exception)
        {
        }
    }
}
