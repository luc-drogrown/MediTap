namespace MediTap.Api.Models
{
    public class Appointment
    {
        ///<summary>
        /// This class models an appointment between a patient and a medic
        /// </summary>


        public int Id { get; set; }

        public DateTime Date { get; set; }

        // The date when the appointment was issued by the medic
        public DateTime IssueDate { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        // Foreign keys

        // The medic that issued the appointment
        public int MedicId { get; set; }
        public Medic Medic { get; set; }


        // The patient that has the appointment
        public int PatientId { get; set; }
        public Patient Patient { get; set; }


    }
}
