namespace MediTap.Api.Services.Interfaces
{
    /// <summary>
    /// These services are used to make sure that only the authorized 
    /// users can acccess and modify a resource
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Checks if a user (Medic or Patient) can access / modify an Appointment 
        /// </summary>
        /// <param name="patientId">Id of the patient that either does the Appointment or is the other half on an Appointment</param>
        /// <param name="medicId">Id of the medic that either does the Appointment or is the other half on an Appointment</param>
        /// <param name="appointmentId">Id of the Appointment to be accesed / modified.</param>
        /// <returns>True if Patient and Medic can create an Appointment</returns>
        bool IsAppointmentAuth(int patientId, int medicId, int appointmentId);

        /// <summary>
        /// Cheks whether the logged in user is connected to the specified Appointment
        /// </summary>
        /// <param name="appointmentId">Id of the appointment to be accesed / modified</param>
        /// <param name="userId">Id of the user trying to acces/modify it</param>
        /// <param name="role">Role of the current user</param>
        /// <returns></returns>
        bool IsAssociatedWithAppointment(int appointmentId, int userId, string role);


        /// <summary>
        /// Checks if a Patient and a Medic are linked.
        /// </summary>
        /// <param name="patientId">Id of the Patient</param>
        /// <param name="medicId">Id of the Medic</param>
        /// <returns>True if they are connected, false otherwise</returns>
        bool IsPatientMedicLinked(int patientId, int medicId);
    }
}
