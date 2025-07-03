namespace DoAn.DTO
{
    public class CreateBookingDTO
    {
        public Guid? StaffId { get; set; } 
        public string? Note { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; } = string.Empty;
        public List<BookingDetailDTO> Details { get; set; } = new();
    }
}
