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

        public CNP CNP { get; set; }

        public string FirstName { get; set; }

        public string Uname { get; set; }

        public string Password { get; set; }

        public string? LastName { get; set; }

        public Email? Email { get; set; }

        public PhoneNumber? PhoneNumber { get; set; }

        public string? Address { get; set; }

        

    }
}
