using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using System.Net.Http.Headers;

namespace MediTap.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly MediTapDbContext _context;

        public AuthService(MediTapDbContext context)
        {
            _context = context;
        }

        bool IAuthService.IsAppointmentAuth(int patientId, int medicId, int appointmentId)
        {
            throw new NotImplementedException();
        }

        bool IAuthService.IsAssociatedWithAppointment(int appointmentId, int userId, string role)
        {
            bool isAuth;

            switch (role)
            {

                case "Medic":
                    isAuth = _context.Medics.Any(m => m.Id == userId && m.Appointments.Any(a => a.Id == appointmentId)); 
                    break;

                case "Patient":
                    isAuth = _context.Patients.Any(p => p.Id == userId && p.Appointments.Any(a => a.Id == appointmentId));
                    break;

                default:
                    isAuth = false;
                    // TODO -> Implement a custom exception for this case
                    break;
            }

            return isAuth;

        }

        bool IAuthService.IsPatientMedicLinked(int patientId, int medicId)
        {
            bool isAuth = _context.Patients.Any(p => p.Id == patientId && p.Medics.Any(m => m.Id ==  medicId));
            return isAuth;
        }

    }
}
