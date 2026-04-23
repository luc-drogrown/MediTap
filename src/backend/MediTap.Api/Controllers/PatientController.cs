using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
namespace MediTap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        public PatientController(ILogger<PatientController> logger)
        {
            _logger = logger;
        }
        //--------------------------------------------------------------------//


        /// GET methods



        // Show all patients with pagination
        // GET: api/patient?pageNumber=1
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAll(int? pageNumber)
        {
            _logger.LogInformation("Getting all patients, page number: {PageNumber}", pageNumber);
            // Default page number is 1 if not provided
            return PaginatedList<Patient>.Create(PatientService.GetAll(), pageNumber ?? 1, 10);
        }


        // Show patient by ID
        // GET: api/patient/5
        [HttpGet("{id}")]
        public ActionResult<Patient> GetById(int id)
        {
            _logger.LogInformation("Getting patient by ID: {Id}", id);
            var patient = PatientService.GetById(id);
            _logger.LogInformation("Patient found: {Patient}", patient);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            return Ok(patient);
        }


        // Show appointments for a patient
        // GET: api/patient/5/appointment
        [HttpGet("{id}/appointment")]
        public ActionResult<IEnumerable<Appointment>> GetAppointments(int id)
        {
            _logger.LogInformation("Getting appointments for patient ID: {Id}", id);
            var patient = PatientService.GetById(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            var appointments = PatientService.GetAppointments(id);
            return Ok(appointments);
        }

        // Show symptoms for a patient
        // GET: api/patient/5/symptom
        [HttpGet("{id}/symptom")]
        public ActionResult<IEnumerable<Symptom>> GetSymptoms(int id)
        {
            _logger.LogInformation("Getting symptoms for patient ID: {Id}", id);
            var patient = PatientService.GetById(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            var symptoms = PatientService.GetSymptoms(id);
            return Ok(symptoms);
        }


        // Show affections for a patient
        // GET: api/patient/5/affection
        [HttpGet("{id}/affection")]
        public ActionResult<IEnumerable<Affection>> GetAffections(int id)
        {
            _logger.LogInformation("Getting affections for patient ID: {Id}", id);
            var patient = PatientService.GetById(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            var affections = PatientService.GetAffections(id);
            return Ok(affections);
        }


        // Show medications for a patient
        // GET: api/patient/5/medication
        [HttpGet("{id}/medication")]
        public ActionResult<IEnumerable<Medication>> GetMedications(int id)
        {
            _logger.LogInformation("Getting medications for patient ID: {Id}", id);
            var patient = PatientService.GetById(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found", id);
                return NotFound();
            }
            var medications = PatientService.GetMedications(id);
            return Ok(medications);
        }



        //--------------------------------------------------------------------//


        // POST methods


        // Add a symptom
        // POST: api/patient/5/symptom
        [HttpPost("{id}/symptom")]
        public IActionResult CreateSymptom(int id, Symptom symptom)
        {
            _logger.LogInformation("Adding symptom for patient ID: {Id}, Symptom: {Symptom}", id, symptom);
            var patient = PatientService.GetById(id);
            _logger.LogInformation("Patient found for symptom addition: {Patient}", patient);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found for symptom addition", id);
                return NotFound();
            }
            PatientService.AddSymptom(id, symptom);
            _logger.LogInformation("Symptom added for patient ID: {Id}, Symptom: {Symptom}", id, symptom);
            return CreatedAtAction(nameof(GetById), new { id = id }, symptom);

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
