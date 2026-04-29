using Microsoft.AspNetCore.Mvc;

namespace MediTap.Front.Controllers
{
    public class PatientController : Controller
    {
        private readonly HttpClient _httpClient;

        public PatientController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MediTapApi");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginSubmit(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new
            {
                email = email,
                password = password,
                role = "Patient"
            });
            
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid username or password";
                return View("Login");
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            
            if (result == null || result.Role != "Patient")
            {
                ViewBag.Error = "This account is not a patient account";
                return View("Login");
            }

            HttpContext.Session.SetString("JwtToken", result.Token);
            HttpContext.Session.SetString("Role", result.Role);

            return RedirectToAction("Dashboard");
        }

        public IActionResult Scan()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public class LoginResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
        }
    }
}