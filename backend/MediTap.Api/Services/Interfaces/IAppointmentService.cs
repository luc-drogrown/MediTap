using MediTap.Api.Models;
using MediTap.Api.DTO;
namespace MediTap.Api.Services.Interfaces
{
    public interface IAppointmentService
    {
        /// <summary>
        /// Gets the Appointment specified by the Id argument
        /// </summary>
        /// <param name="id">Id of the Appointment</param>
        /// <returns>An AppointmentDTO or null if Appointment wasn't found</returns>
        AppointmentDTO GetById(int id);


        /// <summary>
        /// Creates a new appointment using the specified appointment details, associating it with the given user and
        /// role.
        /// </summary>
        /// <param name="appointment">An object containing the details required to create the appointment. Cannot be null.</param>
        /// <param name="userId">The identifier of the user for whom the appointment is being created.</param>
        /// <param name="role">The role of the user in the context of the appointment. Cannot be null or empty.</param>
        /// <returns>An AppointmentDTO representing the newly created appointment.</returns>
        AppointmentDTO Add(AppointmentCreationDTO appointment, int userId, string role);
        


        /// <summary>
        /// Delets a Appointment by the ID
        /// </summary>
        /// <param name="id">Id of the appointment to be deleted</param>
        void Delete(int id);
    }
}
