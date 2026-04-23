using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediTap.Api.Services.Interfaces;
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
        public ActionResult<Patient> GetMyProfile()
        {
            // Finds the Id of the logged in user from the JWT
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                var patient = _patientService.GetById(loggedInUserId);
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

            // THESE INFORMATIONS ARE FOUND IN THE 
            // GET BY ID METHOD OF THE PATIENT CONTROLLER TOO

            //// Show appointments for a patient
            //// GET: api/patient/5/appointment
            //[HttpGet("{id}/appointment")]
            //public ActionResult<IEnumerable<Appointment>> GetAppointments(int id)
            //{
            //    _logger.LogInformation("Getting appointments for patient ID: {Id}", id);
            //    var patient = PatientService.GetById(id);
            //    if (patient == null)
            //    {
            //        _logger.LogWarning("Patient with ID {Id} not found", id);
            //        return NotFound();
            //    }
            //    var appointments = PatientService.GetAppointments(id);
            //    return Ok(appointments);
            //}

            //// Show symptoms for a patient
            //// GET: api/patient/5/symptom
            //[HttpGet("{id}/symptom")]
            //public ActionResult<IEnumerable<Symptom>> GetSymptoms(int id)
            //{
            //    _logger.LogInformation("Getting symptoms for patient ID: {Id}", id);
            //    var patient = PatientService.GetById(id);
            //    if (patient == null)
            //    {
            //        _logger.LogWarning("Patient with ID {Id} not found", id);
            //        return NotFound();
            //    }
            //    var symptoms = PatientService.GetSymptoms(id);
            //    return Ok(symptoms);
            //}


            //// Show affections for a patient
            //// GET: api/patient/5/affection
            //[HttpGet("{id}/affection")]
            //public ActionResult<IEnumerable<Affection>> GetAffections(int id)
            //{
            //    _logger.LogInformation("Getting affections for patient ID: {Id}", id);
            //    var patient = PatientService.GetById(id);
            //    if (patient == null)
            //    {
            //        _logger.LogWarning("Patient with ID {Id} not found", id);
            //        return NotFound();
            //    }
            //    var affections = PatientService.GetAffections(id);
            //    return Ok(affections);
            //}


            //// Show medications for a patient
            //// GET: api/patient/5/medication
            //[HttpGet("{id}/medication")]
            //public ActionResult<IEnumerable<Medication>> GetMedications(int id)
            //{
            //    _logger.LogInformation("Getting medications for patient ID: {Id}", id);
            //    var patient = PatientService.GetById(id);
            //    if (patient == null)
            //    {
            //        _logger.LogWarning("Patient with ID {Id} not found", id);
            //        return NotFound();
            //    }
            //    var medications = PatientService.GetMedications(id);
            //    return Ok(medications);
            //}



            //--------------------------------------------------------------------//


            // POST methods


            // Add a symptom
            // POST: api/patient/me/symptom
            [Authorize(Roles = "Patient")]
            [HttpPost("me/symptom")]
        public ActionResult<Symptom> CreateSymptom(Symptom symptom)
            {
                var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("Adding symptom for patient ID: {Id}, Symptom: {Symptom}", loggedInUserId, symptom);
                var patient = _patientService.GetById(loggedInUserId);
                _logger.LogInformation("Patient found for symptom addition: {Patient}", patient);
                
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {Id} not found for symptom addition", loggedInUserId);
                    return NotFound();
                }

                try
                {
                    var response = _patientService.AddSymptom(symptom, loggedInUserId, role);
                    if (response == null) { 
                        _logger.LogWarning("Failed to add symptom for patient ID: {Id}, Symptom: {Symptom}", loggedInUserId, symptom);
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding symptom for patient ID: {Id}, Symptom: {Symptom}", loggedInUserId, symptom);
                    return StatusCode(500, "An error occurred while adding the symptom.");
                }
                _logger.LogInformation("Symptom added for patient ID: {Id}, Symptom: {Symptom}", loggedInUserId, symptom);
                return CreatedAtAction(nameof(GetMyProfile), null, symptom);

            }



            // THIS IS IMPLEMENTED IN THE 
            // APPOINTMENT CONTROLLER

            // Add an appointment
            // POST: api/patient/5/appointment
            //[HttpPost("{id}/appointment")]
            //public IActionResult CreateAppointment(int id, Appointment appointment)
            //{
            //    // LOG
            //    _logger.LogInformation("Adding appointment for patient ID: {Id}, Appointment: {Appointment}", id, appointment);

            //    var patient = PatientService.GetById(id);

            //    // LOG
            //    _logger.LogInformation("Patient found for appointment addition: {Patient}", patient);

            //    if (patient == null)
            //    {
            //        // LOG
            //        _logger.LogWarning("Patient with ID {Id} not found for appointment addition", id);

            //        return NotFound();
            //    }
            //    PatientService.AddAppointment(id, appointment);

            //    // LOG
            //    _logger.LogInformation("Appointment added for patient ID: {Id}, Appointment: {Appointment}", id, appointment);

            //    return CreatedAtAction(nameof(GetById), new { id = id }, appointment);
            //}


        }
    }
