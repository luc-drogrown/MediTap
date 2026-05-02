namespace MediTap.Api.DTO
{
    public class AppointmentCreationDTO
    {
        public DateTime Date { get; set; }
        public DateTime IssueDate { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }


        // Foreign keys
        
        public int MedicId { get; set; }
        public int PatientId { get; set; }



    }
}
