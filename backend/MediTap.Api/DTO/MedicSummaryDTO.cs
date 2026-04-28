using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class MedicSummaryDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        // Foreign Keys
        public IEnumerable<PatientSummaryDTO>? Patients { get; set; }

        public IEnumerable<AppointmentDTO>? Appointments { get; set; }
    
    
        public MedicSummaryDTO(Medic m)
        {
            Id = m.Id;
            FirstName = m.FirstName;
            LastName = m.LastName;
            Email = m.Email?.EmailAddress;
            PhoneNumber = m.PhoneNumber?.Number;
            
            // Map patients to PatientSummaryDTO
            Patients = m.Patients?.Select(p => new PatientSummaryDTO(p));

            // Map appointments to AppointmentDTO
            Appointments = m.Appointments?.Select(a => new AppointmentDTO(a));
        }
    }
}
