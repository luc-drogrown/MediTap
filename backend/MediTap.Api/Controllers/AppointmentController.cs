using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediTap.Api.Services.Interfaces;
using MediTap.Api.DTO;

namespace MediTap.Api.Controllers
{
    // DONE
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {

        private readonly ILogger<AppointmentController> _logger;
        private readonly IAppointmentService _appointmentService;
        private readonly IAuthService _authService;
        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService appointmentService, IAuthService authService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
            _authService = authService;
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
        public ActionResult<AppointmentDTO> GetById(int id)
        {
            // Check that the user is either the medic or the patient associated with the appointment
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // CHecking if the user has permission to access the Appoitnment
            if(!_authService.IsAssociatedWithAppointment(id, userId, role))
            {
                _logger.LogError($"Appoitnment id {id} cannot be retrieved for uid {userId} with role {role}");
                return NotFound();
            }
            else
            {
                var appointment = _appointmentService.GetById(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found", id);
                    return NotFound();
                }
                // Return the appointment
                return Ok(appointment);

            }
        }


        // Create a new appointment
        // POST: api/appointment
        [Authorize(Roles = "Medic,Patient")]
        [HttpPost]
        public IActionResult CreateAppointment(AppointmentCreationDTO appointment)
        {

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("Creating a new appointment: {Appointment}", appointment);
            try
            {
                // Check that the user is either the medic or the patient associated with the appointment
                // AKA, the patient and the medic are linked togheter
                bool isAuth;
                switch ( role)
                {
                    case "Medic":
                        isAuth = _authService.IsPatientMedicLinked(appointment.PatientId, userId);
                        break;


                    case "Patient":
                        isAuth = _authService.IsPatientMedicLinked(userId, appointment.MedicId);
                        break;

                    default: isAuth = false; 
                        // TODO --> Throw a custom exception for this case
                        break;
                }

                if (!isAuth)
                {
                    // Permission denied
                    _logger.LogError($"User with Id {userId} and role {role} failed to create an appointment.");
                    return BadRequest();
                }
                else
                {
                    // We need to make sure the POST body params are the correct ones
                    // AKA make sure the userID is equal to apppointment.MedicId or apppointment.PatientID depending on the role of the user
                    var result = _appointmentService.Add(appointment, userId, role);

                    if (result == null)
                    {
                        _logger.LogWarning("Failed to create appointment: {Appointment}", appointment);
                        return StatusCode(500);
                    }


                    _logger.LogInformation("Appointment created successfully: {Appointment}", result);
                    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating appointment: {Appointment}", appointment);
                return StatusCode(500, "An error occurred while creating the appointment.");
            }
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

            if(!_authService.IsAssociatedWithAppointment(id, userId, role))
            {
                // Permission denied
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
