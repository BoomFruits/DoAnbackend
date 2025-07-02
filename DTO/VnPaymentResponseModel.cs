namespace DoAn.Service
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public long OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string OrderDescription { get;  set; }
    }
    public class VnPaymentRequestModel
    {
        public long BookingId { get; set; }
        public int totalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
