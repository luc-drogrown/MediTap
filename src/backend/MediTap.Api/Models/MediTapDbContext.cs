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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var builder = new ConfigurationBuilder()
                //.AddUserSecrets();
            var connString = 
            optionsBuilder.UseNpgsql("Server=localhost;Database=MediTapDb;Trusted_Connection=True;");
        }

    }
}
