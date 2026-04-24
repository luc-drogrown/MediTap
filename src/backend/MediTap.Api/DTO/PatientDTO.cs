using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class PatientDTO
    {
        public  PatientSummaryDTO PatientSummary { get; set; }

        // Foreign Keys
        public IEnumerable<AppointmentDTO>? Appointments { get; set; }
        public IEnumerable<SymptomDTO>? Symptoms { get; set; }
        public IEnumerable<AffectionDTO>? Affections { get; set; }
        public IEnumerable<MedicationDTO>? Medications { get; set; }

        public PatientDTO(Patient p)
        {
            PatientSummary = new PatientSummaryDTO(p);
            Appointments = p.Appointments?.Select(a => new AppointmentDTO(a)) ?? Enumerable.Empty<AppointmentDTO>();
            Symptoms = p.Symptoms?.Select(s => new SymptomDTO(s)) ?? Enumerable.Empty<SymptomDTO>();
            Affections = p.Affections?.Select(a => new AffectionDTO(a)) ?? Enumerable.Empty<AffectionDTO>();
            Medications = p.Medications?.Select(m => new MedicationDTO(m)) ?? Enumerable.Empty<MedicationDTO>();
        }
    }
}
