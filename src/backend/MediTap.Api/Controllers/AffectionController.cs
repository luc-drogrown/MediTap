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
    public class AffectionController : ControllerBase
    {
        private readonly ILogger<AffectionController> _logger;
        private readonly IAffectionService _affectionService;
        public AffectionController(ILogger<AffectionController> logger, IAffectionService affectionService)
        {
            _logger = logger;
            _affectionService = affectionService;
        }


        // Get affection by Id
        // GET: api/affection/5
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}")]
        public ActionResult<Affection> GetById(int id)
        {
            _logger.LogInformation("Getting affection by ID: {Id}", id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var affection = _affectionService.GetById(id, userId, role);
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
        [Authorize(Roles = "Medic")]
        [HttpPost]
        public ActionResult<Affection> CreateAffection(Affection affection)
        {
            _logger.LogInformation("Creating a new affection");
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                _affectionService.Add(affection, userId, role);
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
        [Authorize(Roles = "Medic")]
        [HttpDelete("{id}")]
        public IActionResult DeleteAffection(int id)
        {
            _logger.LogInformation("Deleting affection with ID: {Id}", id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var affection = _affectionService.GetById(id, userId, role);
            if (affection == null)
            {
                _logger.LogWarning("Affection with ID {Id} not found", id);
                return NotFound();
            }
            try
            {
                _affectionService.Delete(id);
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
