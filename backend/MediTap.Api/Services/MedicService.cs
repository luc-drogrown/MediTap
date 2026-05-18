using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Exceptions;
using MediTap.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace MediTap.Api.Services
{
    public class MedicService : IMedicService
    {

        private readonly MediTapDbContext _context;
        private readonly ILogger<MedicService> _logger;
        public MedicService(MediTapDbContext context, ILogger<MedicService> logger)
        {
            _context = context;
            _logger = logger;
        }

        Medic IMedicService.Create(MedicCreationDTO request)
        {
            try
            {
                var medic = new Medic(request.FirstName, request.Specialty, request.Password, request.MedicStatus)
                {
                    LastName = request.LastName,
                    Email = request.Email != null ? new Email(request.Email) : null,
                    PhoneNumber = request.PhoneNumber != null ? new PhoneNumber(request.PhoneNumber) : null
                };
                if (_context.Medics.Any(m => m.Uname == medic.Uname))
                {
                    _logger.LogError("Attempt to create a medic with an existing username: {Uname}", medic.Uname);
                    return null;
                }

                _context.Medics.Add(medic);
                _context.SaveChanges();
                return medic;
            }
            catch (InvalidPhoneNumberException ex)
            {
                _logger.LogError(ex, "Invalid phone number provided for medic creation: {PhoneNumber}", request.PhoneNumber);
                throw;

            }
            catch (InvalidEmailException ex)
            {
                _logger.LogError(ex, "Invalid email provided for medic creation: {Email}", request.Email);
                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a medic");
                throw;
            }
        }

        IEnumerable<MedicSummaryDTO> IMedicService.GetAllForAdmin()
        {
            try
            {
                return _context.Medics
                    .Include(m => m.Patients)
                    .Include(m => m.Appointments)
                    .OrderBy(m => m.Id)
                    .Select(m => new MedicSummaryDTO(m))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving all medics for admin.");
                throw;
            }
        }

        void IMedicService.DisableAccount(int medicId)
        {
            var medic = _context.Medics.FirstOrDefault(m => m.Id == medicId);

            if (medic == null)
            {
                throw new InvalidOperationException("Medic account was not found.");
            }

            if (medic.Id == 1)
            {
                throw new InvalidOperationException("The admin account cannot be disabled.");
            }

            medic.MedicStatus = MedicStatus.Inactive;

            _context.SaveChanges();
        }

        void IMedicService.EnableAccount(int medicId)
        {
            var medic = _context.Medics.FirstOrDefault(m => m.Id == medicId);

            if (medic == null)
            {
                throw new InvalidOperationException("Medic account was not found.");
            }

            medic.MedicStatus = MedicStatus.Active;

            _context.SaveChanges();
        }

        void IMedicService.UpdateAccountForAdmin(int medicId, MedicAdminUpdateDTO request)
        {
            var medic = _context.Medics.FirstOrDefault(m => m.Id == medicId);

            if (medic == null)
            {
                throw new InvalidOperationException("Medic account was not found.");
            }

            medic.FirstName = request.FirstName;
            medic.LastName = request.LastName;
            medic.Email = new Email(request.Email);
            medic.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? null
                : new PhoneNumber(request.PhoneNumber);

            _context.SaveChanges();
        }

        MedicSummaryDTO IMedicService.GetById(int id)
        {
            var medic = _context.Medics
                .Include(m => m.Patients)
                .Include(m => m.Appointments)
                .FirstOrDefault(m => m.Id == id);


            if (medic == null)
            {
                _logger.LogWarning("Medic with ID {Id} not found", id);
                return null;
            }

            // Creates a MedicSummaryDTO from the Medic entity found in the DB
            // so that we return only neccesary information to the frontend
            var medicDTO = new MedicSummaryDTO(medic);

            return medicDTO;
        }


        PatientSummaryDTO IMedicService.Scan(PatientScanDTO request, int loggedInUserId, string role)
        {
            _logger.LogInformation("Scan initiated by UserId: {UserId} with Role: '{Role}' for Target PatientId: {PatientId}", loggedInUserId, role, request.Id);
            if (role != "Medic")
            {
                _logger.LogWarning("Scan rejected. UserId: {UserId} attempted a scan but is not a Medic.", loggedInUserId);
                return null;
            }


            try
            {
                _logger.LogInformation("Querying database for PatientId: {PatientId} using scan card credentials...", request.Id);
                // Try to find the patient from the Card informations
                var patient = _context.Patients
                    .Where(p => p.Id == request.Id)
                    .Where(p => p.Uname == request.Uname)
                    .Where(p => p.PasswordHash == request.PasswordHashed)
                    .FirstOrDefault();

                // Patient doesn't exist in the DB
                if (patient == null)
                {
                    _logger.LogWarning("Scan failed. No matching patient found for PatientId: {PatientId} with the provided Uname and PasswordHash.", request.Id);
                    return null;
                }

                // Link Patient with the Medic
                _logger.LogInformation("PatientId: {PatientId} successfully found and authenticated. Fetching Medic profile...", patient.Id);
                // Finding the Medic that is logged in
                // and including the Patients collumn 
                var medic = _context.Medics
                    .Include(m => m.Patients)
                    .FirstOrDefault(m => m.Id == loggedInUserId);

                // TODO
                // Check if Medic is null and throw custom exception then catch it in the controller

                // They are already linked
                if (medic.Patients.Any(p => p.Id == request.Id))
                {
                    _logger.LogInformation("Link already exists between PatientId: {PatientId} and MedicId: {MedicId}...", patient.Id, medic.Id);
                    return new PatientSummaryDTO(patient);
                }

                else
                {
                    _logger.LogInformation("No existing link found. Linking PatientId: {PatientId} to MedicId: {MedicId} now...", patient.Id, medic.Id);
                    medic.Patients.Add(patient);
                    _context.SaveChanges();

                    _logger.LogInformation("Successfully saved new link between PatientId: {PatientId} and MedicId: {MedicId}.", patient.Id, medic.Id);
                    return new PatientSummaryDTO(patient);
                }

            }
            catch (Exception ex)
            {
                {
                    _logger.LogError(ex, "A fatal exception occurred during the scan process for MedicId: {MedicId} scanning PatientId: {PatientId}", loggedInUserId, request.Id);

                    throw;
                }
            }
        }


        bool IMedicService.AuthCheck(int pid, int mid)
        {
            try
            {
                var authCheck = _context.Medics
                    .Any(m => m.Patients.Any(p => p.Id == pid) && m.Id == mid);
                return authCheck;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
