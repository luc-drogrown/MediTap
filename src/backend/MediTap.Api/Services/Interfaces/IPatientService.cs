
using MediTap.Api.DTO;
using MediTap.Api.Models;
namespace MediTap.Api.Services.Interfaces
{
    public interface IPatientService
    {
        Patient Create(PatientCreationDTO request);
        PatientDTO GetLoggedInPatient(int loggedInUserId);
        PatientSummaryDTO GetProfileSummary(int id);
        IEnumerable<AppointmentDTO> GetAppointment(int id, int loggedInUserId, string role);
        IEnumerable<SymptomDTO> GetSymptom(int id);
        IEnumerable<MedicationDTO> GetMedication(int id);
        IEnumerable<AffectionDTO> GetAffection(int id);

        /// <summary>
        /// This method is only used by a logged in Medic
        /// to check if the patient is linked with them.
        /// If it is NOT it returns null
        /// </summary>
        /// <param name="loggedInUserId">
        /// Id of the Medic taht calls this method</param>
        /// <param name="role">
        /// <param name="id">Id of the Patient that is to be searched</param>
        /// ROle of the user that is logged in. It MUST be a "Medic".</param>
        /// <returns>PatientDTO | null</returns>
        PatientDTO GetById(int id, int loggedInUserId, string role);



        SymptomDTO AddSymptom(SymptomCreationDTO symptom, int loggedInUserId, string role);
    }
}
