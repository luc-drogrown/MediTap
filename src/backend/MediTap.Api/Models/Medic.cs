namespace MediTap.Api.Models
{
    public class Medic
    {
        /// <summary>
        /// This class models a medic in the system
        /// </summary>

        public int Id { get; set; }

        public string FirstName { get; set; }

        public Email Email { get; set; }

        public string Specialty { get; set; }


        // The statuses were taken from the [here](https://regmed.cmr.ro/registrul-medicilor)
        public MedicStatus MedicStatus { get; set; }
        public string? LastName { get; set; }


        // Foreign keys

        // A collection of patients associated with the medic.
        // A medic can have multiple patients => many-to-many relationship
        public ICollection<Patient>? Patients { get; set; }

        // A collection of medications issued by the medic.
        // A medic can issue multiple medications => one-to-many relationship
        public ICollection<Medication>? Medications  { get; set; }

        // A collection of appointments associated with the medic.
        // A medic can have multiple appointments => one-to-many relationship
        public ICollection<Appointment> Appointments { get; set; }


    }
}
