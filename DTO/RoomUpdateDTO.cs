namespace DoAn.DTO
{
    public class RoomUpdateDTO
    {
        public string Room_No { get; set; } = string.Empty;
        public string Room_Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Type { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
        public int Bed { get; set; }
        public int Bath { get; set; }
        public int Area { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? KeptImageUrls { get; set; } = string.Empty;
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
    }
}
