using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DoAn.Data
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Room_No { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Room_Name { get; set; } = string.Empty;
        [Range(1,20)]
        public int Capacity { get; set; }
        [Required]
        [MaxLength(10)]
        public string Type { get; set; } = string.Empty; // Single, Double, Triple, Quad
        [Range(0, double.MaxValue)]
        public double Price { get; set; } // Price per night
        public DateTime CreatedAt { get; set; } 
        public bool IsAvailable { get; set; } = true;
        public int Bed { get; set; } // Number of beds
        public int Bath { get; set; } // Number of bathrooms
        public int Area { get; set; } // Area in square meters
        public string description { get; set; } = string.Empty; // Description of the room
        public List<RoomImage> Images { get; set; } = new List<RoomImage>(); // List of images associated with the room
        // Navigation properties
        public ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
        
        
    }
}
