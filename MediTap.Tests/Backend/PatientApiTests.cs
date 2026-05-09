using MediTap.Api;
using MediTap.Api.Models;
using MediTap.Tests.Frontend;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MediTap.Tests.Backend
{
    [TestFixture]
    public class PatientApiTests
    {
        private IConfiguration _config;
        private HttpClient _client;
        private string _baseUrl = "https://localhost:7116";

        [OneTimeSetUp]
        public void ClassInit()
        {
            _config = new ConfigurationBuilder()
                .AddUserSecrets<PatientApiTests>()
                .Build();
        }
        [SetUp]
        public void Setup()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }

        private async Task<string> GetTokenAsync()
        {
            string adminPassword = _config["TestSettings:AdminPassword"];
            string adminUsername = _config["TestSettings:AdminUsername"];
            var loginData = new { Uname = adminUsername, Password = adminPassword };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginData);

            if(!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                NUnit.Framework.Assert.Fail($"Login failed with {response.StatusCode}. Body: {errorBody}");
            }

            var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            return result.GetProperty("token").GetString();
        }

        private string GenerateUniqueValidCNP()
        {
            var random = new Random();

            // 1 = Male born in 1900s
            // 870918 = Sept 18, 1987 (Matches your DateOfBirth field)
            string prefix = "1870918";

            // Random County (01-52) and Random Sequential Number (001-999)
            int county = random.Next(1, 53);
            int sequential = random.Next(1, 1000);

            string baseCnp = $"{prefix}{county:D2}{sequential:D3}";

            // The official Romanian CNP Checksum string
            string multiplier = "279146358279";
            int sum = 0;

            // Multiply each digit by its corresponding multiplier digit
            for (int i = 0; i < 12; i++)
            {
                sum += (baseCnp[i] - '0') * (multiplier[i] - '0');
            }

            // Calculate the final 13th digit
            int remainder = sum % 11;
            int controlDigit = remainder == 10 ? 1 : remainder;

            return baseCnp + controlDigit;
        }

        [Test]
        public async Task RegisterPatient_ValidData_ReturnsSuccess()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var uniqueEmail = $"test_{Guid.NewGuid()}@example.com"; 
            var newPatient = new
            {
                FirstName = "Test",
                LastName = "User",
                Email = uniqueEmail,
                CNP = GenerateUniqueValidCNP(), 
                DateOfBirth = "1987-09-18",
                Password = "TestPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/Patient", newPatient);
            var responseBody = await response.Content.ReadAsStringAsync();

            NUnit.Framework.Assert.That(
                response.StatusCode, 
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.Created), 
                $"Happy Path Failed! API returned {response.StatusCode}. Server said: {responseBody}"
                );
        }

        [Test]
        public async Task GetPatientMe_NoToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/api/Patient/me");

            NUnit.Framework.Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task RegisterPatient_InvalidEmail_ReturnsBadRequest()
        {
            // 1. Arrange: Get Token
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Arrange: Build Patient Data
            // Everything is perfectly valid EXCEPT the email.
            var invalidPatient = new
            {
                FirstName = "Test",
                LastName = "User",
                Email = "this-is-not-an-email", // <--- The intentional error!
                CNP = GenerateUniqueValidCNP(),
                DateOfBirth = "1987-09-18",
                Password = "TestPassword123!"
            };

            // 3. Act: Send to the CORRECT route you found
            var response = await _client.PostAsJsonAsync("/api/Patient", invalidPatient);

            // 4. READ THE BODY: To catch any hidden errors
            var responseBody = await response.Content.ReadAsStringAsync();

            // 5. Assert
            NUnit.Framework.Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest),
                $"Invalid Email Test Failed! API returned {response.StatusCode}. Server said: {responseBody}"
            );
        }

        private async Task<string> GetPatientTokenAsync()
        {
            // Fetch an existing patient from your Secrets or hardcode a test one
            string pUsername = _config["TestSettings:PatientUsername"] ?? "P-Timotei-Medi-91b1d0fe";
            string pPassword = _config["TestSettings:PatientPassword"] ?? "Odobesti16E";

            var loginData = new { Uname = pUsername, Password = pPassword };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                NUnit.Framework.Assert.Fail($"Could not log in as Patient. Status: {response.StatusCode}. Body: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();

            return result.GetProperty("token").GetString();
        }

        [Test]
        public async Task AccessAdminPanel_AsPatient_ReturnsForbidden()
        {
            // 1. Arrange: Get the Patient Token
            string patientToken = await GetPatientTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", patientToken);

            // 2. Act: Try to sneak into the Medic-only Create Patient endpoint
            // We send an empty object because the [Authorize] attribute blocks it before validating the body
            var response = await _client.PostAsJsonAsync("/api/Patient", new { });

            // 3. Assert: Verify we get kicked out
            NUnit.Framework.Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.Forbidden),
                $"Access check failed! API returned {response.StatusCode} instead of 403 Forbidden."
            );
        }

        [Test]
        public async Task RegisterPatient_DuplicateUser_ReturnsBadRequestOrConflict()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newPatient = new
            {
                FirstName = "Duplicate",
                LastName = "Test",
                Email = $"duplicate_{Guid.NewGuid()}@example.com",
                CNP = GenerateUniqueValidCNP(),
                DateOfBirth = "1987-09-18",
                Password = "TestPassword123!"
            };

            // First request: Should succeed
            await _client.PostAsJsonAsync("/api/Patient", newPatient);

            // Second request: Exact same data, should fail gracefully
            var response = await _client.PostAsJsonAsync("/api/Patient", newPatient);
            var body = await response.Content.ReadAsStringAsync();

            NUnit.Framework.Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest).Or.EqualTo(HttpStatusCode.Conflict),
                $"Duplicate test failed! API returned {response.StatusCode} instead of 400 or 409. Body: {body}"
            );
        }

        [Test]
        public async Task AccessProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            // Explicitly ensure no token is attached
            _client.DefaultRequestHeaders.Authorization = null;

            // Try to hit the protected Create Patient endpoint
            var response = await _client.PostAsJsonAsync("/api/Patient", new { });

            NUnit.Framework.Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Missing token test failed! API returned {response.StatusCode} instead of 401 Unauthorized."
            );
        }

        [Test]
        public async Task RegisterPatient_InvalidCNPChecksum_ReturnsBadRequest()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var invalidCnpPatient = new
            {
                FirstName = "Bad",
                LastName = "Checksum",
                Email = $"badcnp_{Guid.NewGuid()}@example.com",
                CNP = "1870918123459", // 9 at the end is mathematically invalid for this sequence
                DateOfBirth = "1987-09-18",
                Password = "TestPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/Patient", invalidCnpPatient);
            var body = await response.Content.ReadAsStringAsync();

            NUnit.Framework.Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest),
                $"Invalid CNP test failed! API returned {response.StatusCode} instead of 400. Body: {body}"
            );
        }

        [Test]
        public async Task RegisterPatient_InvalidDateOfBirth_ReturnsBadRequest()
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var badDatePatient = new
            {
                FirstName = "Bad",
                LastName = "Date",
                Email = $"baddate_{Guid.NewGuid()}@example.com",
                CNP = GenerateUniqueValidCNP(),
                DateOfBirth = "2024-13-45", // Month 13, Day 45 does not exist
                Password = "TestPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/Patient", badDatePatient);
            var body = await response.Content.ReadAsStringAsync();

            NUnit.Framework.Assert.That(
                response.StatusCode,
                Is.EqualTo(HttpStatusCode.BadRequest),
                $"Bad Date test failed! API returned {response.StatusCode} instead of 400. Body: {body}"
            );
        }
    } 
}