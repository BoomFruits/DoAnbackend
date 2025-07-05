namespace DoAn.DTO
{
    public class BookingDetailDTO
    {
        public int RoomId { get; set; }
        public string Room_No { get; set; } = string.Empty;
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public bool IsCheckedIn { get; set; }
        public bool IsCheckedOut { get; set; }
        public double Price { get; set; } // đơn giá/ngày
        public string? RoomNote { get; set; }
        public List<ServiceDTO> Services { get; set; } = new();
        public double TotalAmount { get; set; } // tổng tiền phòng + dịch vụ

    }
}
