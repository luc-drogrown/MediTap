using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class MedicCreationDTO
    {
        public string FirstName { get; set; }

        public string Specialty { get; set; }

        //public string Uname { get; private set; }

        public MedicStatus MedicStatus { get; set; }
        public string Password { get; set; }

        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
