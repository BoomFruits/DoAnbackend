namespace DoAn.Data
{
    public class Notification
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public bool AdminRead { get; set; } = false; 
        public int? BookingId { get; set; }
        public User User { get; set; }
    }
}
