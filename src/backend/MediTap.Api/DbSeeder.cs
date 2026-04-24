using MediTap.Api.Models;

namespace MediTap.Api
{
    public class DbSeeder
    {

        public static void SeedAdminUser(IServiceProvider serviceProvider, ILogger logger)
        {
            using var context = serviceProvider.GetRequiredService<MediTapDbContext>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            context.Database.EnsureCreated(); 

            // Check if Id = 1 already exists to avoid duplicates
            if (!context.Medics.Any(p => p.Id == 1))
            {
                var adminPassword = configuration["AdminSettings:DefaultPassword"];
                logger.LogInformation("Admin password retrieved from configuration: {AdminPassword}", adminPassword);
                if (string.IsNullOrEmpty(adminPassword))
                {
                    throw new Exception("Admin password is not set in secrets.json!");
                }

                
                var adminUser = new Medic(1, adminPassword);

                logger.LogInformation("Admin created {AdminUser}", adminUser);

                adminUser.FirstName = "Admin";
                adminUser.Specialty = "Admin";
                adminUser.MedicStatus = MedicStatus.Active;


                // Save to the database
                context.Medics.Add(adminUser);
                logger.LogInformation("Admin user added to the context.");
                context.SaveChanges();
            }
        }

    }
}
