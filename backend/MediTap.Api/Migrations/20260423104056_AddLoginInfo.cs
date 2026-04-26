using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediTap.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Uname",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Medics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Uname",
                table: "Medics",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Uname",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Medics");

            migrationBuilder.DropColumn(
                name: "Uname",
                table: "Medics");
        }
    }
}
