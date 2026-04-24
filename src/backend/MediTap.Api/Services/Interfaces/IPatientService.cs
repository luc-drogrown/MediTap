
using MediTap.Api.DTO;
using MediTap.Api.Models;
namespace MediTap.Api.Services.Interfaces
{
    public interface IPatientService
    {
        Patient Create(PatientCreationDTO request);
        PatientDTO GetLoggedInPatient(int loggedInUserId);
        IEnumerable<PatientSummaryDTO> GetProfileSummary(int id);
        IEnumerable<AppointmentDTO> GetAppointment(int id, int loggedInUserId, string role);
        IEnumerable<SymptomDTO> GetSymptom(int id);
        IEnumerable<MedicationDTO> GetMedication(int id);
        IEnumerable<AffectionDTO> GetAffection(int id);
        PatientDTO GetById(int id, int loggedInUserId, string role);
        PatientDTO AddSymptom(SymptomDTO symptom, int patientId, string role);
    }
}
