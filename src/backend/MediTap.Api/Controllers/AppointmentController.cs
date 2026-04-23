using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
namespace MediTap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {

        private readonly ILogger<AppointmentController> _logger;
        public AppointmentController(ILogger<AppointmentController> logger)
        {
            _logger = logger;
        }


        // Show all appointments with pagination
        // GET: api/appointment?pageNumber=1
        //[HttpGet]
        //public ActionResult<IEnumerable<Appointment>> GetAll(int? pageNumber)
        //{
        //    _logger.LogInformation("Getting all appointments, page number: {PageNumber}", pageNumber);
        //    return PaginatedList<Appointment>.Create(AppointmentService.GetAll(), pageNumber ?? 1, 10);
        //}


        // Get appointment by ID
        // GET: api/appointment/5
        [HttpGet("{id}")]
        public ActionResult<Appointment> GetById(int id)
        {
            var appointment = AppointmentService.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        // TODO 
        // Implement logic for medic vs patient appointment
        // Create a new appointment
        // POST: api/appointment
        [HttpPost]
        public IActionResult CreateAppointment(Appointment appointment)
        {
            _logger.LogInformation("Creating a new appointment: {Appointment}", appointment);
            try
            {
                AppointmentService.Add(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating appointment: {Appointment}", appointment);
                return StatusCode(500, "An error occurred while creating the appointment.");
            }

            _logger.LogInformation("Appointment created successfully: {Appointment}", appointment);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }


        // Delete an appointment
        // DELETE: api/appointment/5
        [HttpDelete("{id}")]
        public ActionResult DeleteAppointment(int id)
        {
            _logger.LogInformation("Deleting appointment with ID: {Id}", id);
            var appointment = AppointmentService.GetById(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                AppointmentService.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting appointment with ID: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the appointment.");
            }
            _logger.LogInformation("Appointment with ID {Id} deleted successfully", id);
            return NoContent(); 
        }

    }
}
