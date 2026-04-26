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
            bool isPatient = request.Uname.StartsWith("P");

            // Is a patient
            if (isPatient)
            {
                _logger.LogInformation("Attempting login for patient with username: {Username}", request.Uname);
                var patient = _context.Patients.FirstOrDefault(p => p.Uname == request.Uname);

                if (patient == null)
                {
                    _logger.LogWarning("Login failed for patient with username: {Username} - User not found", request.Uname);
                    return Unauthorized("Invalid username or password.");
                }
                else
                {
                    if(!Verify(request.Password, patient.PasswordHash))
                    {
                        _logger.LogWarning("Login failed for patient with username: {Username} - Incorrect password", request.Uname);
                        return Unauthorized("Invalid username or password.");
                    }

                    var token = GenerateJwtToken(patient.Id, "Patient", patient.Uname);

                    return Ok(new { Token = token , Role = "Patient" });
                }
            }

            // Is a doctor
            else
            {
                _logger.LogInformation("Attempting login for medic with username: {Username}", request.Uname);
                var medic = _context.Medics.FirstOrDefault(m => m.Uname == request.Uname);

                if (medic == null)
                {
                    _logger.LogWarning("Login failed for medic with username: {Username} - User not found", request.Uname);
                    return Unauthorized("Invalid username or password.");
                }
                else
                {
                    if (!Verify(request.Password, medic.PasswordHash))
                    {
                        //_logger.LogInformation("Attempting to log with password: {Password}", BCrypt.Net.BCrypt.HashPassword(request.Password));
                        //_logger.LogInformation("Hashed password: {PasswordHash}", medic.PasswordHash);

                        _logger.LogWarning("Login failed for medic with username: {Username} - Incorrect password", request.Uname);
                        return Unauthorized("Invalid username or password.");
                    }

                    var token = GenerateJwtToken(medic.Id, "Medic", medic.Uname);
                    return Ok(new { Token = token, Role = "Medic" });
                }
            }
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
