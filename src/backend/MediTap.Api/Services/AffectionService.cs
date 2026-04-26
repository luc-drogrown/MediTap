using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        // @ TODO Implement the method
        void IAffectionService.Add(Affection affection, int userId, string role)
        {
            return;    
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

        // @ TODO Implement the method
        Affection IAffectionService.GetById(int id, string role)
        {
            if(role != "Medic") { return null; }

        }
    }
}
