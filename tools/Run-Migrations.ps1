<#
.SYNOPSIS
    Automates Entity Framework Core migrations and runs the DbMigrator for WeCare.
.DESCRIPTION
    This script provides a quick, standardized command for developers and agents 
    to add migrations, apply them, and seed the SQL Server database.
.PARAMETER MigrationName
    The name of the EF migration to create. If empty, the script will only run existing migrations.
.EXAMPLE
    .\tools\Run-Migrations.ps1 -MigrationName "AddAttendanceEntity"
#>
param (
    [string]$MigrationName
)

$ErrorActionPreference = "Stop"

Write-Host "Starting WeCare DB Automation..." -ForegroundColor Cyan

# 1. Add EF Migration if name is provided
if (-not [string]::IsNullOrEmpty($MigrationName)) {
    Write-Host "Step 1: Creating EF Core Migration $MigrationName..." -ForegroundColor Yellow
    dotnet ef migrations add $MigrationName --project src\WeCare.EntityFrameworkCore --startup-project src\WeCare.DbMigrator --context WeCareDbContext
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "EF Migration creation failed!"
        exit $LASTEXITCODE
    }
    Write-Host "Migration $MigrationName created successfully." -ForegroundColor Green
}

# 2. Build the DbMigrator project
Write-Host "Step 2: Building WeCare.DbMigrator..." -ForegroundColor Yellow
dotnet build src\WeCare.DbMigrator\WeCare.DbMigrator.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Error "DbMigrator build failed!"
    exit $LASTEXITCODE
}

# 3. Execute the DbMigrator to apply schema and seeds
Write-Host "Step 3: Running DbMigrator (Schema Update & Data Seeding)..." -ForegroundColor Yellow
Push-Location src\WeCare.DbMigrator
dotnet run
Pop-Location

if ($LASTEXITCODE -ne 0) {
    Write-Error "DbMigrator execution failed!"
    exit $LASTEXITCODE
}

Write-Host "WeCare Database is up-to-date and populated!" -ForegroundColor Green
