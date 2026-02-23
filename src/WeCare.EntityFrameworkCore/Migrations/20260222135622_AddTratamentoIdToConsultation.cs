using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddTratamentoIdToConsultation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AppPerformedTrainings;");
            migrationBuilder.Sql("DELETE FROM AppConsultations;");

            migrationBuilder.AddColumn<Guid>(
                name: "TratamentoId",
                table: "AppConsultations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppConsultations_TratamentoId",
                table: "AppConsultations",
                column: "TratamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppConsultations_AppTratamentos_TratamentoId",
                table: "AppConsultations",
                column: "TratamentoId",
                principalTable: "AppTratamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppConsultations_AppTratamentos_TratamentoId",
                table: "AppConsultations");

            migrationBuilder.DropIndex(
                name: "IX_AppConsultations_TratamentoId",
                table: "AppConsultations");

            migrationBuilder.DropColumn(
                name: "TratamentoId",
                table: "AppConsultations");
        }
    }
}
