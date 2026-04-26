namespace MediTap.Api.Models
{
    public class Medication
    {
        /// <summary>
        /// This class models the medication that was prescribed to a patient
        /// </summary>
        public int Id { get; set; }

        public string Name { get; set; }

        public float Quantity { get; set; }

        public string UnitOfMeasure { get; set; }

        public DateOnly IssueDate { get; set; }

        // Cat la % din pret e decontat 
        // @TODO: decide if this should be implemented
        public int PercentReimbursed { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Brand { get; set; }



        // Foreign key to Patient that needs the medication
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // Foreign key to Medic that issued the medication
        public int MedicId { get; set; }
        public Medic Medic { get; set; }
    }
}
