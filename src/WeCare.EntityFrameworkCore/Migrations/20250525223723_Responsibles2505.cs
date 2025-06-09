using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Responsibles2505 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AppPatients");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AppResponsibles",
                newName: "NameResponsible");

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "AppResponsibles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CPF",
                table: "AppResponsibles",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "AppResponsibles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AppResponsibles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrincipalResponsibleId",
                table: "AppPatients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppResponsibles_PatientId",
                table: "AppResponsibles",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPatients_PrincipalResponsibleId",
                table: "AppPatients",
                column: "PrincipalResponsibleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppPatients_AppResponsibles_PrincipalResponsibleId",
                table: "AppPatients",
                column: "PrincipalResponsibleId",
                principalTable: "AppResponsibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppResponsibles_AppPatients_PatientId",
                table: "AppResponsibles",
                column: "PatientId",
                principalTable: "AppPatients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppPatients_AppResponsibles_PrincipalResponsibleId",
                table: "AppPatients");

            migrationBuilder.DropForeignKey(
                name: "FK_AppResponsibles_AppPatients_PatientId",
                table: "AppResponsibles");

            migrationBuilder.DropIndex(
                name: "IX_AppResponsibles_PatientId",
                table: "AppResponsibles");

            migrationBuilder.DropIndex(
                name: "IX_AppPatients_PrincipalResponsibleId",
                table: "AppPatients");

            migrationBuilder.DropColumn(
                name: "CPF",
                table: "AppResponsibles");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "AppResponsibles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AppResponsibles");

            migrationBuilder.DropColumn(
                name: "PrincipalResponsibleId",
                table: "AppPatients");

            migrationBuilder.RenameColumn(
                name: "NameResponsible",
                table: "AppResponsibles",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "AppResponsibles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AppPatients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
