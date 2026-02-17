using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Add_Guest_Relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ResponsibleId",
                table: "AppGuests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Relationship",
                table: "AppGuests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Relationship",
                table: "AppGuests");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResponsibleId",
                table: "AppGuests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
