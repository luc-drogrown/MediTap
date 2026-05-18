using MediTap.Api.Models;   
namespace MediTap.Api.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime IssueDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }


        // Foreign keys
        public int MedicId { get; set; }
        public int PatientId { get; set; }


        // Constructor that takes an Appointment entity and maps it to the DTO
        public AppointmentDTO(Appointment a)
        {
            // Appointment entity prop
            Id = a.Id;
            Date = a.Date;
            IssueDate = a.IssueDate;
            Title = a.Title ?? string.Empty;
            Description = a.Description ?? string.Empty;

            // Patient entity props
            PatientId = a.PatientId;


            // Medic entity props
            MedicId = a.MedicId;

        }




    }
}
