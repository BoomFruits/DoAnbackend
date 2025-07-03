using System.ComponentModel.DataAnnotations;

namespace DoAn.Data
{
    public class Booking
    {
        public int Id { get; set; }
        public Guid? StaffId { get; set; }
        public User Staff { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public User Customer { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public int status { get; set; } = 0; // 0: pending ( chờ xác nhận ), 1: confirmed (đã xác nhận thanh toán), 2: canceled (huỷ) 3: done ( hoàn thành )
        [MaxLength(20)]
        public string? PaymentMethod { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; } = false; // Trạng thái thanh toán
        [Required]
        public string Note { get; set; } = string.Empty;
        public int TotalPrice { get; set; } // Sum price bookingdetails
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
        public ICollection<ServiceDetail> ServiceDetails { get; set; } = new List<ServiceDetail>();

        //  1 người ( 1 tài khoản )  có thể đặt nhiều phòng khác nhau .

    }
}
