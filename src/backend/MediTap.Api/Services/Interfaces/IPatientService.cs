
using MediTap.Api.Models;
namespace MediTap.Api.Services.Interfaces
{
    public interface IPatientService
    {
        Patient GetById(int id);
        Patient AddSymptom(Symptom symptom, int patientId, string role);
    }
}
