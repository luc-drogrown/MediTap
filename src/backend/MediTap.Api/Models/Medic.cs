namespace MediTap.Api.Models
{
    public class Medic
    {
        /// <summary>
        /// This class models a medic in the system
        /// </summary>

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Specialty { get; set; }

        // These two are used to login to the system
        // they are gee=nerated when a new Medic is created
        public string Uname { get; private set; }
        // Password is set by the medic
        public string PasswordHash { get; set; }

        // The statuses were taken from the [here](https://regmed.cmr.ro/registrul-medicilor)
        public MedicStatus MedicStatus { get; set; }


        // Non mandatory info about the medic
        public string? LastName { get; set; }
        public Email? Email { get; set; }
        public PhoneNumber? PhoneNumber { get; set; }


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

        // A collection of affections diagnosed by the medic.
        // A medic can diagnose multiple affections => one-to-many relationship
        public ICollection<Affection> Affections { get; set; }



        // Constructors
        public Medic(string firstName, string specialty)
        {
            if(firstName == null) { throw new ArgumentNullException("First name cannot be null"); }
            if(specialty == null) { throw new ArgumentNullException("Specialty cannot be null"); }

            this.FirstName = firstName;
            this.Specialty = specialty;

            this.Uname = "M-" + this.FirstName + "-" + this.Specialty + "-" + Guid.NewGuid().ToString().Substring(0, 8);
        }

        private Medic() { }

    }
}
