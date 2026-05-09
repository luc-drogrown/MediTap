using MediTap.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediTap.Api.Services.Interfaces;
namespace MediTap.Api.Controllers
{
    // DONE
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationController : ControllerBase
    {

        private readonly ILogger<MedicationController> _logger;
        private readonly IMedicationService _medicationService;
        public MedicationController(ILogger<MedicationController> logger, IMedicationService medicationService)
        {
            _logger = logger;
            _medicationService = medicationService;
        }




        // API endpoints for Medication

        // Get medication by Id
        // GET: api/medication/5
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}")]
        public ActionResult<Medication> GetById(int id)
        {
            _logger.LogInformation("Getting medication by ID: {Id}", id);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var medication = _medicationService.GetById(id, userId, role);
            if (medication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found for user ID {UserId} with role {Role}", id, userId, role);
                return NotFound();
            }

            _logger.LogInformation("Medication with ID {Id} retrieved successfully", id);
            return Ok(medication);

        }

        // Create a new medication
        // POST: api/medication
        [Authorize(Roles = "Medic")]
        [HttpPost]
        public ActionResult<Medication> CreateMedication(Medication medication)
        {
            _logger.LogInformation("Creating a new medication");
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                // The Add method should return the created medication with its new ID, or null if creation failed
                var result = _medicationService.Add(medication, userId, role);
                if (result == null)
                {
                    _logger.LogWarning("Failed to create medication for user ID {UserId} with role {Role}", userId, role);
                    return BadRequest("Failed to create medication.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating medication");
                return StatusCode(500, "An error occurred while creating the medication.");
            }
            _logger.LogInformation("Medication created successfully with ID: {Id}", medication.Id);
            return CreatedAtAction(nameof(GetById), new { id = medication.Id }, medication);
        }


        // Update a medication
        // PUT: api/medication/5
        [Authorize(Roles = "Medic")]
        [HttpPut("{id}")]
        public IActionResult UpdateMedication(int id, Medication medication)
        {
            _logger.LogInformation("Updating medication with ID: {Id}, Medication: {Medication}", id, medication);
            if (id != medication.Id)
            {
                _logger.LogWarning("ID in URL does not match ID in body. URL ID: {UrlId}, Body ID: {BodyId}", id, medication.Id);
                return BadRequest("ID in URL does not match ID in body");
            }
            // Check if the medication exists and belongs to the user
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var existingMedication = _medicationService.GetById(id, userId, role);
            if (existingMedication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found for user ID {UserId} with role {Role}", id, userId, role);
                return NotFound();
            }

            try
            {
                _medicationService.Update(medication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating medication with ID: {Id}, Medication: {Medication}", id, medication);
                return StatusCode(500, "An error occurred while updating the medication.");
            }
            _logger.LogInformation("Medication with ID {Id} updated successfully", id);
            return NoContent();
        }


        // Delete a medication
        // DELETE: api/medication/5
        [Authorize(Roles = "Medic")]
        [HttpDelete("{id}")]
        public IActionResult DeleteMedication(int id)
        {
            _logger.LogInformation("Deleting medication with ID: {Id}", id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var medication = _medicationService.GetById(id, userId, role);
            if (medication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found for user ID {UserId} with role {Role}", id, userId, role);
                return NotFound();
            }
            try
            {
                _medicationService.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting medication with ID: {Id} for user ID {UserId} with role {Role}", id, userId, role);
                return StatusCode(500, "An error occurred while deleting the medication.");
            }
            _logger.LogInformation("Medication with ID {Id} deleted successfully for user ID {UserId} with role {Role}", id, userId, role);
            return NoContent();
        }


    }
}
