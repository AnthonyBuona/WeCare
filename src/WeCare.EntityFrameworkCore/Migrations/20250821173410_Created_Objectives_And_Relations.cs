using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Created_Objectives_And_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerformedTraining_AppConsultations_ConsultationId",
                table: "PerformedTraining");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PerformedTraining",
                table: "PerformedTraining");

            migrationBuilder.RenameTable(
                name: "PerformedTraining",
                newName: "PerformedTrainings");

            migrationBuilder.RenameIndex(
                name: "IX_PerformedTraining_ConsultationId",
                table: "PerformedTrainings",
                newName: "IX_PerformedTrainings_ConsultationId");

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectiveId",
                table: "AppConsultations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PerformedTrainings",
                table: "PerformedTrainings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppTrainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTrainings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Objectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TherapistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppActivities_AppTrainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "AppTrainings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppActivities_TrainingId",
                table: "AppActivities",
                column: "TrainingId");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformedTrainings_AppConsultations_ConsultationId",
                table: "PerformedTrainings",
                column: "ConsultationId",
                principalTable: "AppConsultations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerformedTrainings_AppConsultations_ConsultationId",
                table: "PerformedTrainings");

            migrationBuilder.DropTable(
                name: "AppActivities");

            migrationBuilder.DropTable(
                name: "Objectives");

            migrationBuilder.DropTable(
                name: "AppTrainings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PerformedTrainings",
                table: "PerformedTrainings");

            migrationBuilder.DropColumn(
                name: "ObjectiveId",
                table: "AppConsultations");

            migrationBuilder.RenameTable(
                name: "PerformedTrainings",
                newName: "PerformedTraining");

            migrationBuilder.RenameIndex(
                name: "IX_PerformedTrainings_ConsultationId",
                table: "PerformedTraining",
                newName: "IX_PerformedTraining_ConsultationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PerformedTraining",
                table: "PerformedTraining",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformedTraining_AppConsultations_ConsultationId",
                table: "PerformedTraining",
                column: "ConsultationId",
                principalTable: "AppConsultations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
