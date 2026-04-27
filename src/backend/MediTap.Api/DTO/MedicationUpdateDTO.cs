

namespace MediTap.Api.DTO
{
    public class MedicationUpdateDTO
    {
        public float? Quantity { get; set; }

        public string? UnitOfMeasure { get; set; }

        public int? PercentReimbursed { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Brand { get; set; }

    }
}
