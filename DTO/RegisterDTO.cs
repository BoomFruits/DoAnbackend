using System.ComponentModel.DataAnnotations;

namespace DoAn.DTO
{
    public class RegisterDTO
    {
        [Required, MinLength(4), MaxLength(20)]
        public string Username { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, RegularExpression("Male|Female|Other")]
        public string Gender { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; } = 1;
    }
}
