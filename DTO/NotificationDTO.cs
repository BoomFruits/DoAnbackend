namespace DoAn.DTO
{
    public class NotificationDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int bookingId { get; set; }
        public Guid userId { get; set; }
    }
}
