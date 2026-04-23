using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IAppointmentService
    {
        Appointment GetById(int id, int userId, string role);
        Appointment Add(Appointment appointment, int userId, string role);
        void Delete(int id);
    }
}
