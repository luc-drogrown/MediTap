using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;

namespace MediTap.Api.Services
{
    public class PatientService : IPatientService
    {
        private readonly MediTapDbContext _context;
        private readonly ILogger<PatientService> _logger;
        public PatientService(MediTapDbContext context, ILogger<PatientService> logger)
        {
            _context = context;
            _logger = logger;
        }

        Patient IPatientService.Create(PatientCreationDTO request)
        {
            // Converts the PatientCreationDTO to a Patient entity
            // // in order for it to be saved on the DB
            var patient = new Patient(request.FirstName, new CNP(request.CNP), request.DateOfBirth, request.Password)
            {
                LastName = request.LastName ?? string.Empty,
                Email = request.Email != null ? new Email(request.Email) : null,
                PhoneNumber = request.PhoneNumber != null ? new PhoneNumber(request.PhoneNumber) : null,
                Address = request.Address ?? string.Empty,
            };
            _logger.LogInformation("Creating patient with CNP: {CNP}", patient.CNP.CodNumericPersonal);


            // Validate the patient data before saving

            // Check if CNP is unique
            // TODO


            // DoB needs to be in the past
            if (patient.DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
            {
                _logger.LogWarning("Attempt to create patient with future date of birth: {DateOfBirth}", patient.DateOfBirth);
                throw new Exception("Date of birth cannot be in the future.");
            }

            // CHeck if Uname is unique
            if (_context.Patients.Any(p => p.Uname == patient.Uname))
            {
                _logger.LogWarning("Attempt to create patient with existing username: {Username}", patient.Uname);
                throw new Exception("Username already exists.");
            }


            // If all validations pass, save the patient to the database
            _logger.LogInformation("Patient with CNP: {CNP} passed uniqueness check. Proceeding to save.", patient.CNP.CodNumericPersonal);
            _context.Patients.Add(patient);
            _context.SaveChanges();

            return patient;
        }


        // TODO
        Patient IPatientService.GetById(int id, int loggedInUserId, string role)
        {
            throw new NotImplementedException();
        }

        IEnumerable<PatientSummaryDTO> IPatientService.GetProfileSummary(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Symptom> IPatientService.GetSymptom(int id)
        {
            throw new NotImplementedException();
        }

        Patient IPatientService.AddSymptom(SymptomDTO symptom, int patientId, string role)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Appointment> IPatientService.GetAppointment(int id, int loggedInUserId, string role)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Medication> IPatientService.GetMedication(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Affection> IPatientService.GetAffection(int id)
        {
            throw new NotImplementedException();
        }

        public PatientDTO GetLoggedInPatient(int id)
        {
            var patient = _context.Patients
                .Where(p => p.Id == id)
                .Select(p => new PatientDTO(p))
                .FirstOrDefault();

            if(patient == null) {
                return null;
            }

            return patient;
        }


    }
}
