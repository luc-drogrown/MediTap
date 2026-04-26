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


        /// <summary>
        /// This constructor is used when all the
        /// Appoinments must be seen
        /// Gemini says to not use this because it will break
        /// still it works flawlessly so I am not gonna rewrite it
        /// </summary>
        /// <param name="p"></param>
        public PatientDTO(Patient p)
        {
            PatientSummary = new PatientSummaryDTO(p);
            Appointments = p.Appointments?.Select(a => new AppointmentDTO(a)) ?? Enumerable.Empty<AppointmentDTO>();
            Symptoms = p.Symptoms?.Select(s => new SymptomDTO(s)) ?? Enumerable.Empty<SymptomDTO>();
            Affections = p.Affections?.Select(a => new AffectionDTO(a)) ?? Enumerable.Empty<AffectionDTO>();
            Medications = p.Medications?.Select(m => new MedicationDTO(m)) ?? Enumerable.Empty<MedicationDTO>();
        }

        /// <summary>
        /// This constructor is used when only some of the Appointments must be seen.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="appointments">The appointments to be included in the DTO.</param>
        public PatientDTO(Patient p, IEnumerable<AppointmentDTO>? appointments = null)
        {
            PatientSummary = new PatientSummaryDTO(p);
            Appointments = appointments ?? Enumerable.Empty<AppointmentDTO>();
            Symptoms = p.Symptoms?.Select(s => new SymptomDTO(s)) ?? Enumerable.Empty<SymptomDTO>();
            Affections = p.Affections?.Select(a => new AffectionDTO(a)) ?? Enumerable.Empty<AffectionDTO>();
            Medications = p.Medications?.Select(m => new MedicationDTO(m)) ?? Enumerable.Empty<MedicationDTO>();
        }

        public PatientDTO()
        {
            
        }


    }
}
