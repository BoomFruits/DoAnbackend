using Microsoft.AspNetCore.Mvc;
using PayPal.Api;

namespace DoAn.Service
{
    public interface IPaypalService
    {
        Payment CreatePayment(string baseUrl, List<Item> items, double tax, double shipping,long bookingId);
        Payment ExecutePayment(string paymentId, string payerId);
    }
}
