using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IMedicService
    {
        Medic GetById(int id);
    }
}
