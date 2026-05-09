namespace MediTap.Api.DTO
{
    public class AppointmentCreationDTO
    {


        public DateTime Date { get; set; }
        public DateTime IssueDate { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }


        // Foreign keys
        // These are instantiated according to what the current user is
        //public int MedicId { get; set; }
        //public int PatientId { get; set; }

        // This is the Id of the Patient if current user is a Medic or vice-versa
        public int OtherUserId { get; set; }



    }
}
