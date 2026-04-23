using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
namespace MediTap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SymptomController : ControllerBase
    {
        private readonly ILogger<SymptomController> _logger;
        public SymptomController(ILogger<SymptomController> logger)
        {
            _logger = logger;
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
        [HttpPut("{id}")]
        public IActionResult UpdateSymptom(int id, Symptom symptom)
        {
            _logger.LogInformation("Updating Symptom with ID: {Id}, Symptom: {Symptom}", id, symptom);

            if (id != symptom.Id)
            {
                _logger.LogWarning("ID in URL does not match ID in body. URL ID: {UrlId}, Body ID: {BodyId}", id, symptom.Id);
                return BadRequest("ID in URL does not match ID in body");
            }

            var existingSymptom = SymptomService.GetById(id);
            if (existingSymptom == null)
            {
                _logger.LogWarning("Symptom with ID {Id} not found", id);
                return NotFound();
            }

            SymptomService.Update(symptom);

            _logger.LogInformation("Symptom with ID {Id} updated successfully", id);
            return Ok(existingSymptom);
        }


        //----------------------------------------------------------------------//

        // DELETE methods


        // Delete a symptom
        // DELETE: api/symptom/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSymptom(int id)
        {
            _logger.LogInformation("Deleting Symptom with ID: {Id}", id);
            var symptom = SymptomService.GetById(id);
            if (symptom == null)
            {
                _logger.LogWarning("Symptom with ID {Id} not found", id);
                return NotFound();
            }

            SymptomService.Delete(id);
            _logger.LogInformation("Symptom with ID {Id} deleted successfully", id);
            return NoContent();
        }

    }
}
