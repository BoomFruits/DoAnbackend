namespace DoAn.DTO
{
    public class UpdateUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}
