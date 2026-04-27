using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    // TODO
    public class AffectionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly DiagnoseDate { get; set; }
        public string? Description { get; set; }

        // FOreign key
        public int PatientId { get; set; }


        public AffectionDTO(Affection a)
        {
            Id = a.Id;
            Name = a.Name;
            DiagnoseDate = a.DiagnoseDate;
            Description = a.Description ?? string.Empty;
            PatientId = a.PatientId;
        }
    }
}