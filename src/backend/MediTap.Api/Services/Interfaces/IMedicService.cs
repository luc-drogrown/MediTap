using MediTap.Api.DTO;
using MediTap.Api.Models;

namespace MediTap.Api.Services.Interfaces
{
    public interface IMedicService
    {
        Medic GetById(int id);
        Medic Create(MedicCreationDTO request);
    }
}
