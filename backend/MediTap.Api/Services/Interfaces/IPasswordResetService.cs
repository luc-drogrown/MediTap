using MediTap.Api.DTO;

namespace MediTap.Api.Services.Interfaces
{
    public interface IPasswordResetService
    {
        void CreateRequest(PasswordResetRequestCreationDTO request);

        IEnumerable<PasswordResetRequestDTO> GetPendingRequests();

        void CompleteRequest(int requestId, CompletePasswordResetDTO request, int adminId);

        void RejectRequest(int requestId, int adminId);
    }
}