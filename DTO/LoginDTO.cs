namespace DoAn.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Provider { get; set; } = string.Empty;  // Google , FB
        public string? IdTokenOrAccessToken { get; set; } = string.Empty;

    }
}

