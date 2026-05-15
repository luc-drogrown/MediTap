namespace MediTap.Front.Models.Admin
{
    public class AdminPatientViewModel
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public long CNP { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
