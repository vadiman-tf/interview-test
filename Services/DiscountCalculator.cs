using OrdersApi.Models;

namespace OrdersApi.Services;

public class DiscountCalculator
{
    private static DiscountCalculator? _instance;

    public static DiscountCalculator Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DiscountCalculator();
            return _instance;
        }
    }

    public double LastDiscount { get; private set; }

    private DiscountCalculator() { }

    public double Calculate(Customer customer, double total)
    {
        switch (customer.Type)
        {
            case "regular":
                LastDiscount = 0;
                break;
            case "gold":
                LastDiscount = total > 100 ? 0.05 : 0;
                break;
            case "vip":
                LastDiscount = total > 100 ? 0.15 : 0.1;
                break;
            default:
                LastDiscount = 0;
                break;
        }
        return LastDiscount;
    }
}
