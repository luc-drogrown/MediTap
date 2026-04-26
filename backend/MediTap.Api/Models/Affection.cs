namespace MediTap.Api.Models
{
    public class Affection
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public DateOnly DiagnoseDate { get; set; }

        public string? Description { get; set; }



        // Foreign Keys
        // The Patient that has this affection
        // It must have a patient, otherwise it doesn't make sense to have an affection without a patient
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // The Medic that diagnosed this affection
        public int MedicId { get; set; }
        public Medic Medic { get; set; }

    }
}
