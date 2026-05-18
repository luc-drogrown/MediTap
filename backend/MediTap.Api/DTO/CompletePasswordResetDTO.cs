namespace MediTap.Api.DTO
{
    public class CompletePasswordResetDTO
    {
        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}