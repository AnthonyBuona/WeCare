<#
.SYNOPSIS
    Automates unlocking and setting exclusions for the WeCare project folder.
.DESCRIPTION
    Recursively unblocks all project files and registers the WeCare directory 
    as a Windows Defender exclusion path to prevent execution blocks.
    Requires Administrator privileges to add Defender exclusions.
.EXAMPLE
    Run inside an Administrator PowerShell terminal:
    powershell -ExecutionPolicy Bypass .\tools\Unblock-Project.ps1
#>
$ErrorActionPreference = "Stop"

$projectPath = "C:\projects\WeCare"

Write-Host "🔓 Step 1: Recursively unblocking all files in WeCare..." -ForegroundColor Yellow
Get-ChildItem -Path $projectPath -Recurse | Unblock-File
Write-Host "✅ All files unblocked successfully!" -ForegroundColor Green

Write-Host "🛡️ Step 2: Registering WeCare folder in Windows Defender exclusions..." -ForegroundColor Yellow
try {
    # Check if running as Admin
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    if ($principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        Add-MpPreference -ExclusionPath $projectPath
        Write-Host "✅ Windows Defender exclusion successfully added for '$projectPath'!" -ForegroundColor Green
    } else {
        Write-Warning "⚠️ Administrator privileges are required to configure Windows Defender exclusions."
        Write-Warning "Please re-run this script in an Administrator terminal."
    }
} catch {
    Write-Error "❌ Failed to configure Windows Defender exclusions: $_"
}

Write-Host "🎉 WeCare project environment is ready and unlocked!" -ForegroundColor Green
