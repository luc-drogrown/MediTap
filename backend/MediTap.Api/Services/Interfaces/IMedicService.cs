using MediTap.Api.DTO;
using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IMedicService
    {
        MedicSummaryDTO GetById(int id);
        Medic Create(MedicCreationDTO request);

        PatientSummaryDTO Scan(PatientScanDTO request, int userId, string role);

    }
}
