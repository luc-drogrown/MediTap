
using MediTap.Api.DTO;
using MediTap.Api.Models;
namespace MediTap.Api.Services.Interfaces
{
    //@TODO
    //Write documentation for the methods
    public interface IPatientService
    {
        Patient Create(PatientCreationDTO request);
        IEnumerable<PatientSummaryDTO> GetAllForAdmin();

        void DisableAccount(int patientId);
        void EnableAccount(int patientId);
        void UpdateAccountForAdmin(int patientId, PatientAdminUpdateDTO request);

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
        /// <remarks>THIS METHOD NEEDS TO BE REDONE</remarks>
        /// <param name="loggedInUserId">
        /// Id of the Medic taht calls this method</param>
        /// <param name="role">
        /// <param name="id">Id of the Patient that is to be searched</param>
        /// ROle of the user that is logged in. It MUST be a "Medic".</param>
        /// <returns>PatientDTO | null</returns>
        PatientDTO GetById(int id, int loggedInUserId, string role);


        /// <summary>
        /// Adds a symptom to the current Patient's list of symptoms. 
        /// </summary>
        /// <param name="symptom">The Symptom that it need to be added</param>
        /// <param name="loggedInUserId">The current user logged in</param>
        /// <param name="role">It must be a Patient</param>
        /// <returns></returns>
        SymptomDTO AddSymptom(SymptomCreationDTO symptom, int loggedInUserId, string role);
    }
}
