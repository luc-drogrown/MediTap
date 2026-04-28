using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
//using MediTap.Api.Models;

namespace MediTap.Tests.Backend
{
    /*
    [TestFixture]
    public class PatientApiTests
    {
        private WebApplicationFactory<Patient> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void Setup()
        {
            // FIX: Use <Patient> here instead of <Program>
            _factory = new WebApplicationFactory<Patient>();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task GetPatients_ShouldReturnSuccess()
        {
            // Act
            var response = await _client.GetAsync("/api/Patient/me");

            // Assert
            NUnit.Framework.Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }
    } */
}