using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace MediTap.Api.Services
{
    public class MedicService : IMedicService
    {

        private readonly MediTapDbContext _context;
        private readonly ILogger<MedicService> _logger;
        public MedicService(MediTapDbContext context, ILogger<MedicService> logger)
        {
            _context = context;
            _logger = logger;
        }

        Medic IMedicService.Create(MedicCreationDTO request)
        {
            var medic = new Medic(request.FirstName, request.Specialty, request.Password, request.MedicStatus)
            {
                LastName = request.LastName ?? string.Empty,
                Email = request.Email != null ? new Email(request.Email) : null,
                PhoneNumber = request.PhoneNumber != null ? new PhoneNumber(request.PhoneNumber) : null
            };

            if(_context.Medics.Any(m => m.Uname == medic.Uname))
            {
                _logger.LogError("Attempt to create a medic with an existing username: {Uname}", medic.Uname);
                return null;
            }

            _context.Medics.Add(medic);
            _context.SaveChanges();
            return medic;
        }

        Medic IMedicService.GetById(int id)
        {
            var medic = _context.Medics.Find(id);

            return medic;
        }
    }
}
