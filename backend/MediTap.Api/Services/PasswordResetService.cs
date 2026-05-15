using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediTap.Api.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly MediTapDbContext _context;
        private readonly ILogger<PasswordResetService> _logger;

        public PasswordResetService(
            MediTapDbContext context,
            ILogger<PasswordResetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void CreateRequest(PasswordResetRequestCreationDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (request.Role != "Patient" && request.Role != "Medic")
            {
                throw new ArgumentException("Invalid role.");
            }

            var accountExists = request.Role == "Patient"
                ? _context.Patients
                    .AsEnumerable()
                    .Any(p =>
                        p.Email != null &&
                        string.Equals(p.Email.EmailAddress, request.Email, StringComparison.OrdinalIgnoreCase))
                : _context.Medics
                    .AsEnumerable()
                    .Any(m =>
                        m.Email != null &&
                        string.Equals(m.Email.EmailAddress, request.Email, StringComparison.OrdinalIgnoreCase));

            if (!accountExists)
            {
                _logger.LogInformation(
                    "Password reset requested for non-existing {Role} email: {Email}",
                    request.Role,
                    request.Email);

                return;
            }

            var alreadyPending = _context.PasswordResetRequests.Any(r =>
                r.Email == request.Email &&
                r.Role == request.Role &&
                r.Status == "Pending");

            if (alreadyPending)
            {
                return;
            }

            var resetRequest = new PasswordResetRequest
            {
                Email = request.Email,
                Role = request.Role,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _context.PasswordResetRequests.Add(resetRequest);
            _context.SaveChanges();
        }

        public IEnumerable<PasswordResetRequestDTO> GetPendingRequests()
        {
            return _context.PasswordResetRequests
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new PasswordResetRequestDTO
                {
                    Id = r.Id,
                    Email = r.Email,
                    Role = r.Role,
                    Status = r.Status,
                    RequestedAt = r.RequestedAt
                })
                .ToList();
        }

        public void CompleteRequest(int requestId, CompletePasswordResetDTO request, int adminId)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new ArgumentException("New password is required.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new ArgumentException("Passwords do not match.");
            }

            var resetRequest = _context.PasswordResetRequests
                .FirstOrDefault(r => r.Id == requestId && r.Status == "Pending");

            if (resetRequest == null)
            {
                throw new InvalidOperationException("Password reset request was not found.");
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            if (resetRequest.Role == "Patient")
            {
                var patient = _context.Patients
                    .AsEnumerable()
                    .FirstOrDefault(p =>
                        p.Email != null &&
                        string.Equals(p.Email.EmailAddress, resetRequest.Email, StringComparison.OrdinalIgnoreCase));

                if (patient == null)
                {
                    throw new InvalidOperationException("Patient account was not found.");
                }

                patient.PasswordHash = newPasswordHash;
            }
            else if (resetRequest.Role == "Medic")
            {
                var medic = _context.Medics
                    .AsEnumerable()
                    .FirstOrDefault(m =>
                        m.Email != null &&
                        string.Equals(m.Email.EmailAddress, resetRequest.Email, StringComparison.OrdinalIgnoreCase));

                if (medic == null)
                {
                    throw new InvalidOperationException("Medic account was not found.");
                }

                medic.PasswordHash = newPasswordHash;
            }
            else
            {
                throw new ArgumentException("Invalid role.");
            }

            resetRequest.Status = "Completed";
            resetRequest.CompletedAt = DateTime.UtcNow;
            resetRequest.CompletedByAdminId = adminId;

            _context.SaveChanges();
        }

        public void RejectRequest(int requestId, int adminId)
        {
            var resetRequest = _context.PasswordResetRequests
                .FirstOrDefault(r => r.Id == requestId && r.Status == "Pending");

            if (resetRequest == null)
            {
                throw new InvalidOperationException("Password reset request was not found.");
            }

            resetRequest.Status = "Rejected";
            resetRequest.CompletedAt = DateTime.UtcNow;
            resetRequest.CompletedByAdminId = adminId;

            _context.SaveChanges();
        }
    }
}