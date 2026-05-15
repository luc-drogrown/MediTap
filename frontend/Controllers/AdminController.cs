using Microsoft.AspNetCore.Mvc;
using MediTap.Front.Models.Admin;

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

            var response = await _httpClient.PostAsJsonAsync("api/Patient/prepare-registration", new
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

            return RedirectToAction("WritePatientCard", new
            {
                uname = createdPatient.Uname,
                pendingRegistrationId = createdPatient.PendingRegistrationId
            });
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

        public IActionResult WritePatientCard(string uname, string pendingRegistrationId)
        {
            TempData["PatientUname"] = uname;
            TempData["PendingRegistrationId"] = pendingRegistrationId;

            return View();
        }

        public async Task<IActionResult> ViewPatients()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Medic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Patient/admin/all");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load patients.";
                return View(new List<AdminPatientViewModel>());
            }

            var patients = await response.Content.ReadFromJsonAsync<List<AdminPatientViewModel>>();

            return View(patients ?? new List<AdminPatientViewModel>());
        }

        public async Task<IActionResult> ViewMedics()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Medic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Medic/admin/all");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load medics.";
                return View(new List<AdminMedicViewModel>());
            }

            var medics = await response.Content.ReadFromJsonAsync<List<AdminMedicViewModel>>();

            return View(medics ?? new List<AdminMedicViewModel>());
        }

        public async Task<IActionResult> ManageAccounts()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Medic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var pageModel = new ManageAccountsPageViewModel();

            var resetRequestsResponse = await _httpClient.GetAsync("api/password-reset/admin/pending");

            if (resetRequestsResponse.IsSuccessStatusCode)
            {
                var resetRequests = await resetRequestsResponse.Content
                    .ReadFromJsonAsync<List<PasswordResetRequestViewModel>>();

                if (resetRequests != null)
                {
                    pageModel.PasswordResetRequests = resetRequests;
                }
            }

            var patientsResponse = await _httpClient.GetAsync("api/Patient/admin/all");

            if (patientsResponse.IsSuccessStatusCode)
            {
                var patients = await patientsResponse.Content.ReadFromJsonAsync<List<AdminPatientViewModel>>();

                if (patients != null)
                {
                    pageModel.Accounts.AddRange(patients.Select(p => new ManageAccountViewModel
                    {
                        Id = p.Id,
                        FullName = $"{p.FirstName} {p.LastName}",
                        Email = p.Email,
                        PhoneNumber = p.PhoneNumber,
                        Role = "Patient",
                        Status = p.Status
                    }));
                }
            }

            var medicsResponse = await _httpClient.GetAsync("api/Medic/admin/all");

            if (medicsResponse.IsSuccessStatusCode)
            {
                var medics = await medicsResponse.Content.ReadFromJsonAsync<List<AdminMedicViewModel>>();

                if (medics != null)
                {
                    pageModel.Accounts.AddRange(medics.Select(m => new ManageAccountViewModel
                    {
                        Id = m.Id,
                        FullName = $"{m.FirstName} {m.LastName}",
                        Email = m.Email,
                        PhoneNumber = m.PhoneNumber,
                        Role = "Medic",
                        Status = m.Status
                    }));
                }
            }

            pageModel.Accounts = pageModel.Accounts
                .OrderBy(a => a.Role)
                .ThenBy(a => a.FullName)
                .ToList();

            return View(pageModel);
        }

        [HttpPost]
        public async Task<IActionResult> CompletePasswordReset(int requestId, string newPassword, string confirmPassword)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Medic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync(
                $"api/password-reset/admin/complete/{requestId}",
                new
                {
                    newPassword = newPassword,
                    confirmPassword = confirmPassword
                });

            if (!response.IsSuccessStatusCode)
            {
                TempData["ManageAccountsError"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("ManageAccounts");
            }

            TempData["ManageAccountsSuccess"] = "Password reset completed successfully.";
            return RedirectToAction("ManageAccounts");
        }

        [HttpPost]
        public async Task<IActionResult> RejectPasswordReset(int requestId)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Medic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(
                $"api/password-reset/admin/reject/{requestId}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ManageAccountsError"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("ManageAccounts");
            }

            TempData["ManageAccountsSuccess"] = "Password reset request rejected.";
            return RedirectToAction("ManageAccounts");
        }

        [HttpPost]
        public async Task<IActionResult> DisableAccount(int accountId, string role)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Medic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var endpoint = role == "Patient"
                ? $"api/Patient/admin/{accountId}/disable"
                : $"api/Medic/admin/{accountId}/disable";

            var response = await _httpClient.PutAsync(endpoint, null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ManageAccountsError"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("ManageAccounts");
            }

            TempData["ManageAccountsSuccess"] = $"{role} account disabled successfully.";
            return RedirectToAction("ManageAccounts");
        }

        [HttpPost]
        public async Task<IActionResult> EnableAccount(int accountId, string role)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Medic");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var endpoint = role == "Patient"
                ? $"api/Patient/admin/{accountId}/enable"
                : $"api/Medic/admin/{accountId}/enable";

            var response = await _httpClient.PutAsync(endpoint, null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ManageAccountsError"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("ManageAccounts");
            }

            TempData["ManageAccountsSuccess"] = $"{role} account enabled successfully.";
            return RedirectToAction("ManageAccounts");
        }

        public class PatientRegisterResponse
        {
            public string PendingRegistrationId { get; set; } = string.Empty;

            public string Uname { get; set; } = string.Empty;
        }
    }
}
