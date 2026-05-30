using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Training_And_Objective_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activity",
                table: "AppPerformedTrainings");

            migrationBuilder.AddColumn<Guid>(
                name: "TrainingId",
                table: "AppPerformedTrainings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppTrainings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppTrainings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppTrainings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectiveId",
                table: "AppTrainings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppPerformedTrainings_TrainingId",
                table: "AppPerformedTrainings",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTrainings_ObjectiveId",
                table: "AppTrainings",
                column: "ObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_AppConsultations_ObjectiveId",
                table: "AppConsultations",
                column: "ObjectiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppConsultations_AppObjectives_ObjectiveId",
                table: "AppConsultations",
                column: "ObjectiveId",
                principalTable: "AppObjectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTrainings_AppObjectives_ObjectiveId",
                table: "AppTrainings",
                column: "ObjectiveId",
                principalTable: "AppObjectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppPerformedTrainings_AppTrainings_TrainingId",
                table: "AppPerformedTrainings",
                column: "TrainingId",
                principalTable: "AppTrainings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppConsultations_AppObjectives_ObjectiveId",
                table: "AppConsultations");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTrainings_AppObjectives_ObjectiveId",
                table: "AppTrainings");

            migrationBuilder.DropForeignKey(
                name: "FK_AppPerformedTrainings_AppTrainings_TrainingId",
                table: "AppPerformedTrainings");

            migrationBuilder.DropIndex(
                name: "IX_AppPerformedTrainings_TrainingId",
                table: "AppPerformedTrainings");

            migrationBuilder.DropIndex(
                name: "IX_AppTrainings_ObjectiveId",
                table: "AppTrainings");

            migrationBuilder.DropIndex(
                name: "IX_AppConsultations_ObjectiveId",
                table: "AppConsultations");

            migrationBuilder.DropColumn(
                name: "TrainingId",
                table: "AppPerformedTrainings");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppTrainings");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppTrainings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppTrainings");

            migrationBuilder.DropColumn(
                name: "ObjectiveId",
                table: "AppTrainings");

            migrationBuilder.AddColumn<string>(
                name: "Activity",
                table: "AppPerformedTrainings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
