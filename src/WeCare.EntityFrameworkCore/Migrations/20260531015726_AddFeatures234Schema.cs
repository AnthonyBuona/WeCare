using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatures234Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppBillingBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    XmlPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashSignature = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppBillingBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppBillingGuides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConsultationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HealthInsuranceName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthorizationPassword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsultationValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppBillingGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBillingGuides_AppConsultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "AppConsultations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppCaregiverQuests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    VideoTutorialUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    XpReward = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppCaregiverQuests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCaregiverQuests_AppPatients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AppPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppCrossTenantAccessConsents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedPermissions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AuthTokenHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppCrossTenantAccessConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCrossTenantAccessConsents_AppPatients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AppPatients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppTussProcedureMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Specialty = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TussCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppTussProcedureMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserGamifiedProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    TotalXp = table.Column<int>(type: "int", nullable: false),
                    ActiveStreak = table.Column<int>(type: "int", nullable: false),
                    UnlockedBadgesJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppUserGamifiedProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserGamifiedProfiles_AbpUsers_ParentUserId",
                        column: x => x.ParentUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppQuestExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnjoymentScore = table.Column<int>(type: "int", nullable: false),
                    CaregiverNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppQuestExecutionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppQuestExecutionLogs_AppCaregiverQuests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "AppCaregiverQuests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppSharedAccessAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConsentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAccessingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionPerformed = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccessingIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSharedAccessAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSharedAccessAuditLogs_AppCrossTenantAccessConsents_ConsentId",
                        column: x => x.ConsentId,
                        principalTable: "AppCrossTenantAccessConsents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBillingGuides_ConsultationId",
                table: "AppBillingGuides",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCaregiverQuests_PatientId",
                table: "AppCaregiverQuests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCrossTenantAccessConsents_AuthTokenHash_TenantId",
                table: "AppCrossTenantAccessConsents",
                columns: new[] { "AuthTokenHash", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppCrossTenantAccessConsents_PatientId",
                table: "AppCrossTenantAccessConsents",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppQuestExecutionLogs_QuestId",
                table: "AppQuestExecutionLogs",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSharedAccessAuditLogs_ConsentId",
                table: "AppSharedAccessAuditLogs",
                column: "ConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserGamifiedProfiles_ParentUserId",
                table: "AppUserGamifiedProfiles",
                column: "ParentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBillingBatches");

            migrationBuilder.DropTable(
                name: "AppBillingGuides");

            migrationBuilder.DropTable(
                name: "AppQuestExecutionLogs");

            migrationBuilder.DropTable(
                name: "AppSharedAccessAuditLogs");

            migrationBuilder.DropTable(
                name: "AppTussProcedureMappings");

            migrationBuilder.DropTable(
                name: "AppUserGamifiedProfiles");

            migrationBuilder.DropTable(
                name: "AppCaregiverQuests");

            migrationBuilder.DropTable(
                name: "AppCrossTenantAccessConsents");
        }
    }
}
