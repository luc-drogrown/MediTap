namespace MediTap.Api.Models
{
    /// <summary>
    /// Symptoms are added by the patient
    /// They can be viewed by Medics and checked
    /// </summary>
    public class Symptom
    {
        public int Id { get; set; }

        public string Description { get; set; }

        // A checked symptom means that a medic has aknowledged that the patient has this symptom
        public bool isChecked { get; set; }

        // A present symptom means that the patient currently has this symptom,
        // as opposed to a past symptom that the patient had in the past but doesn't have anymore
        public bool isPresent { get; set; }
        public DateTime AddedDate { get; set; }

        public DateOnly? StartOfSymptoms { get; set; }

        // Foreign Keys
        // The Patient that added this symptom
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // The Medic that checked this symptom
        public int? MedicId { get; set; }
        public Medic? Medic { get; set; }

    }
}
