using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Added_PerformedTrainings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "AppConsultations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainTraining",
                table: "AppConsultations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PerformedTraining",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HelpNeeded = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAttempts = table.Column<int>(type: "int", nullable: false),
                    SuccessfulAttempts = table.Column<int>(type: "int", nullable: false),
                    ConsultationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_PerformedTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformedTraining_AppConsultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "AppConsultations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerformedTraining_ConsultationId",
                table: "PerformedTraining",
                column: "ConsultationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformedTraining");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "AppConsultations");

            migrationBuilder.DropColumn(
                name: "MainTraining",
                table: "AppConsultations");
        }
    }
}
