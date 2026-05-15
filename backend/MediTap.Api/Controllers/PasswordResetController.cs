using MediTap.Api.DTO;
using MediTap.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediTap.Api.Controllers
{
    [Route("api/password-reset")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;
        private readonly ILogger<PasswordResetController> _logger;

        public PasswordResetController(
            IPasswordResetService passwordResetService,
            ILogger<PasswordResetController> logger)
        {
            _passwordResetService = passwordResetService;
            _logger = logger;
        }

        [HttpPost("request")]
        public IActionResult CreateRequest([FromBody] PasswordResetRequestCreationDTO request)
        {
            try
            {
                _passwordResetService.CreateRequest(request);

                return Ok(new
                {
                    message = "If this email exists, a password reset request has been sent to the administrator."
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid password reset request.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating password reset request.");
                return StatusCode(500, "An error occurred while creating the password reset request.");
            }
        }

        [Authorize(Roles = "Medic")]
        [HttpGet("admin/pending")]
        public ActionResult<IEnumerable<PasswordResetRequestDTO>> GetPendingRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to view password reset requests by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                var requests = _passwordResetService.GetPendingRequests();

                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving password reset requests.");
                return StatusCode(500, "An error occurred while retrieving password reset requests.");
            }
        }

        [Authorize(Roles = "Medic")]
        [HttpPost("admin/complete/{requestId}")]
        public IActionResult CompleteRequest(int requestId, [FromBody] CompletePasswordResetDTO request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to complete password reset by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                _passwordResetService.CompleteRequest(requestId, request, userId);

                return Ok(new
                {
                    message = "Password reset completed successfully."
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid password reset completion request. Request ID: {RequestId}", requestId);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Password reset request not found or invalid. Request ID: {RequestId}", requestId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while completing password reset. Request ID: {RequestId}", requestId);
                return StatusCode(500, "An error occurred while completing password reset.");
            }
        }

        [Authorize(Roles = "Medic")]
        [HttpPost("admin/reject/{requestId}")]
        public IActionResult RejectRequest(int requestId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != 1)
            {
                _logger.LogWarning("Unauthorized attempt to reject password reset by user ID: {UserId}", userId);
                return NotFound();
            }

            try
            {
                _passwordResetService.RejectRequest(requestId, userId);

                return Ok(new
                {
                    message = "Password reset request rejected successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Password reset request not found or invalid. Request ID: {RequestId}", requestId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while rejecting password reset. Request ID: {RequestId}", requestId);
                return StatusCode(500, "An error occurred while rejecting password reset request.");
            }
        }
    }
}