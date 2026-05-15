namespace MediTap.Api.DTO
{
    public class PasswordResetRequestDTO
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; }
    }
}