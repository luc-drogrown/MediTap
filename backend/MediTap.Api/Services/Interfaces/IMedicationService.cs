using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IMedicationService
    {
        Medication GetById(int id, int userId, string role);
        Medication Add(Medication medication, int userId, string role);
        void Update(Medication medication);
        void Delete(int id);
    }
}
