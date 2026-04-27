using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediTap.Api.DTO;
namespace MediTap.Api.Services
{
    public class AffectionService : IAffectionService
    {
        private readonly MediTapDbContext _context;
        private readonly ILogger<AffectionService> _logger;
        public AffectionService(MediTapDbContext context, ILogger<AffectionService> logger)
        {
            _context = context;
            _logger = logger;
        }


        // TODO -> Check that the affection date is not in the future
        AffectionDTO IAffectionService.Add(AffectionCreationDTO affection, int userId)
        {
            var affectionEntity = new Affection
            {
                DiagnoseDate = affection.DiagnoseDate,
                Name = affection.Name,
                PatientId = affection.PatientId,
                MedicId = userId,
                Description = affection.Description ?? string.Empty,
            };

            _context.Add(affectionEntity);
            _context.SaveChanges();
            return new AffectionDTO(affectionEntity);
        }

        void IAffectionService.Delete(int id)
        {
            try
            {
                _context.Affections.Where(s => s.Id == id)
                    .ExecuteDelete();
                _logger.LogInformation($"Affection deleted: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        AffectionDTO IAffectionService.GetById(int id, int userId)
        {
            try
            {
                var affection = _context.Affections
                    .FirstOrDefault(a => a.Id == id && a.Patient.Medics.Any(m => m.Id == userId));

                if ( affection == null ) { return null; }

                return new AffectionDTO(affection);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        bool IAffectionService.GetAuthById(int id, int userId)
        {
            try
            {
                var isAuth = _context.Affections
                    .Any(a => a.Id == id && a.Patient.Medics.Any(m => m.Id == userId));

                return isAuth;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
