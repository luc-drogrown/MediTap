using MediTap.Api.DTO;
using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IMedicService
    {
        MedicSummaryDTO GetById(int id);

        /// <summary>
        /// Checks if Medic is linked with the Patient
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="mid"></param>
        /// <returns>True if patient is linked with the medic, false otherwise</returns>
        bool AuthCheck(int pid, int mid);

        Medic Create(MedicCreationDTO request);

        PatientSummaryDTO Scan(PatientScanDTO request, int userId, string role);

    }
}
