using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
namespace MediTap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationController : ControllerBase
    {

        private readonly ILogger<MedicationController> _logger;
        public MedicationController(ILogger<MedicationController> logger)
        {
            _logger = logger;
        }


        // Get medication by Id
        // GET: api/medication/5
        [HttpGet("{id}")]
        public ActionResult<Medication> GetById(int id)
        {
            _logger.LogInformation("Getting medication by ID: {Id}", id);
            var medication = MedicationService.GetById(id);
            if (medication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Medication with ID {Id} retrieved successfully", id);
            return Ok(medication);

        }

        // Create a new medication
        // POST: api/medication
        [HttpPost]
        public ActionResult<Medication> CreateMedication(Medication medication)
        {
            _logger.LogInformation("Creating a new medication");
            try
            {
                MedicationService.Add(medication);
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
        [HttpPut("{id}")]
        public IActionResult UpdateMedication(int id, Medication medication)
        {
            _logger.LogInformation("Updating medication with ID: {Id}, Medication: {Medication}", id, medication);
            if (id != medication.Id)
            {
                _logger.LogWarning("ID in URL does not match ID in body. URL ID: {UrlId}, Body ID: {BodyId}", id, medication.Id);
                return BadRequest("ID in URL does not match ID in body");
            }
            var existingMedication = MedicationService.GetById(id);
            if (existingMedication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                MedicationService.Update(medication);
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
        [HttpDelete("{id}")]
        public IActionResult DeleteMedication(int id)
        {
            _logger.LogInformation("Deleting medication with ID: {Id}", id);
            var medication = MedicationService.GetById(id);
            if (medication == null)
            {
                _logger.LogWarning("Medication with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                MedicationService.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting medication with ID: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the medication.");
            }
            _logger.LogInformation("Medication with ID {Id} deleted successfully", id);
            return NoContent();
        }


    }
}
