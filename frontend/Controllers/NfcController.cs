using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace MediTap.Front.Controllers
{
    public class NfcController : Controller
    {
        private readonly HttpClient _httpClient;

        private static bool isScanPageActive = false;
        private static string? latestPatientId = null;

        private static bool isWritePageActive = false;
        private static string? pendingPatientUname = null;
        private static string? pendingRegistrationId = null;
        private static string? pendingAdminJwtToken = null;
        private static bool cardWriteCompleted = false;
        private static string? cardWriteError = null;

        private static bool cardWriteCancelled = false;

        public NfcController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MediTapApi");
        }
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
        public IActionResult StartWrite(string uname, string pendingId)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Admin session token is missing.");
            }

            if (string.IsNullOrWhiteSpace(uname) || string.IsNullOrWhiteSpace(pendingId))
            {
                return BadRequest("Patient username or pending registration ID is missing.");
            }

            isWritePageActive = true;
            pendingPatientUname = uname;
            pendingRegistrationId = pendingId;
            pendingAdminJwtToken = token;
            cardWriteCompleted = false;
            cardWriteError = null;
            cardWriteCancelled = false;

            return Ok(new
            {
                message = "Write mode activated",
                patientUname = pendingPatientUname
            });
        }

        [HttpGet]
        public IActionResult CanWrite()
        {
            return Ok(new
            {
                canWrite = isWritePageActive
                           && !cardWriteCancelled
                           && !string.IsNullOrWhiteSpace(pendingPatientUname)
                           && !string.IsNullOrWhiteSpace(pendingRegistrationId)
            });
        }

        //Android app call: is there a patient username waiting to be written?
        [HttpGet]
        public IActionResult GetPendingWrite()
        {
            if (cardWriteCancelled)
            {
                return Ok(new
                {
                    cancelled = true
                });
            }

            if (!isWritePageActive || string.IsNullOrEmpty(pendingPatientUname))
            {
                return NoContent();
            }

            return Ok(new
            {
                patientUname = pendingPatientUname,
                cancelled = false
            });
        }

        //Android app write success
        [HttpPost]
        public async Task<IActionResult> ConfirmWrite()
        {
            if (string.IsNullOrWhiteSpace(pendingRegistrationId))
            {
                cardWriteError = "Pending registration ID is missing.";
                return BadRequest(cardWriteError);
            }

            if (string.IsNullOrWhiteSpace(pendingAdminJwtToken))
            {
                cardWriteError = "Admin authorization token is missing.";
                return Unauthorized(cardWriteError);
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", pendingAdminJwtToken);

            var response = await _httpClient.PostAsync(
                $"api/Patient/confirm-registration/{pendingRegistrationId}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                cardWriteError = await response.Content.ReadAsStringAsync();
                cardWriteCompleted = false;
                isWritePageActive = false;

                return StatusCode((int)response.StatusCode, cardWriteError);
            }

            cardWriteCompleted = true;
            isWritePageActive = false;
            cardWriteError = null;

            pendingPatientUname = null;
            pendingRegistrationId = null;
            pendingAdminJwtToken = null;

            return Ok(new
            {
                message = "Card writing confirmed and patient registered"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CancelWrite()
        {
            if (string.IsNullOrWhiteSpace(pendingRegistrationId))
            {
                isWritePageActive = false;
                pendingPatientUname = null;
                pendingAdminJwtToken = null;
                cardWriteCompleted = false;
                cardWriteError = null;

                return BadRequest("Pending registration ID is missing.");
            }

            if (string.IsNullOrWhiteSpace(pendingAdminJwtToken))
            {
                isWritePageActive = false;
                pendingPatientUname = null;
                pendingRegistrationId = null;
                cardWriteCompleted = false;
                cardWriteError = null;

                return Unauthorized("Admin authorization token is missing.");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", pendingAdminJwtToken);

            var response = await _httpClient.PostAsync(
                $"api/Patient/cancel-registration/{pendingRegistrationId}",
                null);

            isWritePageActive = false;
            pendingPatientUname = null;
            pendingRegistrationId = null;
            pendingAdminJwtToken = null;
            cardWriteCompleted = false;
            cardWriteError = null;
            cardWriteCancelled = true;

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, error);
            }

            return Ok(new
            {
                message = "Card writing cancelled."
            });
        }

        //Let the page know when Android finished writing
        [HttpGet]
        public IActionResult GetWriteStatus()
        {
            return Ok(new
            {
                completed = cardWriteCompleted,
                error = cardWriteError
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
