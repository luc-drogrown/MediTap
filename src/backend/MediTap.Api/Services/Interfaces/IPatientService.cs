
using MediTap.Api.DTO;
using MediTap.Api.Models;
namespace MediTap.Api.Services.Interfaces
{
    public interface IPatientService
    {
        Patient Create(PatientCreationDTO request);
        Patient GetLoggedInPatient(int loggedInUserId);
        IEnumerable<PatientSummaryDTO> GetProfileSummary(int id);

        IEnumerable<Appointment> GetAppointment(int id, int loggedInUserId, string role);
        IEnumerable<Symptom> GetSymptom(int id);
        IEnumerable<Medication> GetMedication(int id);
        IEnumerable<Affection> GetAffection(int id);
        Patient GetById(int id, int loggedInUserId, string role);
        Patient AddSymptom(Symptom symptom, int patientId, string role);
    }
}
