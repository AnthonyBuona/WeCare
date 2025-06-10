using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Add_Correct_Entities_And_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrincipalResponsibleId1",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TherapistId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppTherapists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTherapists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppTratamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TherapistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTratamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTratamentos_AppPatients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AppPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppTratamentos_AppTherapists_TherapistId",
                        column: x => x.TherapistId,
                        principalTable: "AppTherapists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_PrincipalResponsibleId1",
                table: "AppPatients",
                column: "PrincipalResponsibleId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_TherapistId",
                table: "AppPatients",
                column: "TherapistId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTratamentos_PatientId",
                table: "AppTratamentos",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTratamentos_TherapistId",
                table: "AppTratamentos",
                column: "TherapistId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppResponsibles_PrincipalResponsibleId1",
                table: "AppPatients",
                column: "PrincipalResponsibleId1",
                principalTable: "AppResponsibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppTherapists_TherapistId",
                table: "AppPatients",
                column: "TherapistId",
                principalTable: "AppTherapists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppResponsibles_PrincipalResponsibleId1",
                table: "AppPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppTherapists_TherapistId",
                table: "AppPatients");

            migrationBuilder.DropTable(
                name: "AppTratamentos");

            migrationBuilder.DropTable(
                name: "AppTherapists");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_PrincipalResponsibleId1",
                table: "AppPatients");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_TherapistId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PrincipalResponsibleId1",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "TherapistId",
                table: "AppPatients");
        }
    }
}
