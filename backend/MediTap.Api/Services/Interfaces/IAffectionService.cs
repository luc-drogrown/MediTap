using MediTap.Api.DTO;
namespace MediTap.Api.Services.Interfaces
{
    public interface IAffectionService
    {
        /// <summary>
        /// Gets the affection specified by the ID parameter.
        /// </summary>
        /// <param name="id">Id of the Affection</param>
        /// <param name="userId">Id of the user checking the Affection</param>
        /// <returns></returns>
        AffectionDTO GetById(int id, int userId);



        /// <summary>
        /// Adds an affection from a DTO
        /// </summary>
        /// <param name="affection">DTO of the affection to be created</param>
        /// <param name="userId">Id of the user creating the Affection</param>
        /// <returns>The newly created Affection DTO</returns>
        AffectionDTO Add(AffectionCreationDTO affection, int userId);

        /// <summary>
        /// Every medic associated with the patient can modify a Affection
        /// </summary>
        /// <param name="id">Id of the Affection</param>
        /// <param name="userId">Id of the user trying to get authentification</param>
        /// <returns>True if user gets authentification, false otherwise</returns>
        bool GetAuthById(int id, int userId);

        /// <summary>
        /// Deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        void Delete(int id);
    }
}