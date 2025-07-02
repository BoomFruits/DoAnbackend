using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Xml;

namespace DoAn.Data
{
    public class BookingDetail
    { //primary key là BookingId + RoomId
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; } = null!;

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public string? RoomNote { get; set; } = string.Empty; // Ghi chú của khách hàng về phòng
        public bool IsCheckedIn { get; set; } = false; // Trạng thái check-in của phòng
        public bool IsCheckedOut { get; set; } = false; // Trạng thái check-out của phòng
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public double Price { get; set; }
        [NotMapped]
        public int Unit => (CheckoutDate - CheckinDate).Days; // Tự động tính dựa trên ngày checkin và checkout
        public ICollection<ServiceDetail> ServiceDetails { get; set; } = new List<ServiceDetail>(); // 1 chi tiết đặt phòng có thể sử dụng các dịch vụ

    }
}
