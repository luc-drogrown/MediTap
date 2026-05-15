using MediTap.Api.Models.Converters;
using Microsoft.EntityFrameworkCore;

namespace MediTap.Api.Models
{
    public class MediTapDbContext : DbContext
    {
        // Here I defined all the tables in our DB
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medic> Medics { get; set; }
        public DbSet<Affection> Affections { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Medication> Medications{ get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }

        public MediTapDbContext(DbContextOptions<MediTapDbContext> options) : base(options) { }

        // Tells EF how to handle Email, CNP and PhoneNumber types
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<CNP>()
                .HaveConversion<CNPConverter>();

            configurationBuilder.Properties<Email>()
                .HaveConversion<EmailConverter>();

            configurationBuilder.Properties<PhoneNumber>()
                .HaveConversion<PhoneNumberConverter>();

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Tells EF how to handle the MedicStatus enum when saving to the database. 
            modelBuilder.Entity<Medic>()
                .Property(e => e.MedicStatus)
                .HasConversion(

                    v => v.ToString(), // Convert enum to string for storage
                    v => (MedicStatus)Enum.Parse(typeof(MedicStatus), v) // Convert string back to enum when reading


                );

            // Tells EF how to handle the PatientStatus enum when saving to the database. 
            modelBuilder.Entity<Patient>()
                .Property(e => e.PatientStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (PatientStatus)Enum.Parse(typeof(PatientStatus), v)
                );
        }


    }
}
