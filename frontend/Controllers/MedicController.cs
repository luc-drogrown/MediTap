using Microsoft.AspNetCore.Mvc;

namespace MediTap.Front.Controllers
{
    public class MedicController : Controller
    {
        private readonly HttpClient _httpClient;

        public MedicController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> LoginSubmit(string uname, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new
            {
                uname = uname,
                password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid username or password";
                return View("Login");
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result == null || result.Role != "Medic")
            {
                ViewBag.Error = "This account is not a medic account";
                return View("Login");
            }

            HttpContext.Session.SetString("JwtToken", result.Token);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetInt32("UserId", result.Id);

            if (result.Id == 1)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

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
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}