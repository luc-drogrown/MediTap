namespace MediTap.Api.Models
{
    public class Patient
    {
        /// Essential info that must be provided for a patient to be created.
        public int Id { get; set; }
        public DateOnly DateOfBirth { get; set;}
        public CNP CNP { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


        public string Uname { get; private set; }
        public string PasswordHash { get; set; }

        /// Non essential info about the patient
        public Email? Email { get; set; }

        public PhoneNumber? PhoneNumber { get; set; }
        public string? Address { get; set; }


        ///  A collection of medications prescribed to the patient. 
        ///  This represents a one-to-many relationship
        public ICollection<Medication>?  Medications { get; set; }

        // A collection of medics associated with the patient.
        // A patient can see multiple medics => many-to-many relationship
        public ICollection<Medic>? Medics { get; set; }

        // A collection of appointments associated with the patient.
        // A patient can have multiple appointments => one-to-many relationship
        public ICollection<Appointment>? Appointments { get; set; }

        // A collection of affections associated with the patient.
        // A patient can have multiple affections => one-to-many relationship
        public ICollection<Affection>? Affections { get; set; }

        // A collection of symptoms associated with the patient.
        // A patient can have multiple symptoms => one-to-many relationship
        public ICollection<Symptom> Symptoms { get; set; }


        // Constructors
        public Patient(string firstName, CNP cnp, DateOnly dateOfBirth, string password)
        {
            if(firstName == null) { throw new ArgumentNullException("First name cannot be null"); }
            if(cnp == null) {throw new ArgumentNullException("CNP cannot be null"); }
            if(password == null) { throw new ArgumentNullException("Password cannot be null"); }


            this.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            this.FirstName = firstName;
            this.DateOfBirth = dateOfBirth;
            this.CNP = cnp;

            this.Uname = "P-" + this.FirstName + "-" + this.CNP.CodNumericPersonal.ToString().Substring(0,4) + "-" + Guid.NewGuid().ToString().Substring(0, 8);
        }

        private Patient()
        { }
    }
}
