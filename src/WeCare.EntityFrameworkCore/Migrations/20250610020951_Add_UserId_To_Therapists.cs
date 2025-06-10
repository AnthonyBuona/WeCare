using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserId_To_Therapists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppTherapists_TherapistId",
                table: "AppPatients");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_TherapistId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "TherapistId",
                table: "AppPatients");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AppTherapists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppTherapists");

            migrationBuilder.AddColumn<Guid>(
                name: "TherapistId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_TherapistId",
                table: "AppPatients",
                column: "TherapistId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppTherapists_TherapistId",
                table: "AppPatients",
                column: "TherapistId",
                principalTable: "AppTherapists",
                principalColumn: "Id");
        }
    }
}
