using MediTap.Api.Controllers;
using MediTap.Api.DTO;
namespace MediTap.Api.Services.Interfaces
{
    public interface ISymptomService
    {
        /// <summary>
        /// Checks if the user has permission to access a specific Symptom
        /// </summary>
        /// <param name="id">Id of the Symptom</param>
        /// <param name="userId">Id of the user trying to access the Symptom</param>
        /// <returns>Returns <![CDATA[true]]> if user is authorized, <![CDATA[false]]> otherwise</returns>
        bool GetByIdIfAuthorized(int id, int userId);

        /// <summary>
        /// Updates the details of an existing symptom using the specified data transfer object.
        /// Only used by Patient.
        /// For Medic access on a certain Symptom see <seealso cref="PatientController.GetSymptoms(int)"/>
        /// </summary>
        /// <param name="symptom">An object containing the updated values for the symptom. Cannot be null.</param>
        /// <param name="id">Id of the Symptom being updated</param>
        void Update(SymptomUpdateDTO symptom, int id);


        /// <summary>
        /// Deletes the entity identified by the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete. Must correspond to an existing entity.</param>
        void Delete(int id);
    }
}
