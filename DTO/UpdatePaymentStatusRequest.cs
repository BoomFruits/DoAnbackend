namespace DoAn.DTO
{
    public class UpdatePaymentStatusRequest
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; } = "";
    }
}
