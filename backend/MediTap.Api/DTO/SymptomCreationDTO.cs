namespace MediTap.Api.DTO
{
    public class SymptomCreationDTO
    {
        // Self properties

        public string Name { get; set; }
        // When creating the Symptom is this is defaulted to false
        //public bool isChecked { get; set; }

        // When creating the symtpom this is defaulted to true
        //public bool isPresent { get; set; }
        public DateTime AddedTime { get; set; }
        public DateOnly? StartOfSymptom { get; set; }
        public string? Description { get; set; }


        // FOreign keys
        // We do't need this either
        //public int PatientId { get; set; }

        //public int? MedicId { get; set; }
    }
}