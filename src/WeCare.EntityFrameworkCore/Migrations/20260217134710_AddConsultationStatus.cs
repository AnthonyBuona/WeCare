using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppConsultations_Objectives_ObjectiveId",
                table: "AppConsultations");

            migrationBuilder.AlterColumn<Guid>(
                name: "ObjectiveId",
                table: "AppConsultations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppConsultations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AppConsultations_Objectives_ObjectiveId",
                table: "AppConsultations",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppConsultations_Objectives_ObjectiveId",
                table: "AppConsultations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppConsultations");

            migrationBuilder.AlterColumn<Guid>(
                name: "ObjectiveId",
                table: "AppConsultations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppConsultations_Objectives_ObjectiveId",
                table: "AppConsultations",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
