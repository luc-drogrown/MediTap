using MediTap.Api.DTO;
using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IPendingPatientRegistrationService
    {
        PendingPatientRegistrationDTO Prepare(PatientCreationDTO request);

        Patient Confirm(string pendingRegistrationId);

        void Cancel(string pendingRegistrationId);
    }
}