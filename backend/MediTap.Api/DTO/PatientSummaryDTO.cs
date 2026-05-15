using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class PatientSummaryDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string CNP { get; set; }


        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public string Status { get; set; } = string.Empty;

        // Constructor that takes a Patient entity and maps it to the DTO
        public PatientSummaryDTO(Patient p)
        {
            Id = p.Id;
            FirstName = p.FirstName;
            CNP = p.CNP.CodNumericPersonal;
            LastName = p.LastName;
            Email = p.Email?.EmailAddress;
            PhoneNumber = p.PhoneNumber?.Number;
            Status = p.PatientStatus.ToString();
        }

        public PatientSummaryDTO()
        {
            
        }
    }
}
