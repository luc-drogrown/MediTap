using Microsoft.AspNetCore.Mvc;

namespace MediTap.Front.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MediTapApi");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPatientSubmit(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string cnp,
            string dateOfBirth,
            string address,
            string password)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "JWT token is missing from session.";
                return View("RegisterPatient");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("api/Patient", new
            {
                firstName = firstName,
                lastName = lastName,
                email = email,
                phoneNumber = phoneNumber,
                cnp = cnp,
                dateOfBirth = dateOfBirth,
                address = address,
                password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await GetApiErrorMessage(response, "Failed to register patient.");
                return View("RegisterPatient");
            }

            var createdPatient = await response.Content.ReadFromJsonAsync<PatientRegisterResponse>();

            if (createdPatient == null || string.IsNullOrEmpty(createdPatient.Uname))
            {
                ViewBag.Error = "Patient was created, but username could not be retrieved.";
                return View("RegisterPatient");
            }

            return RedirectToAction("WritePatientCard", new { uname = createdPatient.Uname });
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMedicSubmit(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string specialty,
            string password,
            string medicStatus = "Active")
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "JWT token is missing from session.";
                return View("RegisterMedic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("api/Medic", new
            {
                firstName = firstName,
                lastName = lastName,
                email = email,
                phoneNumber = phoneNumber,
                specialty = specialty,
                password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = await GetApiErrorMessage(response, "Failed to register medic.");
                return View("RegisterMedic");
            }

            TempData["Success"] = "Medic registered successfully!";
            return RedirectToAction("RegisterMedic");
        }

        private async Task<string> GetApiErrorMessage(HttpResponseMessage response, string fallbackMessage)
        {
            var apiMessage = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(apiMessage))
            {
                return apiMessage;
            }

            return fallbackMessage;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult RegisterPatient()
        {
            return View();
        }

        public IActionResult RegisterMedic()
        {
            return View();
        }

        public IActionResult WritePatientCard(string uname)
        {
            TempData["PatientUname"] = uname;
            return View();
        }

        public class PatientRegisterResponse
        {
            public string Uname { get; set; }
        }
    }
}
