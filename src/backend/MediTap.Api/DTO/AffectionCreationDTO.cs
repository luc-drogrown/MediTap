using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class AffectionCreationDTO
    {
        public string Name { get; set; }
        public DateOnly DiagnoseDate { get; set; }
        public string? Description { get; set; }

        // FOreign key
        public int PatientId { get; set; }


    }
}
