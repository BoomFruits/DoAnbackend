namespace DoAn.DTO
{
    public class CreateBookingDTO
    {
        public Guid? StaffId { get; set; } // Cho phép null nếu là khách tự đặt
        public string? Note { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; } = string.Empty;
        public List<BookingDetailDTO> Details { get; set; } = new();
    }
}
