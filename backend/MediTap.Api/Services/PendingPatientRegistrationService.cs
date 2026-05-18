using MediTap.Api.DTO;
using MediTap.Api.Exceptions;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using System.Collections.Concurrent;

namespace MediTap.Api.Services
{
    public class PendingPatientRegistrationService : IPendingPatientRegistrationService
    {
        private readonly ConcurrentDictionary<string, PatientCreationDTO> _pendingRegistrations = new();
        private readonly IServiceScopeFactory _scopeFactory;

        public PendingPatientRegistrationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public PendingPatientRegistrationDTO Prepare(PatientCreationDTO request)
        {
            var uname = GeneratePatientUname(request.FirstName, request.CNP);

            request.Uname = uname;

            var patient = new Patient(request.FirstName, new CNP(request.CNP), request.DateOfBirth, request.Password, request.Uname)
            {
                LastName = request.LastName,
                Email = new Email(request.Email),
                PhoneNumber = request.PhoneNumber != null ? new PhoneNumber(request.PhoneNumber) : null,
                Address = request.Address ?? string.Empty,
            };

            if (patient.DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new FutureDateException();
            }

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MediTapDbContext>();

            if (context.Patients.Any(p => p.Uname == patient.Uname))
            {
                throw new UnameAlreadyExistsException();
            }

            if (context.Patients.Any(p => p.CNP == patient.CNP))
            {
                throw new CNPAlreadyExistsException();
            }

            var pendingRegistrationId = Guid.NewGuid().ToString();

            _pendingRegistrations[pendingRegistrationId] = request;

            return new PendingPatientRegistrationDTO
            {
                PendingRegistrationId = pendingRegistrationId,
                Uname = uname
            };
        }

        public Patient Confirm(string pendingRegistrationId)
        {
            if (!_pendingRegistrations.TryRemove(pendingRegistrationId, out var request))
            {
                throw new InvalidOperationException("Pending patient registration was not found.");
            }

            using var scope = _scopeFactory.CreateScope();
            var patientService = scope.ServiceProvider.GetRequiredService<IPatientService>();

            return patientService.Create(request);
        }

        public void Cancel(string pendingRegistrationId)
        {
            _pendingRegistrations.TryRemove(pendingRegistrationId, out _);
        }

        private static string GeneratePatientUname(string firstName, string cnp)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName), "First name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(cnp) || cnp.Length < 4)
            {
                throw new InvalidCNPException("CNP must contain at least 4 digits.");
            }

            return "P-" + firstName + "-" + cnp.Substring(0, 4) + "-" + Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}