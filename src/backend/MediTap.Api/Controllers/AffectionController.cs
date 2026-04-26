using MediTap.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediTap.Api.Services.Interfaces;
using MediTap.Api.DTO;

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


        /// <summary>
        /// Retrieves the affection record with the specified identifier.
        /// </summary>
        /// <remarks>This method is accessible only to users in the "Medic" role. Returns a 404 Not Found
        /// response if the specified affection does not exist or is not accessible to the current user. 
        /// This endpoint returns only one specific Affection, 
        ///     as opposed to <see cref="PatientController.GetAffections(int)"></see> 
        ///     that returns all of them associated with a Patient
        ///     Affections can be seen by every Medic</remarks>
        /// <param name="id">The unique identifier of the affection to retrieve.</param>
        /// <returns>An <see cref="ActionResult{AffectionDTO}"/> containing the affection data if found; otherwise, a NotFound
        /// result.</returns>
        // Get affection by Id
        // GET: api/affection/5
        [Authorize(Roles = "Medic")]
        [HttpGet("{id}")]
        public ActionResult<AffectionDTO> GetById(int id)
        {
            _logger.LogInformation("Getting affection by ID: {Id}", id);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var affection = _affectionService.GetById(id, role);
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
