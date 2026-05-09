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

        AppointmentDTO IAppointmentService.Add(AppointmentCreationDTO appointment, int userId, string role)
        {
            if(appointment == null) { return null; }
            if(!IsAppointmentDateFree(appointment, userId, role) || !IsAppointmentValid(appointment)) { return null; }
            
            
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
                MedicId = role == "Medic" ? userId : appointment.OtherUserId,
                PatientId = role == "Patient" ? userId : appointment.OtherUserId,
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

        bool IsAppointmentDateFree(AppointmentCreationDTO appointment, int userId, string role)
        {
            var windowStart = appointment.Date.AddMinutes(-15);
            var windowEnd = appointment.Date.AddMinutes(15);

            int MedicId = role == "Medic" ? userId : appointment.OtherUserId;
            int PatientId = role == "Patient" ? userId : appointment.OtherUserId;

            // Ask the database if ANY appointment overlaps this window 
            // for EITHER the Medic OR the Patient.
            bool hasConflict = _context.Appointments
                .Any(a =>
                    (a.MedicId == MedicId || a.PatientId == PatientId)
                    &&
                    (a.Date > windowStart && a.Date < windowEnd)
                );

            return !hasConflict;

        }

        bool CheckValidity(AppointmentCreationDTO appointment, int userId, string role)
        {
            bool isAuth;
            int MedicId = role == "Medic" ? userId : appointment.OtherUserId;
            int PatientId = role == "Patient" ? userId : appointment.OtherUserId;
            switch (role)
            {
                case "Medic":
                    isAuth = userId == MedicId; break;

                case "Patient":
                    isAuth = userId == PatientId; break;

                default:
                    isAuth = false; break;
            }

            return isAuth;
        }
    }
}
