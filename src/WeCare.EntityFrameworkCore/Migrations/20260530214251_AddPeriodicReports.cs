using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodicReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPeriodicReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TherapistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumoClinico = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ObjetivosStatus = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EngajamentoCasa = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProximosPassos = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResponsibleSignatureDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponsibleSignatureIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponsibleSignatureCPF = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPeriodicReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPeriodicReports_AppPatients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AppPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppPeriodicReports_AppTherapists_TherapistId",
                        column: x => x.TherapistId,
                        principalTable: "AppTherapists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPeriodicReports_PatientId",
                table: "AppPeriodicReports",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPeriodicReports_TherapistId",
                table: "AppPeriodicReports",
                column: "TherapistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPeriodicReports");
        }
    }
}
