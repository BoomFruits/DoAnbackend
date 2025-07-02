namespace DoAn.DTO
{
    public class CreateBookingDetailDTO
    {
        public int RoomId { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public string? RoomNote { get; set; }
        public double Price { get; set; }
    }
}
