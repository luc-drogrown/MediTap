namespace MediTap.Api.Models
{
    public class PasswordResetRequest
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        // Patient or Medic

        public string Status { get; set; } = "Pending";
        // Pending, Completed, Rejected

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public int? CompletedByAdminId { get; set; }

        public string? Notes { get; set; }
    }
}