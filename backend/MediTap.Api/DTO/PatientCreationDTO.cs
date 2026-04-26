using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    /// <summary>
    /// This DTO is used when creating a new Patient.
    /// It omits fields like passwordHash, Id, etc.
    /// </summary>
    public class PatientCreationDTO
    {
        public DateOnly DateOfBirth { get; set;}

        public string CNP { get; set; }

        public string FirstName { get; set; }

        public string Password { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        

    }
}
