using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
namespace MediTap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicController : ControllerBase
    {
        private readonly ILogger<MedicController> _logger;
        public MedicController(ILogger<MedicController> logger)
        {
            _logger = logger;
        }


        // Get medic by Id
        // GET: api/medic/5
        [HttpGet("{id}")]
        public ActionResult<Medic> GetById(int id)
        {
            _logger.LogInformation("Getting medic by ID: {Id}", id);
            var medic = MedicService.GetById(id);
            if (medic == null)
            {
                _logger.LogWarning("Medic with ID {Id} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Medic with ID {Id} retrieved successfully", id);
            return Ok(medic);

        }


        // Get all patient of a medic
        // GET: api/medic/5/patient
        [HttpGet("{id}/patient")]
        public ActionResult<IEnumerable<Patient>> GetPatients(int id)
        {
            _logger.LogInformation("Getting patients for medic ID: {Id}", id);
            var medic = MedicService.GetById(id);
            if (medic == null)
            {
                _logger.LogWarning("Medic with ID {Id} not found", id);
                return NotFound();
            }
            var patients = MedicService.GetPatients(id);
            _logger.LogInformation("Patients for medic ID {Id} retrieved successfully", id);
            return Ok(patients);
        }


        // Get all appointments of a medic
        // GET: api/medic/5/appointment
        [HttpGet("{id}/appointment")]
        public ActionResult<IEnumerable<Appointment>> GetAppointments(int id)
        {
            _logger.LogInformation("Getting appointments for medic ID: {Id}", id);
            var medic = MedicService.GetById(id);
            if (medic == null)
            {
                _logger.LogWarning("Medic with ID {Id} not found", id);
                return NotFound();
            }
            var appointments = MedicService.GetAppointments(id);
            _logger.LogInformation("Appointments for medic ID {Id} retrieved successfully", id);
            return Ok(appointments);
        }

    }
}
