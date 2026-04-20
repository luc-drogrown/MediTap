namespace MediTap.Api.Models
{
    public class Patient
    {
        /// Essential info that must be provided for a patient to be created.
        public int Id { get; set; }
        public DateOnly DateOfBirth { get; set;}
        public CNP CNP { get; set; }
        public string FirstName { get; set; }
        
        
        /// Non essential info about the patient
        public string? LastName { get; set; }
        public Email? Email { get; set; }

        //TODO add a phone number class and exceptions for invalid phone numbers
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
    }
}
