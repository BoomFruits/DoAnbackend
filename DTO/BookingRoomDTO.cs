namespace DoAn.DTO
{
    public class BookingRoomDTO
    {
        public string RoomNo { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public double Price { get; set; }
    }
}
