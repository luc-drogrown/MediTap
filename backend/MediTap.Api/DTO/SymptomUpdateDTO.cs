namespace MediTap.Api.DTO
{
    public class SymptomUpdateDTO
    {
        // Patient can toggle this if symtpom stopped
        public bool isPresent { get; set; }

        public DateOnly? StartOfSymptom { get; set; }
        public string? Description { get; set; }
    }
}