using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Exceptions;
//using MediTap.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediTap.Api.Services.Interfaces;
using MediTap.Api.DTO;
namespace MediTap.Api.Controllers
{
    // DONE
    [ApiController]
    [Route("api/[controller]")]
    public class MedicController : ControllerBase
    {
        private readonly ILogger<MedicController> _logger;
        private readonly IMedicService _medicService;
        private readonly IPatientService _patientService;
        public MedicController(ILogger<MedicController> logger, IMedicService medicService, IPatientService patientService)
        {
            _logger = logger;
            _medicService = medicService;
            _patientService = patientService;
        }

        // Get the profile of the logged-in medic
        // GET: api/medic/me
        [Authorize(Roles = "Medic")]
        [HttpGet("me")]
        public ActionResult<MedicSummaryDTO> GetMyProfile()
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

        // Show all medics for Admin
        // Admin is the Medic with Id = 1
        // GET: api/medic/admin/all
        [Authorize(Roles = "Medic")]
        [HttpGet("admin/all")]
        public ActionResult<IEnumerable<MedicSummaryDTO>> GetAllMedicsForAdmin()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to view all medics by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                var medics = _medicService.GetAllForAdmin();

                _logger.LogInformation("All medics retrieved successfully by admin user ID: {UserId}", userId);

                return Ok(medics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all medics for admin user ID: {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving medics.");
            }
        }

        // Disable a medic account
        // Admin is the Medic with Id = 1
        // PUT: api/medic/admin/{medicId}/disable
        [Authorize(Roles = "Medic")]
        [HttpPut("admin/{medicId}/disable")]
        public IActionResult DisableMedicAccount(int medicId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to disable medic account by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                _medicService.DisableAccount(medicId);

                _logger.LogInformation("Medic account disabled successfully by admin user ID: {UserId}. Medic ID: {MedicId}", userId, medicId);

                return Ok(new
                {
                    message = "Medic account disabled successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Medic account could not be disabled. Medic ID: {MedicId}", medicId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling medic account. Medic ID: {MedicId}", medicId);
                return StatusCode(500, "An error occurred while disabling the medic account.");
            }
        }

        // Enable a medic account
        // Admin is the Medic with Id = 1
        // PUT: api/medic/admin/{medicId}/enable
        [Authorize(Roles = "Medic")]
        [HttpPut("admin/{medicId}/enable")]
        public IActionResult EnableMedicAccount(int medicId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to enable medic account by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                _medicService.EnableAccount(medicId);

                _logger.LogInformation("Medic account enabled successfully by admin user ID: {UserId}. Medic ID: {MedicId}", userId, medicId);

                return Ok(new
                {
                    message = "Medic account enabled successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Medic account could not be enabled. Medic ID: {MedicId}", medicId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling medic account. Medic ID: {MedicId}", medicId);
                return StatusCode(500, "An error occurred while enabling the medic account.");
            }
        }

        // Update a medic account
        // Admin is the Medic with Id = 1
        // PUT: api/medic/admin/{medicId}/update
        [Authorize(Roles = "Medic")]
        [HttpPut("admin/{medicId}/update")]
        public IActionResult UpdateMedicAccountForAdmin(int medicId, [FromBody] MedicAdminUpdateDTO request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to update medic account by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                _medicService.UpdateAccountForAdmin(medicId, request);

                _logger.LogInformation(
                    "Medic account updated successfully by admin user ID: {UserId}. Medic ID: {MedicId}",
                    userId,
                    medicId);

                return Ok(new
                {
                    message = "Medic account updated successfully."
                });
            }
            catch (InvalidEmailException ex)
            {
                _logger.LogWarning(ex, "Invalid email while updating medic account. Medic ID: {MedicId}", medicId);
                return BadRequest(ex.Message);
            }
            catch (InvalidPhoneNumberException ex)
            {
                _logger.LogWarning(ex, "Invalid phone number while updating medic account. Medic ID: {MedicId}", medicId);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Medic account could not be updated. Medic ID: {MedicId}", medicId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medic account. Medic ID: {MedicId}", medicId);
                return StatusCode(500, "An error occurred while updating the medic account.");
            }
        }

        // Create Medic profile
        // Can only be done by an admin (MedicId = 1)
        // POST: api/medic
        [Authorize(Roles = "Medic")]
        [HttpPost]
        public IActionResult CreateMedic([FromBody] MedicCreationDTO request)
        {
            _logger.LogInformation("Creating medic profile for username: {Uname}", request.FirstName);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Medic" && userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to create medic profile by user ID: {Id} with role: {Role}", userId, role);
                return NotFound();
            }


            try
            {
                var response = _medicService.Create(request);
                _logger.LogInformation("Medic profile created successfully for username: {Uname}", response.Uname);
                return Ok(response);
            }
            catch (InvalidPhoneNumberException ex)
            {
                _logger.LogError(ex, "Invalid phone number provided for medic profile creation for username: {Uname}", request.FirstName);
                return BadRequest(ex.Message);
            }
            catch (InvalidEmailException ex)
            {
                _logger.LogError(ex, "Invalid email provided for medic profile creation for username: {Uname}", request.FirstName);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medic profile for username: {Uname}", request.FirstName);
                return BadRequest(ex.Message);
            }
        }


        // Scan a Patient's card and link current MedicId to that PatientID
        // Returns a Patient Summary
        [Authorize(Roles = "Medic")]
        [HttpPost("me/scan")]
        public IActionResult Scan([FromBody] PatientScanDTO request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            try
            {
                var response = _medicService.Scan(request, userId, role);
                if(response == null)
                {
                    _logger.LogInformation($"");
                    return NotFound();
                }
                _logger.LogInformation($"");
                return Ok(response);
            }
            catch
            {
                _logger.LogError($"");
                throw;
            }

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
