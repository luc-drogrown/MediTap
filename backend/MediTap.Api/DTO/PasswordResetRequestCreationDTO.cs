namespace MediTap.Api.DTO
{
    public class PasswordResetRequestCreationDTO
    {
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}