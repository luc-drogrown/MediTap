using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using MediTap.Api.Exceptions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                // Converts the PatientCreationDTO to a Patient entity
                // // in order for it to be saved on the DB
                var patient = new Patient(request.FirstName, new CNP(request.CNP), request.DateOfBirth, request.Password)
                {
                    LastName = request.LastName,
                    Email = request.Email != null ? new Email(request.Email) : null,
                    PhoneNumber = request.PhoneNumber != null ? new PhoneNumber(request.PhoneNumber) : null,
                    Address = request.Address ?? string.Empty,
                };
                _logger.LogInformation("Creating patient with CNP: {CNP}", patient.CNP.CodNumericPersonal);


                // Validate the patient data before saving


                // DoB needs to be in the past
                if (patient.DateOfBirth > DateOnly.FromDateTime(DateTime.Now))
                {
                    _logger.LogWarning("Attempt to create patient with future date of birth: {DateOfBirth}", patient.DateOfBirth);
                    throw new FutureDateException();
                }

                // CHeck if Uname is unique
                if (_context.Patients.Any(p => p.Uname == patient.Uname))
                {
                    _logger.LogWarning($"Attempt to create patient with existing username: {patient.Uname}");
                    throw new UnameAlreadyExistsException();
                }

                // Check if CNP is unique
                if(_context.Patients.Any(p => p.CNP.CodNumericPersonal.Equals(patient.CNP.CodNumericPersonal)))
                {
                    _logger.LogWarning($"Attempt to create patient with existing CNP: {patient.CNP.CodNumericPersonal}");
                    throw new CNPAlreadyExistsException();
                }


                // If all validations pass, save the patient to the database
                _logger.LogInformation("Patient with CNP: {CNP} passed uniqueness check. Proceeding to save.", patient.CNP.CodNumericPersonal);
                _context.Patients.Add(patient);
                _context.SaveChanges();

                return patient;
            }
            catch (InvalidPhoneNumberException ex)
            {
                _logger.LogError(ex, "Invalid phone number provided for patient creation: {PhoneNumber}", request.PhoneNumber);
                throw new InvalidPhoneNumberException("The phone number provided is not valid.");
            }
            catch (InvalidEmailException ex)
            {
                _logger.LogError(ex, "Invalid email provided for patient creation: {Email}", request.Email);
                throw new InvalidEmailException("The Email address provided is not valid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a patient");
                throw;
            }
        }


        PatientDTO IPatientService.GetById(int id, int loggedInUserId, string role)
        {
            if (role != "Medic" ) { return null; }


            try
            {
                // Gets the appointments that the Patient has with the 
                // logged in Medic. This can be null
                var appointments = _context.Appointments
                    .Where(a => a.PatientId == id)
                    .Where(a => a.MedicId == loggedInUserId)
                    .Select(a => new AppointmentDTO(a))
                    .ToList();
                

                // Selects the patient with the given ID and maps it to a PatientDTO,
                // including the appointments with the logged in Medic

                var patient = _context.Patients
                    .Where(p => p.Id == id)
                    .Where(p => p.Medics.Any(m => m.Id == loggedInUserId))
                    .Select(p => new PatientDTO
                    {
                        PatientSummary = new PatientSummaryDTO
                        {
                            Id = p.Id,
                            FirstName = p.FirstName,
                            CNP = p.CNP.CodNumericPersonal,
                            LastName = p.LastName,
                            Email = p.Email != null ? p.Email.EmailAddress : null,
                            PhoneNumber = p.PhoneNumber != null ? p.PhoneNumber.Number : null,
                        },

                        // These are already a DTO
                        Appointments = appointments,

                        // If there are no symptoms => return an empty enumerable
                        // else convert them into a DTO
                        Symptoms = p.Symptoms != null ? p.Symptoms.Select(s => new SymptomDTO(s)) : Enumerable.Empty<SymptomDTO>(),
                        Affections = p.Affections != null ? p.Affections.Select(a => new AffectionDTO(a)) : Enumerable.Empty<AffectionDTO>(),
                        Medications = p.Medications != null ? p.Medications.Select(m => new MedicationDTO(m)) : Enumerable.Empty<MedicationDTO>(),

                    })
                    .FirstOrDefault();


                return patient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving patient with ID: {Id} for medic with ID: {MedicId}", id, loggedInUserId);
                throw;
            }

        }

        PatientSummaryDTO IPatientService.GetProfileSummary(int id)
        {
            try
            {
                var patientSummary = _context.Patients
                    .Where(p => p.Id == id)
                    .Select(p => new PatientSummaryDTO(p))
                    .FirstOrDefault();

                if(patientSummary == null)
                {
                    _logger.LogWarning("Patient with ID {Id} not found when attempting to retrieve profile summary", id);
                    throw new PatientNotFoundException(id);
                }

                _logger.LogInformation("Retrieved profile summary for patient with ID: {Id}", id);
                return patientSummary;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving profile summary for patient with ID: {Id}", id);
                throw;
            }
        }


        IEnumerable<SymptomDTO> IPatientService.GetSymptom(int id)
        {
            try
            {
                var symptom = _context.Symptoms
                    .Where(s => s.PatientId == id)
                    .AsEnumerable()
                    .Select(s => new SymptomDTO(s))
                    .ToList();

                return symptom;

            }
            catch(Exception e)
            {
                _logger.LogWarning($"An error occured when trying to find symptoms for PatientId {id}", id);
                throw;
            }

        }

        SymptomDTO IPatientService.AddSymptom(SymptomCreationDTO symptom, int loggedInUserId, string role)
        {
            try
            {
                // Checks if the loggedInUser is a PAtient
                if(role != "Patient") 
                {
                    _logger.LogWarning("Trying to create symptom as a non-Patient.");
                    return null; 
                }

                var now = DateTime.UtcNow;
                // Sanity check on the dates

                if (symptom.AddedTime.CompareTo(now) > 0)
                {
                    _logger.LogWarning($"Trying to add a symptom in the future");
                    throw new FutureDateException();
                }
                if (symptom.StartOfSymptom.HasValue)
                {
                    if (symptom.StartOfSymptom.Value.CompareTo(DateOnly.FromDateTime(now)) > 0)
                    {
                        _logger.LogWarning($"Trying to add a symptom in the future, id");
                        throw new FutureDateException();
                    }
                }

                // The symptom is valid
                // Time to map it to a actual Symptom Entity in the DB
                // I do not map Id because thats automatically
                var symptomEntity = new Symptom
                {
                    Name = symptom.Name,
                    isChecked = false,
                    isPresent = true,
                    AddedDate = symptom.AddedTime,
                    StartOfSymptoms = symptom.StartOfSymptom,
                    Description = symptom.Description,

                    // take this Id from the loggedInUser
                    PatientId = loggedInUserId,
                    //MedicId = symptom.MedicId != null ? symptom.MedicId : null,

                };


                _context.Symptoms.Add(symptomEntity);
                _context.SaveChanges();
                return new SymptomDTO(symptomEntity);
            }
            catch (Exception e)
            {
                _logger.LogWarning("An error occured when creating a new symptom.");
                throw;
            }
        }

        IEnumerable<AppointmentDTO> IPatientService.GetAppointment(int id, int loggedInUserId, string role)
        {
            try
            {
                if(role != "Medic") { return null; }
                var appointments = _context.Appointments
                    .Where(a => a.PatientId == id && a.MedicId == loggedInUserId)
                    .AsEnumerable()
                    .Select(a => new AppointmentDTO(a))
                    .ToList();
                _logger.LogInformation($"Medic {loggedInUserId} got the appointments for Patient {id}.");
                return appointments;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured when trying to show appointments for Pid {id} | Mid {loggedInUserId}.");
                throw;
            }
        }

        IEnumerable<MedicationDTO> IPatientService.GetMedication(int id)
        {
            try
            {
                var medication = _context.Medications
                    .Where(m => m.PatientId == id)
                    .AsEnumerable()
                    .Select(m => new MedicationDTO(m))
                    .ToList();
                _logger.LogInformation($"Medication for Pid {id} retrieved.");
                return medication;
            }
            catch (Exception e)
            {
                _logger.LogError($"Medication for Pid {id} could not be retrieved.");
                throw;
            }
        }


        IEnumerable<AffectionDTO> IPatientService.GetAffection(int id)
        {
            try
            {
                var affection = _context.Affections
                    .Where(m => m.PatientId == id)
                    .AsEnumerable()
                    .Select(a => new  AffectionDTO(a))
                    .ToList();
                _logger.LogInformation($"Affections for Pid {id} retrieved.");
                return affection;
            }
            catch
            {
                _logger.LogError($"An error occured when trying to get affections for Pid {id}");
                throw;
            }
        }



        public PatientDTO GetLoggedInPatient(int id)
        {
            var patient = _context.Patients
                .Include(p => p.Medications)
                .Include(p => p.Affections)
                .Include(p => p.Symptoms)
                .Include(p => p.Appointments)
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
