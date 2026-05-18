using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediTap.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientStatus",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientStatus",
                table: "Patients");
        }
    }
}
