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
        public string MedicFirstName { get; set; }
        public string MedicSpecialty { get; set; }
        public string? MedicLastName { get; set; }
        public string? MedicPhoneNumber { get; set; }
        public string? MedicEmail { get; set; }


        public int PatientId { get; set; }
        public string PatientFirstName { get; set; }
        public string? PatientLastName { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string PatientEmail { get; set; }

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
            PatientFirstName = a.Patient.FirstName;
            PatientLastName = a.Patient.LastName ?? string.Empty;
            PatientPhoneNumber = a.Patient.PhoneNumber?.Number ?? string.Empty;
            PatientEmail = a.Patient.Email?.EmailAddress ?? string.Empty;

            // Medic entity props
            MedicId = a.MedicId;
            MedicFirstName = a.Medic.FirstName;
            MedicSpecialty = a.Medic.Specialty;
            MedicEmail = a.Medic.Email?.EmailAddress ?? string.Empty;
            MedicLastName = a.Medic.LastName ?? string.Empty;
            MedicPhoneNumber = a.Medic.PhoneNumber?.Number ?? string.Empty;
        }

    }
}
