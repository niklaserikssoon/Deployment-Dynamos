namespace User_API.DTOs
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}