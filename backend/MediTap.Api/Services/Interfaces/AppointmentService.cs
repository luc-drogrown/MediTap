using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediTap.Api.Services.Interfaces
{
    public class AppointmentService : IAppointmentService
    {

        private readonly MediTapDbContext _context;

        public AppointmentService(MediTapDbContext context)
        {
            _context = context;
        }


        AppointmentDTO IAppointmentService.Add(AppointmentCreationDTO appointment, int userId, string role)
        {
            if(appointment == null) { return null; }

            // make sure the userID is equal to apppointment.MedicId or apppointment.PatientID depending on the role of the user
            bool isAuth;
            switch (role)
            {
                case "Medic":
                    isAuth = userId == appointment.MedicId; break;

                case "Patient":
                    isAuth = userId == appointment.PatientId; break;

                default:
                    isAuth = false; break;
            }

            // Permission denied
            if(!isAuth) { return null; }

            // Permission granted
            var appointmentEntity = new Appointment()
            {
                Date = appointment.Date,
                IssueDate = appointment.IssueDate,
                Title = appointment.Title ?? null,
                Description = appointment.Description ?? null,

                // Foreign keys
                MedicId = appointment.MedicId,
                PatientId = appointment.PatientId,
            };

            _context.Appointments.Add(appointmentEntity);
            _context.SaveChanges();
            return new AppointmentDTO(appointmentEntity);
        }

        AppointmentDTO IAppointmentService.GetById(int id)
        {
            var result = _context.Appointments.FirstOrDefault(x => x.Id == id);
            if(result == null) { return null; }

            return new AppointmentDTO(result);
        }



        void IAppointmentService.Delete(int id)
        {
            _context.Appointments.Where(a => a.Id == id)
                .ExecuteDelete();
        }

    }
}
