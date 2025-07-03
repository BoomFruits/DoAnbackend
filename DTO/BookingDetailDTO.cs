namespace DoAn.DTO
{
    public class BookingDetailDTO
    {
        public int RoomId { get; set; }
        public string room_No { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }     
        public double Price { get; set; }
        public string? RoomNote { get; set; }
        public List<CreateServiceDetailDTO> Services { get; set; } = new();

    }
}
