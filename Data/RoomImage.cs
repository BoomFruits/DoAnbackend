using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DoAn.Data
{
    public class RoomImage
    {
        public int Id { get; set; }
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        public int RoomId { get; set; } // Foreign key to Room
        [JsonIgnore]
        public Room Room { get; set; } // Navigation property to Room

    }
}
