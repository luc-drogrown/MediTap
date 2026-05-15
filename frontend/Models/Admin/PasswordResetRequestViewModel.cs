namespace MediTap.Front.Models.Admin
{
    public class PasswordResetRequestViewModel
    {
        public int Id { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public string? Status { get; set; }

        public DateTime RequestedAt { get; set; }
    }
}
