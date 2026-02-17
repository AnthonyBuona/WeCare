using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressNumber",
                table: "AppClinics",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppointmentDurationMinutes",
                table: "AppClinics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AppClinics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "AppClinics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "AppClinics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "AppClinics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "AppClinics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "AppClinics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "AppClinics",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "AppClinics",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "AppClinics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "AppClinics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcomeMessage",
                table: "AppClinics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "AppClinics",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppClinicOperatingHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    BreakStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreakEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppClinicOperatingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppClinicOperatingHours_AppClinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "AppClinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_PatientId",
                table: "Objectives",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppClinicOperatingHours_ClinicId",
                table: "AppClinicOperatingHours",
                column: "ClinicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_AppPatients_PatientId",
                table: "Objectives",
                column: "PatientId",
                principalTable: "AppPatients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_AppPatients_PatientId",
                table: "Objectives");

            migrationBuilder.DropTable(
                name: "AppClinicOperatingHours");

            migrationBuilder.DropIndex(
                name: "IX_Objectives_PatientId",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "AddressNumber",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "AppointmentDurationMinutes",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "State",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "WelcomeMessage",
                table: "AppClinics");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "AppClinics");
        }
    }
}
