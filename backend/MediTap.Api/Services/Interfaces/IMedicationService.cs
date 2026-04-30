using MediTap.Api.Models;
using MediTap.Api.DTO;
namespace MediTap.Api.Services.Interfaces
{
    public interface IMedicationService
    {
        MedicationDTO GetById(int id, int userId);
        MedicationDTO Add(MedicationCreationDTO medication, int userId);
        void Update(MedicationUpdateDTO medication, int id);

        /// <summary>
        /// Validates permission for a user to modify / access Medication
        /// </summary>
        /// <param name="id"> Id of the Medication</param>
        /// <param name="userId">Id of the user trying to access / modify it</param>
        /// <returns>True if user has permission, false otherwise</returns>
        bool GetAuth(int id, int userId);
        void Delete(int id);
    }
}