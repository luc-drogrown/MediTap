using MediTap.Api.Models;
using MediTap.Api.DTO;
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
        private readonly IMedicService _medicService;
        public MedicationController(ILogger<MedicationController> logger, IMedicationService medicationService, IMedicService medicService)
        {
            _logger = logger;
            _medicationService = medicationService;
            _medicService = medicService;
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

            // Checks that user has permission to view the Medication (aka is linked with the Patient)
            var medication = _medicationService.GetById(id, userId);
            if (medication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found for user ID {UserId}.", id, userId);
                return NotFound();
            }

            _logger.LogInformation("Medication with ID {Id} retrieved successfully", id);
            return Ok(medication);

        }

        // Create a new medication
        // POST: api/medication
        [Authorize(Roles = "Medic")]
        [HttpPost]
        public ActionResult<MedicationDTO> CreateMedication(MedicationCreationDTO medication)
        {
            _logger.LogInformation("Creating a new medication");
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Check if medication.PatientId is linked with the current medic
                if (!_medicService.AuthCheck(medication.PatientId, userId))
                {
                    _logger.LogWarning($"Medic with id {userId} does not have permission to add medication to Patient {medication.PatientId}");
                    return NotFound();
                }

                // The Add method should return the created medication with its new ID, or null if creation failed
                var result = _medicationService.Add(medication, userId);
                if (result == null)
                {
                    _logger.LogWarning("Failed to create medication for user ID {UserId} with role {Role}", userId);
                    return BadRequest("Failed to create medication.");
                }
                _logger.LogInformation("Medication created successfully with ID: {Id}", result.Id);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating medication");
                return StatusCode(500, "An error occurred while creating the medication.");
            }
        }


        // Update a medication
        // PUT: api/medication/5
        [Authorize(Roles = "Medic")]
        [HttpPut("{id}")]
        public IActionResult UpdateMedication(int id, MedicationUpdateDTO medication)
        {
            _logger.LogInformation("Updating medication with ID: {Id}, Medication: {Medication}", id, medication);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Checks that Medic is linked to the patient in the Medication
            var isAuth = _medicationService.GetAuth(id, userId);
            if (!isAuth)
            {
                _logger.LogWarning("Medication with ID {Id} not found for user ID {UserId}", id, userId);
                return NotFound();
            }

            try
            {
                // Checks that Medication is prescribed for the right Patient
                _medicationService.Update(medication, id);
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
            // We need to check that
            // Medic with userId is associated with Patient from the medication
            var isAuth = _medicationService.GetAuth(id, userId);
            if (!isAuth)
            {
                _logger.LogWarning("Medication with ID {Id} not found for user ID {UserId}.", id, userId);
                return NotFound();
            }
            try
            {
                _medicationService.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting medication with ID: {Id} for user ID {UserId}.", id, userId);
                return StatusCode(500, "An error occurred while deleting the medication.");
            }
            _logger.LogInformation("Medication with ID {Id} deleted successfully for user ID {UserId}.", id, userId);
            return NoContent();
        }


    }
}