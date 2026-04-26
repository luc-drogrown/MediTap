using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
using MediTap.Api.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediTap.Api.Services.Interfaces;
using MediTap.Api.DTO;
namespace MediTap.Api.Controllers
{
    // DONE
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientService _patientService;
        public PatientController(ILogger<PatientController> logger, IPatientService patientService)
        {
            _logger = logger;
            _patientService = patientService;
        }
        //--------------------------------------------------------------------//


        /// GET methods

        // Show the current patient's information
        // This includes the patient's profile, appointments, symptoms, affections and medications
        // GET: api/patient/me
        [Authorize(Roles = "Patient")]
        [HttpGet("me")]
        public ActionResult<PatientDTO> GetMyProfile()
        {
            // Finds the Id of the logged in user from the JWT
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                var patient = _patientService.GetLoggedInPatient(loggedInUserId);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {Id} not found", loggedInUserId);
                    return NotFound();
                }
                _logger.LogInformation("Patient profile retrieved for ID: {Id}", loggedInUserId);
                return Ok(patient);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient profile for ID: {Id}", loggedInUserId);
                return StatusCode(500, "An error occurred while retrieving the patient profile.");

            }
        }



        // Show all patients with pagination
        // GET: api/patient?pageNumber=1
        //[HttpGet]
        //public ActionResult<IEnumerable<Patient>> GetAll(int? pageNumber)
        //{
        //    _logger.LogInformation("Getting all patients, page number: {PageNumber}", pageNumber);
        //    // Default page number is 1 if not provided
        //    return PaginatedList<Patient>.Create(PatientService.GetAll(), pageNumber ?? 1, 10);
        //}


        // Show patient by ID
        // GET: api/patient/5
        //[HttpGet("{id}")]
        //public ActionResult<Patient> GetById(int id)
        //{
        //    _logger.LogInformation("Getting patient by ID: {Id}", id);
        //    var patient = PatientService.GetById(id);
        //    _logger.LogInformation("Patient found: {Patient}", patient);
        //    if (patient == null)
        //    {
        //        _logger.LogWarning("Patient with ID {Id} not found", id);
        //        return NotFound();
        //    }
        //    return Ok(patient);
        //}


        // CAN BE ONLY USED BY MEDIC
        // Show appointments for a patient
        // GET: api/patient/5/appointment
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}/appointment")]
        public ActionResult<IEnumerable<AppointmentDTO>> GetAppointments(int id)
        {
            try
            {
                _logger.LogInformation("Getting appointments for patient ID: {Id}", id);
                var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                // Here it checks if the patient and medic are linked,
                // if not it returns null ==> NotFound
                var appointments = _patientService.GetAppointment(id, loggedInUserId, role);
                if (appointments == null)
                {
                    _logger.LogWarning("Patient with ID {Id} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Appointments retrieved for patient ID: {Id}", id);
                return Ok(appointments);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for patient ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the appointments.");
            }
        }

        // TODO
        // When a Medic calls this, the MedicId in the SymptomDTO updates
        // CAN BE ONLY USED BY MEDIC
        // Show symptoms for a patient
        // GET: api/patient/5/symptom
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}/symptom")]
        public ActionResult<IEnumerable<SymptomDTO>> GetSymptoms(int id)
        {
            _logger.LogInformation("Getting symptoms for patient ID: {Id}", id);
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var patient = _patientService.GetById(id, loggedInUserId, role);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                var symptoms = _patientService.GetSymptom(id);
                _logger.LogInformation("Symptoms retrieved for patient ID: {Id}", id);
                return Ok(symptoms);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving symptoms for patient ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the symptoms.");
            }
        }

        // CAN BE ONLY USED BY MEDIC
        // Show affections for a patient
        // GET: api/patient/5/affection
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}/affection")]
        public ActionResult<IEnumerable<AffectionDTO>> GetAffections(int id)
        {
            _logger.LogInformation("Getting affections for patient ID: {Id}", id);
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var patient = _patientService.GetById(id,loggedInUserId, role);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                var affections = _patientService.GetAffection(id);
                _logger.LogInformation("Affections retrieved for patient ID: {Id}", id);
                return Ok(affections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving affections for patient ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the affections.");
            }
        }

        // CAN BE ONLY USED BY MEDIC
        // Show medications for a patient
        // GET: api/patient/5/medication
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}/medication")]
        public ActionResult<IEnumerable<MedicationDTO>> GetMedications(int id)
        {
            _logger.LogInformation("Getting medications for patient ID: {Id}", id);
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var patient = _patientService.GetById(id,loggedInUserId, role);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                var medications = _patientService.GetMedication(id);
                _logger.LogInformation("Medications retrieved for patient ID: {Id}", id);
                return Ok(medications);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medications for patient ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the medications.");
            }
        }


        // CAN BE ONLY USED BY MEDIC
        // Show profile summary for a patient
        // GET: api/patient/5/profile
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}/profile")]
        public ActionResult<PatientSummaryDTO> GetProfileSummary(int id)
        {
            _logger.LogInformation("Getting profile summary for patient ID: {Id}", id);
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // This method returns way too much data for how much the final DTO sends to the client
            // We only need to check that the medic has permissions
            // TODO
            // FIx this and make it more efficient
            var patient = _patientService.GetById(id,loggedInUserId, role);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                var profileSummary = _patientService.GetProfileSummary(id);
                return Ok(profileSummary);
            }
            catch (PatientNotFoundException ex)
            {
                _logger.LogWarning(ex, "Patient not found when attempting to retrieve profile summary for patient ID: {Id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile summary for patient ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the profile summary.");
            }
        }



        //--------------------------------------------------------------------//


        // POST methods

        // CAN BE ONLY USED BY PATIENT
        // Add a symptom
        // POST: api/patient/me/symptom
        [Authorize(Roles = "Patient")]
        [HttpPost("me/symptom")]
        public ActionResult<SymptomDTO> CreateSymptom(SymptomCreationDTO symptom)
        {
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;


            try
            {
                // loggedInUserID must be equal to symptom.PatientId
                var response = _patientService.AddSymptom(symptom, loggedInUserId, role);
                if (response == null)
                {
                    _logger.LogWarning("Failed to add symptom for patient ID: {Id}, Symptom: {Symptom}", loggedInUserId, symptom);
                    return NotFound();
                }
            
                _logger.LogInformation("Symptom added for patient ID: {Id}, Symptom: {Symptom}", loggedInUserId, symptom);
                return CreatedAtAction(nameof(GetMyProfile), null, response);
            }
            catch(FutureDateException ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500,ex.Message);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding symptom for patient ID: {Id}, Symptom: {Symptom}", loggedInUserId, symptom);
                return StatusCode(500, "An error occurred while adding the symptom.");
            }

        }


        // Creating a Patient
        // Creation can be only done by the Admin
        // Admin is the Medic with Id = 1
        // POST: api/patient
        [Authorize(Roles = "Medic")]
        [HttpPost]
        public IActionResult CreatePatient([FromBody] PatientCreationDTO request)
        {
            // Checks is the logged in user is the Admin (Medic with Id = 1)
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userId != 1 && role != "Medic")
            {
                _logger.LogWarning("Unauthorized attempt to create patient by user ID: {UserId}, Role: {Role}", userId, role);
                return NotFound();
            }

            // tries to create the patient and logs the outcome
            try
            {
                var response = _patientService.Create(request);
                _logger.LogInformation("Patient created successfully by user ID: {UserId}, Role: {Role}", userId, role);
                return Ok(response);
            }


            // Catches specific exceptions related to patient creation 
            catch (InvalidPhoneNumberException ex)
            {
                _logger.LogError(ex, "Invalid phone number provided for patient creation by user ID: {UserId}, Role: {Role}", userId, role);
                return BadRequest(ex.Message);
            }
            catch (InvalidEmailException ex)
            {
                _logger.LogError(ex, "Invalid email provided for patient creation by user ID: {UserId}, Role: {Role}", userId, role);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient by user ID: {UserId}, Role: {Role}", userId, role);
                return StatusCode(500, "An error occurred while creating the patient.");
            }
        }


    }
}
