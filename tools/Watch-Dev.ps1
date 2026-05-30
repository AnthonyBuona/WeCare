<#
.SYNOPSIS
    Starts the WeCare Web Application in watch/dev mode with client-side libraries.
.DESCRIPTION
    Ensures all NPM packages are installed, client-side libraries are compiled,
    and runs dotnet watch on WeCare.Web for rapid local development.
.EXAMPLE
    .\tools\Watch-Dev.ps1
#>
$ErrorActionPreference = "Stop"

Write-Host "Starting WeCare Dev Server..." -ForegroundColor Cyan

# 1. Install Client Libraries using ABP CLI if needed
Write-Host "Step 1: Installing ABP Client Libraries..." -ForegroundColor Yellow
abp install-libs
if ($LASTEXITCODE -ne 0) {
    Write-Warning "abp install-libs returned an exit code. Continuing anyway..."
}

# 2. Run the Web App with Hot Reload (dotnet watch)
Write-Host "Step 2: Running WeCare.Web in Watch Mode (https://localhost:44373/)..." -ForegroundColor Yellow
dotnet watch --project src\WeCare.Web\WeCare.Web.csproj run
