using MediTap.Api.DTO;
using MediTap.Api.Models;
using MediTap.Api.Exceptions;
using MediTap.Api.Services.Interfaces;
namespace MediTap.Api.Services
{
    // DONE
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
            try
            {
                var medic = new Medic(request.FirstName, request.Specialty, request.Password, request.MedicStatus)
                {
                    LastName = request.LastName ?? string.Empty,
                    Email = request.Email != null ? new Email(request.Email) : null,
                    PhoneNumber = request.PhoneNumber != null ? new PhoneNumber(request.PhoneNumber) : null
                };
                if (_context.Medics.Any(m => m.Uname == medic.Uname))
                {
                    _logger.LogError("Attempt to create a medic with an existing username: {Uname}", medic.Uname);
                    return null;
                }

                _context.Medics.Add(medic);
                _context.SaveChanges();
                return medic;
            }
            catch (InvalidPhoneNumberException ex){
                _logger.LogError(ex, "Invalid phone number provided for medic creation: {PhoneNumber}", request.PhoneNumber);
                throw;

            }
            catch (InvalidEmailException ex)
            {
                _logger.LogError(ex, "Invalid email provided for medic creation: {Email}", request.Email);
                throw;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a medic");
                throw;
            }
        }

        MedicSummaryDTO IMedicService.GetById(int id)
        {
            var medic = _context.Medics.Find(id);

            if(medic == null)
            {
                _logger.LogWarning("Medic with ID {Id} not found", id);
                return null;
            }

            // Creates a MedicSummaryDTO from the Medic entity found in the DB
            // so that we return only neccesary information to the frontend
            var medicDTO = new MedicSummaryDTO(medic);

            return medicDTO;
        }
    }
}
