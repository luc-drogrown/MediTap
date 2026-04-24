using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class PatientSummaryDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public CNP CNP { get; set; }


        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }


    
    }
}
