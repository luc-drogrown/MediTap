using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    // TODO
    public class AffectionDTO
    {
        public string Name { get; set; }
        public DateOnly DiagnoseDate { get; set; }
        public string? Description { get; set; }



        public AffectionDTO(Affection a)
        {
        }
    }
}