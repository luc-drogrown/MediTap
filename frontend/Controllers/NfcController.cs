using Microsoft.AspNetCore.Mvc;

namespace MediTapFRONT.Controllers
{
    public class NfcController : Controller
    {
        private static bool isScanPageActive = false;
        private static string? latestPatientId = null;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Scan()
        {

            if (!isScanPageActive)
            {
                return BadRequest("Scan page is not active.");
            }

            using var reader = new StreamReader(Request.Body);
            var patientId = await reader.ReadToEndAsync();

            latestPatientId = patientId;

            return Content("Received: " + patientId);
        }

        [HttpPost]
        public IActionResult StartScan()
        {
            isScanPageActive = true;
            latestPatientId = null;

            return Ok();
        }

        [HttpPost]
        public IActionResult StopScan()
        {
            isScanPageActive = false;
            latestPatientId = null;

            return Ok();
        }

        [HttpGet]
        public IActionResult GetLatestScan()
        {
            if (string.IsNullOrEmpty(latestPatientId))
            {
                return NoContent();
            }

            return Ok(new { patientId = latestPatientId });
        }
    }
}
