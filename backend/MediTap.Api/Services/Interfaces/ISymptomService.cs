using MediTap.Api.Models;
namespace MediTap.Api.Services.Interfaces
{
    public interface ISymptomService
    {
        Symptom GetById(int id, int userId, string role);
        void Update(Symptom symptom);
        void Delete(int id);
    }
}
