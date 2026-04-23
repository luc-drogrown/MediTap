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
    public class AppointmentController : ControllerBase
    {

        private readonly ILogger<AppointmentController> _logger;
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
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
        [Authorize(Roles = "Medic,Patient")]
        [HttpGet("{id}")]
        public ActionResult<Appointment> GetById(int id)
        {
            // Check that the user is either the medic or the patient associated with the appointment
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var appointment = _appointmentService.GetById(id, userId, role);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {Id} not found", id);
                return NotFound();
            }


            // Return the appointment
            return Ok(appointment);
        }


        // Create a new appointment
        // POST: api/appointment
        [Authorize(Roles = "Medic,Patient")]
        [HttpPost]
        public IActionResult CreateAppointment(Appointment appointment)
        {
            // FIrst a sanity check
            if (appointment == null)
            {
                _logger.LogWarning("Received null appointment object in request body");
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            // Check that the user is either the medic or the patient associated with the appointment
            _logger.LogInformation("Creating a new appointment: {Appointment}", appointment);
            try
            {
                var result = _appointmentService.Add(appointment, userId, role);
                if(result == null)
                {
                    _logger.LogWarning("Failed to create appointment: {Appointment}", appointment);
                    return NotFound();
                }
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
        [Authorize(Roles = "Medic,Patient")]
        [HttpDelete("{id}")]
        public ActionResult DeleteAppointment(int id)
        {
            // Check that the user is either the medic or the patient associated with the appointment
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var appointment = _appointmentService.GetById(id, userId, role);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {Id} not found", id);
                return NotFound();
            }


            // Delete the appointment
            _logger.LogInformation("Deleting appointment with ID: {Id}", id);
            try
            {
                _appointmentService.Delete(id);
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
