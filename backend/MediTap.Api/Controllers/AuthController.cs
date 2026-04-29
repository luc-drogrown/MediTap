using Microsoft.AspNetCore.Mvc;
using MediTap.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static BCrypt.Net.BCrypt;
using MediTap.Api.DTO;

namespace MediTap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MediTapDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(MediTapDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            if (request.Role == "Patient")
            {
                _logger.LogInformation("Attempting login for patient with email: {Email}", request.Email);

                var patient = _context.Patients
                    .AsEnumerable()
                    .FirstOrDefault(p =>
                        p.Email != null &&
                        p.Email.EmailAddress == request.Email);

                if (patient == null)
                {
                    _logger.LogWarning("Login failed for patient with email: {Email} - User not found", request.Email);
                    return Unauthorized("Invalid email or password.");
                }

                if (!Verify(request.Password, patient.PasswordHash))
                {
                    _logger.LogWarning("Login failed for patient with email: {Email} - Incorrect password", request.Email);
                    return Unauthorized("Invalid email or password.");
                }

                var token = GenerateJwtToken(patient.Id, "Patient", patient.Uname);

                return Ok(new
                {
                    Token = token,
                    Role = "Patient",
                    Id = patient.Id
                });
            }

            else if (request.Role == "Medic")
            {
                _logger.LogInformation("Attempting login for medic with email: {Email}", request.Email);

                var medic = _context.Medics
                    .AsEnumerable()
                    .FirstOrDefault(m =>
                        m.Email != null &&
                        m.Email.EmailAddress == request.Email);

                if (medic == null)
                {
                    _logger.LogWarning("Login failed for medic with email: {Email} - User not found", request.Email);
                    return Unauthorized("Invalid email or password.");
                }

                if (!Verify(request.Password, medic.PasswordHash))
                {
                    _logger.LogWarning("Login failed for medic with email: {Email} - Incorrect password", request.Email);
                    return Unauthorized("Invalid email or password.");
                }

                var token = GenerateJwtToken(medic.Id, "Medic", medic.Uname);

                return Ok(new
                {
                    Token = token,
                    Role = "Medic",
                    Id = medic.Id
                });
            }

            return BadRequest("Invalid role.");
        }


        private string GenerateJwtToken(int userId, string role, string uname)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, uname),
            new Claim(ClaimTypes.Role, role) 
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
