namespace MediTap.Front.Models.Admin
{
    public class AdminMedicViewModel
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
