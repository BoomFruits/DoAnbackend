namespace DoAn.Data
{
    //Một người tổ chức -> thuê phòng -> thuê nhiều phòng -> mỗi phòng ( người ) họ sẽ sử dụng dịch vụ khác nhau
    public class ServiceDetail
    {
        public int Id { get; set; }
        //Composite key BookingId + RoomId
        public int BookingId { get; set; }
        public int RoomId { get; set; } 
        public Guid CustomerId { get; set; }
        public User Customer { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public double Price { get; set; }
        public int Amount { get; set; }

    }
}
