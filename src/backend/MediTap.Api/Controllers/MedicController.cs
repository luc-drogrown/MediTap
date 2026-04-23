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
    public class MedicController : ControllerBase
    {
        private readonly ILogger<MedicController> _logger;
        private readonly IMedicService _medicService;
        public MedicController(ILogger<MedicController> logger, IMedicService medicService)
        {
            _logger = logger;
            _medicService = medicService;
        }

        // Get the profile of the logged-in medic
        // GET: api/medic/me
        [Authorize(Roles = "Medic")]
        [HttpGet("me")]
        public ActionResult<Medic> GetMyProfile()
        {
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            _logger.LogInformation("Getting medic profile for ID: {Id}", loggedInUserId);

            var medic = _medicService.GetById(loggedInUserId);
            if (medic == null)
            {
                _logger.LogWarning("Medic with ID {Id} not found", loggedInUserId);
                return NotFound();
            }
            _logger.LogInformation("Medic with ID {Id} retrieved successfully", loggedInUserId);
            return Ok(medic);

        }


        // ---------------------------------------------------------------------//
        // These endpoints have information that can
        // be optained by the medic's profile

        // Get all patient of a medic
        // GET: api/medic/5/patient
        //[Authorize(Roles = "Medic")]
        //[HttpGet("{id}/patient")]
        //public ActionResult<IEnumerable<Patient>> GetPatients(int id)
        //{
        //    _logger.LogInformation("Getting patients for medic ID: {Id}", id);
        //    var medic = MedicService.GetById(id);
        //    if (medic == null)
        //    {
        //        _logger.LogWarning("Medic with ID {Id} not found", id);
        //        return NotFound();
        //    }
        //    var patients = MedicService.GetPatients(id);
        //    _logger.LogInformation("Patients for medic ID {Id} retrieved successfully", id);
        //    return Ok(patients);
        //}


        // Get all appointments of a medic
        // GET: api/medic/5/appointment
        //[Authorize(Roles = "Medic")]
        //[HttpGet("{id}/appointment")]
        //public ActionResult<IEnumerable<Appointment>> GetAppointments(int id)
        //{
        //    _logger.LogInformation("Getting appointments for medic ID: {Id}", id);
        //    var medic = MedicService.GetById(id);
        //    if (medic == null)
        //    {
        //        _logger.LogWarning("Medic with ID {Id} not found", id);
        //        return NotFound();
        //    }
        //    var appointments = MedicService.GetAppointments(id);
        //    _logger.LogInformation("Appointments for medic ID {Id} retrieved successfully", id);
        //    return Ok(appointments);
        //}

    }
}
