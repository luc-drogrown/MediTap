using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IAffectionService
    {
        Affection GetById(int id, string role);
        void Add(Affection affection, int userId, string role);
        void Delete(int id);
    }
}
