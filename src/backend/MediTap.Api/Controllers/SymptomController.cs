using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediTap.Api.DTO;
using MediTap.Api.Services.Interfaces;
namespace MediTap.Api.Controllers
{
    // DONE
    [ApiController]
    [Route("api/[controller]")]
    public class SymptomController : ControllerBase
    {
        private readonly ILogger<SymptomController> _logger;
        private readonly ISymptomService _symptomService;
        public SymptomController(ILogger<SymptomController> logger, ISymptomService symptomService)
        {
            _logger = logger;
            _symptomService = symptomService;
        }

        // Show all symptoms with pagination
        // GET: api/symptom?pageNumber=1
        //[HttpGet]
        //public ActionResult<IEnumerable<Symptom>> GetAll(int? pageNumber)
        //{
        //    _logger.LogInformation("Getting all symptoms, page number: {PageNumber}", pageNumber);

        //    return PaginatedList<Symptom>.Create(SymptomService.GetAll(), pageNumber ?? 1, 10);
        //}


        // Show symptom by ID
        // GET: api/symptom/5
        //[HttpGet("{id}")]
        //public ActionResult<Symptom> GetById(int id)
        //{
        //    _logger.LogInformation("Getting symptom by ID: {Id}", id);
        //    var symptom = SymptomService.GetById(id);
        //    _logger.LogInformation("Symptom found: {Symptom}", symptom);
        //    if (symptom == null)
        //    {
        //        _logger.LogWarning("Symptom with ID {Id} not found", id);
        //        return NotFound();
        //    }
        //    return Ok(symptom);
        //}

        //----------------------------------------------------------------------//

        // PUT methods


        // Modify a symptom
        // PUT: api/symptom/5
        [Authorize(Roles = "Patient")]
        [HttpPut("{id}")]
        public IActionResult UpdateSymptom(int id, SymptomUpdateDTO updatedSymptom)
        {
            _logger.LogInformation("Updating Symptom with ID: {Id}, Symptom: {Symptom}", id, updatedSymptom);

            // Let the services check if the symptom exist and if the patient is the owner of the symptom
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var authCheck = _symptomService.GetByIdIfAuthorized(id, userId, role);
            if (!authCheck)
            {
                _logger.LogWarning("Symptom with ID {Id} not found", id);
                return NotFound();
            }

            // Try updating the symptom
            try
            {
                _symptomService.Update(updatedSymptom, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Symptom with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the symptom");
            }
            _logger.LogInformation("Symptom with ID {Id} updated successfully", id);
            return Ok(updatedSymptom);
        }


        //----------------------------------------------------------------------//

        // DELETE methods


        // Delete a symptom
        // DELETE: api/symptom/5
        [Authorize(Roles = "Patient")]
        [HttpDelete("{id}")]
        public IActionResult DeleteSymptom(int id)
        {
            // Let the services check if the symptom exist and if the patient is the owner of the symptom
            _logger.LogInformation("Deleting Symptom with ID: {Id}", id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var symptom = _symptomService.GetByIdIfAuthorized(id, userId, role);
            if (symptom == null)
            {
                _logger.LogWarning("Symptom with ID {Id} not found", id);
                return NotFound();
            }

            // Try deleting the symptom
            try
            {
                _symptomService.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Symptom with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the symptom");
            }
            _logger.LogInformation("Symptom with ID {Id} deleted successfully", id);
            return NoContent();
        }

    }
}
