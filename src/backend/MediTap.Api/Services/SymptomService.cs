using MediTap.Api.Models;
using MediTap.Api.DTO;
using MediTap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace MediTap.Api.Services
{
    public class SymptomService : ISymptomService
    {
        private readonly MediTapDbContext _context;
        private readonly ILogger<SymptomService> _logger;

        public SymptomService(MediTapDbContext context, ILogger<SymptomService> logger)
        {
            _context = context;
            _logger = logger;
        }

        void ISymptomService.Delete(int id)
        {
            try
            {
                _context.Symptoms.Where(s => s.Id == id)
                    .ExecuteDelete();
                _logger.LogInformation($"Symptoms deleted: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            
        }

        bool ISymptomService.GetByIdIfAuthorized(int id, int userId, string role)
        {
            if(role != "Patient") { return false; }

            else
            {
                try
                {
                    var symptom = _context.Symptoms
                        .FirstOrDefault(s => s.Id == id && s.PatientId == userId);

                    _logger.LogInformation($"Uid {userId} asked permission to access Sid {symptom}");
                    return symptom != null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    throw;
                }
            }

        }

        void ISymptomService.Update(SymptomUpdateDTO symptom, int id)
        {
            try
            {
                int rowsAffected = _context.Symptoms
                    .Where(s => s.Id == id)
                    .ExecuteUpdate(setters => setters
                        // Change the isPresent property
                        .SetProperty(s => s.isPresent, symptom.isPresent)
                        .SetProperty(s => s.Description, symptom?.Description)
                        .SetProperty(s => s.StartOfSymptoms, symptom?.StartOfSymptom)
                        );

                _logger.LogInformation($"{rowsAffected} by updating symptom {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

    }
}
