using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MediTap.Api.Services.Interfaces
{
    public class AppointmentService : IAppointmentService
    {

        private readonly MediTapDbContext _context;

        public AppointmentService(MediTapDbContext context)
        {
            _context = context;
        }

        // TODO --> Make a DTO for Medic and one for the Patient so that they don't have to type out their own id
        AppointmentDTO IAppointmentService.Add(AppointmentCreationDTO appointment, int userId, string role)
        {
            if(appointment == null) { return null; }
            if(!IsAppointmentDateFree(appointment) || !IsAppointmentValid(appointment)) { return null; }
            
            
            // make sure the userID is equal to apppointment.MedicId or apppointment.PatientID depending on the role of the user
            if(!CheckValidity(appointment, userId, role)) { return null; }

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

        bool IsAppointmentValid(AppointmentCreationDTO appointment)
        {
            if (appointment == null) { return false; }

            // Checks for issue date to be in the past or present
            if (appointment.IssueDate > DateTime.Now) { return false; }

            // Checks for the date to be in the future
            if (appointment.Date.Date < DateTime.Now.Date) { return false; }

            return true;
        }

        bool IsAppointmentDateFree(AppointmentCreationDTO appointment)
        {
            var windowStart = appointment.Date.AddMinutes(-15);
            var windowEnd = appointment.Date.AddMinutes(15);

            // Ask the database if ANY appointment overlaps this window 
            // for EITHER the Medic OR the Patient.
            bool hasConflict = _context.Appointments
                .Any(a =>
                    (a.MedicId == appointment.MedicId || a.PatientId == appointment.PatientId)
                    &&
                    (a.Date > windowStart && a.Date < windowEnd)
                );

            return !hasConflict;

        }

        bool CheckValidity(AppointmentCreationDTO appointment, int userId, string role)
        {
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

            return isAuth;
        }
    }
}
