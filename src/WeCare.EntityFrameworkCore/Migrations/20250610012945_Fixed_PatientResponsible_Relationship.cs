using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Fixed_PatientResponsible_Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppResponsibles_PrincipalResponsibleId1",
                table: "AppPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_AppResponsibles_AppPatients_PatientId",
                table: "AppResponsibles");

            migrationBuilder.DropIndex(
                name: "IX_AppResponsibles_PatientId",
                table: "AppResponsibles");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_PrincipalResponsibleId1",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "AppResponsibles");

            migrationBuilder.DropColumn(
                name: "PrincipalResponsibleId1",
                table: "AppPatients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "AppResponsibles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrincipalResponsibleId1",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppResponsibles_PatientId",
                table: "AppResponsibles",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_PrincipalResponsibleId1",
                table: "AppPatients",
                column: "PrincipalResponsibleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppResponsibles_PrincipalResponsibleId1",
                table: "AppPatients",
                column: "PrincipalResponsibleId1",
                principalTable: "AppResponsibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppResponsibles_AppPatients_PatientId",
                table: "AppResponsibles",
                column: "PatientId",
                principalTable: "AppPatients",
                principalColumn: "Id");
        }
    }
}
