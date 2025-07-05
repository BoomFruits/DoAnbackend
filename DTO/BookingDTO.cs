namespace DoAn.DTO
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public string CheckinDate { get; set; }
        public string CheckoutDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int TotalPrice { get; set; }
        public int Status { get; set; }
        public List<BookingDetailDTO> Details { get; set; } = new();
    }
}
