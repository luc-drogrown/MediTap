using MediTap.Api;
using MediTap.Api.Models;
using MediTap.Api.Services;
using MediTap.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Adding the Database connection
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MediTapDbContext>(options => options.UseNpgsql(connString,
   
    // This enables retry on ded connections
    npgsqlOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(2),
            errorCodesToAdd: null);
    }
));

// Adding the services
builder.Services.AddScoped<IMedicService, MedicService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ISymptomService, SymptomService>();


// Adding the controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Tells the JSON reader/writer to use strings instead of numbers for ALL enums
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Adding Authentication with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Seed the database with the admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        DbSeeder.SeedAdminUser(services, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();  

app.UseAuthorization();

app.MapControllers();

app.Run();

