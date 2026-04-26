using MediTap.Api.Models;

namespace MediTap.Api.DTO
{
    public class SymptomDTO
    {
        // Self properties
        public int Id { get; set; }
        public string Name { get; set; }
        public  bool isChecked { get; set; }
        public bool isPresent { get; set; }
        public  DateTime AddedTime { get; set; }
        public DateOnly? StartOfSymptom { get; set; }
        public string? Description { get; set; }


        // FOreign keys
        public int PatientId { get; set; }

        public int? MedicId { get; set; }

        public SymptomDTO(Symptom s)
        {
            this.Id = s.Id;
            this.Name = s.Name;
            this.isChecked = s.isChecked;
            this.isPresent = s.isPresent;
            this.AddedTime = s.AddedDate;
            this.StartOfSymptom = s.StartOfSymptoms ?? null;
            this.Description = s.Description ?? null;

            this.PatientId = s.PatientId;

            this.MedicId = s.MedicId ?? null;
        }
    }
}