using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediTap.Api.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly ILogger<MedicationService> _logger;
        private readonly MediTapDbContext _context;
        public MedicationService(MediTapDbContext context ,ILogger<MedicationService> logger)
        {
            _context = context;
            _logger = logger;
        }




        // TODO -> Add checks that the Medication is valid (dates are not in the future, etc.)
        MedicationDTO IMedicationService.Add(MedicationCreationDTO medication, int userId)
        {
            try
            {
                var medicationEntity = new Medication
                {
                    Name = medication.Name,
                    Brand = medication.Brand ?? string.Empty,
                    IssueDate = medication.IssueDate,
                    EndDate = medication.EndDate ?? null,
                    UnitOfMeasure = medication.UnitOfMeasure,
                    Quantity = medication.Quantity,
                    PercentReimbursed = medication.PercentReimbursed,
                    StartDate = medication.StartDate ?? null,

                    MedicId = userId,
                    PatientId = medication.PatientId


                };

                _context.Medications.Add(medicationEntity);
                _context.SaveChanges();
                return new MedicationDTO(medicationEntity);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        void IMedicationService.Delete(int id)
        {
            try
            {
                _context.Medications.Where(m => m.Id == id)
                    .ExecuteDelete();

                _logger.LogInformation($"Medication with id {id} deleted");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        MedicationDTO IMedicationService.GetById(int id, int userId)
        {
            try
            {
                var medication = _context.Medications
                    .FirstOrDefault(a => a.Id == id && a.Patient.Medics.Any(m => m.Id == userId));

                if (medication == null) return null;

                return new MedicationDTO(medication);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        bool IMedicationService.GetAuth(int id, int userId)
        {
            try
            {
                // Checking that userId is linked to the PatientId from the Medication entity
                var isAuth = _context.Medications
                    .Any(m => m.Id == id && m.Patient.Medics.Any(medic => medic.Id == userId));
                return isAuth;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        void IMedicationService.Update(MedicationUpdateDTO medication, int id)
        {
            try
            {
                int rowsAffected = _context.Medications
                    .Where(m => m.Id == id)
                    .ExecuteUpdate(setters => setters
                        .SetProperty(s => s.Quantity, s => medication.Quantity ?? s.Quantity)
                        .SetProperty(s => s.UnitOfMeasure, s => medication.UnitOfMeasure ?? s.UnitOfMeasure)
                        .SetProperty(s => s.PercentReimbursed, s => medication.PercentReimbursed ?? s.PercentReimbursed)
                        .SetProperty(s => s.EndDate, s => medication.EndDate ?? s.EndDate)
                        .SetProperty(s => s.Brand, s => medication.Brand ?? s.Brand)
                    );
                _logger.LogInformation($"Rows affected {rowsAffected}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
