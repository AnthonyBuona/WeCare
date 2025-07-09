using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class Correct_Therapist_Tenant_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AppTherapists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppTherapists_TenantId",
                table: "AppTherapists",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTherapists_AbpTenants_TenantId",
                table: "AppTherapists",
                column: "TenantId",
                principalTable: "AbpTenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTherapists_AbpTenants_TenantId",
                table: "AppTherapists");

            migrationBuilder.DropIndex(
                name: "IX_AppTherapists_TenantId",
                table: "AppTherapists");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AppTherapists");
        }
    }
}
