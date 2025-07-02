using System.ComponentModel.DataAnnotations;

namespace DoAn.Data
{
    public class User
    {
        public Guid id { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(20)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [RegularExpression("Male|Female|Other", ErrorMessage = "Gender not valid")]
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [Required]
        [RegularExpression("User|Admin", ErrorMessage = "Role not valid")]
        public int RoleId { get; set; }
        [Required]
        public  Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Booking> StaffBookings { get; set; } = new List<Booking>();
        public ICollection<Booking> CustomerBookings { get; set; } = new List<Booking>();
    }
}
