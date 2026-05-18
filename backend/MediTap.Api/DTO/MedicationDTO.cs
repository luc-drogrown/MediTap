using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class MedicationDTO
    {
        public int Id { get; set; }
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

        public int MedicId { get; set; }

        public MedicationDTO(Medication m)
        {
            Id = m.Id;
            Name = m.Name;
            Quantity = m.Quantity;
            UnitOfMeasure = m.UnitOfMeasure;
            IssueDate = m.IssueDate;
            PercentReimbursed = m.PercentReimbursed;
            StartDate = m.StartDate;
            EndDate = m.EndDate;
            Brand = m.Brand;
            PatientId = m.PatientId;
            MedicId = m.MedicId;

        }
    }
}