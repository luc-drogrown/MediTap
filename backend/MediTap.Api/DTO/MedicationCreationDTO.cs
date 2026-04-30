
namespace MediTap.Api.DTO
{
    public class MedicationCreationDTO
    {
        public string Name { get; set; }

        public float Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public DateOnly IssueDate { get; set; }

        public int PercentReimbursed { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Brand { get; set; }


        // Foreign keys
        public int PatientId { get; set; }


    }
}