using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
using MediTap.Api.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediTap.Api.Services.Interfaces;
using MediTap.Api.DTO;
namespace MediTap.Api.Controllers
{
    // TODO -> eliminate all 'role' variables as we don't need them
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientService _patientService;
        private readonly IPendingPatientRegistrationService _pendingPatientRegistrationService;

        public PatientController(
            ILogger<PatientController> logger,
            IPatientService patientService,
            IPendingPatientRegistrationService pendingPatientRegistrationService)
        {
            _logger = logger;
            _patientService = patientService;
            _pendingPatientRegistrationService = pendingPatientRegistrationService;
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

        // Show all patients for Admin
        // Admin is the Medic with Id = 1
        // GET: api/patient/admin/all
        [Authorize(Roles = "Medic")]
        [HttpGet("admin/all")]
        public ActionResult<IEnumerable<PatientSummaryDTO>> GetAllPatientsForAdmin()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to view all patients by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                var patients = _patientService.GetAllForAdmin();

                _logger.LogInformation("All patients retrieved successfully by admin user ID: {UserId}", userId);

                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all patients for admin user ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving patients.");
            }
        }

        // Disable a patient account
        // Admin is the Medic with Id = 1
        // PUT: api/patient/admin/{patientId}/disable
        [Authorize(Roles = "Medic")]
        [HttpPut("admin/{patientId}/disable")]
        public IActionResult DisablePatientAccount(int patientId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to disable patient account by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                _patientService.DisableAccount(patientId);

                _logger.LogInformation("Patient account disabled successfully by admin user ID: {UserId}. Patient ID: {PatientId}", userId, patientId);

                return Ok(new
                {
                    message = "Patient account disabled successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Patient account not found. Patient ID: {PatientId}", patientId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling patient account. Patient ID: {PatientId}", patientId);
                return StatusCode(500, "An error occurred while disabling the patient account.");
            }
        }

        // Enable a patient account
        // Admin is the Medic with Id = 1
        // PUT: api/patient/admin/{patientId}/enable
        [Authorize(Roles = "Medic")]
        [HttpPut("admin/{patientId}/enable")]
        public IActionResult EnablePatientAccount(int patientId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to enable patient account by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                _patientService.EnableAccount(patientId);

                _logger.LogInformation("Patient account enabled successfully by admin user ID: {UserId}. Patient ID: {PatientId}", userId, patientId);

                return Ok(new
                {
                    message = "Patient account enabled successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Patient account not found. Patient ID: {PatientId}", patientId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling patient account. Patient ID: {PatientId}", patientId);
                return StatusCode(500, "An error occurred while enabling the patient account.");
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

        // Cancelling a Patient registration
        // This removes the pending registration if the admin cancels before writing the NFC card
        // Admin is the Medic with Id = 1
        // POST: api/patient/cancel-registration/{pendingRegistrationId}
        [Authorize(Roles = "Medic")]
        [HttpPost("cancel-registration/{pendingRegistrationId}")]
        public IActionResult CancelPatientRegistration(string pendingRegistrationId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to cancel patient registration by user ID: {UserId}", userId);
                return NotFound();
            }

            _pendingPatientRegistrationService.Cancel(pendingRegistrationId);

            _logger.LogInformation(
                "Pending patient registration cancelled successfully by user ID: {UserId}. Pending ID: {PendingRegistrationId}",
                userId,
                pendingRegistrationId);

            return Ok(new
            {
                message = "Pending patient registration cancelled successfully."
            });
        }

        // Preparing a Patient registration
        // This does NOT save the patient in the database yet
        // It only generates the Uname that will be written on the NFC card
        // Admin is the Medic with Id = 1
        // POST: api/patient/prepare-registration
        [Authorize(Roles = "Medic")]
        [HttpPost("prepare-registration")]
        public IActionResult PreparePatientRegistration([FromBody] PatientCreationDTO request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to prepare patient registration by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                var response = _pendingPatientRegistrationService.Prepare(request);

                _logger.LogInformation(
                    "Pending patient registration prepared successfully by user ID: {UserId}",
                    userId);

                return Ok(response);
            }
            catch (InvalidCNPException ex)
            {
                _logger.LogError(ex, "Invalid CNP provided while preparing patient registration by user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (InvalidPhoneNumberException ex)
            {
                _logger.LogError(ex, "Invalid phone number while preparing patient registration by user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (InvalidEmailException ex)
            {
                _logger.LogError(ex, "Invalid email while preparing patient registration by user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (FutureDateException ex)
            {
                _logger.LogError(ex, "Future date of birth while preparing patient registration by user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (UnameAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Generated username already exists while preparing patient registration by user ID: {UserId}", userId);
                return BadRequest(ex.Message + " Please try again.");
            }
            catch (CNPAlreadyExistsException ex)
            {
                _logger.LogError(ex, "CNP already exists while preparing patient registration by user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing patient registration by user ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while preparing the patient registration.");
            }
        }

        // Confirming a Patient registration
        // This saves the patient in the database only after the NFC card was written successfully
        // Admin is the Medic with Id = 1
        // POST: api/patient/confirm-registration/{pendingRegistrationId}
        [Authorize(Roles = "Medic")]
        [HttpPost("confirm-registration/{pendingRegistrationId}")]
        public IActionResult ConfirmPatientRegistration(string pendingRegistrationId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to confirm patient registration by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                var patient = _pendingPatientRegistrationService.Confirm(pendingRegistrationId);

                _logger.LogInformation(
                    "Pending patient registration confirmed successfully by user ID: {UserId}. Patient ID: {PatientId}",
                    userId,
                    patient.Id);

                return Ok(patient);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Pending patient registration not found. ID: {PendingRegistrationId}", pendingRegistrationId);
                return NotFound(ex.Message);
            }
            catch (InvalidCNPException ex)
            {
                _logger.LogError(ex, "Invalid CNP while confirming patient registration. ID: {PendingRegistrationId}", pendingRegistrationId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming patient registration. ID: {PendingRegistrationId}", pendingRegistrationId);
                return StatusCode(500, "An error occurred while confirming the patient registration.");
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
            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to create patient by user ID: {UserId}", userId);
                return NotFound();
            }

            // tries to create the patient and logs the outcome
            try
            {
                var response = _patientService.Create(request);
                _logger.LogInformation("Patient created successfully by user ID: {UserId}", userId);
                return Ok(response);
            }


            // Catches specific exceptions related to patient creation 

            // Invalid data 
            catch (InvalidPhoneNumberException ex)
            {
                _logger.LogError(ex, "Invalid phone number provided for patient creation by user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (InvalidEmailException ex)
            {
                _logger.LogError(ex, "Invalid email provided for patient creation by user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (InvalidCNPException ex)
            {
                _logger.LogError(ex, "CNP for patient creation by user ID is invalid: {UserId}", userId);
                return BadRequest(ex.Message);
            }

            // Not unique data that should be unique
            catch ( UnameAlreadyExistsException ex)
            {
                _logger.LogError(ex, "Uname for patient creation by user ID already exists: {UserId}", userId);
                return BadRequest(ex.Message + "Please try again.");
            }
            catch(CNPAlreadyExistsException ex)
            {
                _logger.LogError(ex, "CNP for patient creation by user ID already exists: {UserId}", userId);
                return BadRequest(ex.Message);
            }

            // General error
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient by user ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while creating the patient.");
            }
        }


    }
}
