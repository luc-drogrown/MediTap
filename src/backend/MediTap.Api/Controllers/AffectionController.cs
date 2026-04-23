using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;

namespace MediTap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AffectionController : ControllerBase
    {
        private readonly ILogger<AffectionController> _logger;
        public AffectionController(ILogger<AffectionController> logger)
        {
            _logger = logger;
        }


        // Get affection by Id
        // GET: api/affection/5
        [HttpGet("{id}")]
        public ActionResult<Affection> GetById(int id)
        {
            _logger.LogInformation("Getting affection by ID: {Id}", id);
            var affection = AffectionService.GetById(id);
            if (affection == null)
            {
                _logger.LogWarning("Affection with ID {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Affection with ID {Id} retrieved successfully", id);
            return Ok(affection);
        }


        // Create a new affection
        // POST: api/affection
        [HttpPost]
        public ActionResult<Affection> CreateAffection(Affection affection)
        {
            _logger.LogInformation("Creating a new affection");
            try
            {
                AffectionService.Add(affection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating affection");
                return StatusCode(500, "An error occurred while creating the affection.");
            }

            _logger.LogInformation("Affection created successfully with ID: {Id}", affection.Id);
            return CreatedAtAction(nameof(GetById), new { id = affection.Id }, affection);
        }


        // Delete an affection
        // DELETE: api/affection/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAffection(int id)
        {
            _logger.LogInformation("Deleting affection with ID: {Id}", id);
            var affection = AffectionService.GetById(id);
            if (affection == null)
            {
                _logger.LogWarning("Affection with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                AffectionService.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting affection with ID: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the affection.");
            }
            _logger.LogInformation("Affection with ID {Id} deleted successfully", id);
            return NoContent();


        }
    }
}
