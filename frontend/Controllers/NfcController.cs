using Microsoft.AspNetCore.Mvc;

namespace MediTap.Front.Controllers
{
    public class NfcController : Controller
    {
        private static bool isScanPageActive = false;
        private static string? latestPatientId = null;
        private static bool isWritePageActive = false;
        private static string? pendingPatientUname = null;
        private static bool cardWriteCompleted = false;
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

        //When patient registration succeeds
        [HttpGet]
        public IActionResult StartWrite(string uname)
        {
            isWritePageActive = true;
            pendingPatientUname = uname;
            cardWriteCompleted = false;

            return Ok(new
            {
                message = "Write mode activated",
                patientUname = pendingPatientUname
            });
        }

        //Android app call: is there a patient username waiting to be written?
        [HttpGet]
        public IActionResult GetPendingWrite()
        {
            if (!isWritePageActive || string.IsNullOrEmpty(pendingPatientUname))
            {
                return NoContent();
            }

            return Ok(new
            {
                patientUname = pendingPatientUname
            });
        }

        //Android app write success
        [HttpPost]
        public IActionResult ConfirmWrite()
        {
            cardWriteCompleted = true;
            isWritePageActive = false;

            return Ok(new
            {
                message = "Card writing confirmed"
            });
        }

        //Let the page know when Android finished writing
        [HttpGet]
        public IActionResult GetWriteStatus()
        {
            return Ok(new
            {
                completed = cardWriteCompleted
            });
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
