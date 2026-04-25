namespace MediTap.Api.DTO
{
    public class SymptomCreationDTO
    {
        // Self properties
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isChecked { get; set; }
        public bool isPresent { get; set; }
        public DateTime AddedTime { get; set; }
        public DateOnly? StartOfSymptom { get; set; }
        public string? Description { get; set; }


        // FOreign keys
        public int PatientId { get; set; }

        public int? MedicId { get; set; }
    }
}
