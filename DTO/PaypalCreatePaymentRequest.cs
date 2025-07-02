using PayPal.Api;

namespace DoAn.DTO
{
    public class PaypalCreatePaymentRequest
    {
        public long BookingId { get; set; }
        public List<Item> Items { get; set; } = new();
        public double Tax { get; set; }
        public double Shipping { get; set; }
        public string BaseUrl { get; set; } = "";  
    }
}
