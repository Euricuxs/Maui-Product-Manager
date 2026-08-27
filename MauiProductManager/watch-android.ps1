# ============================================
# MAUI Product Manager - Hot Reload Script
# ============================================
# Usage: .\watch-android.ps1
#
# Watches for code changes and automatically:
#   1. Rebuilds the project
#   2. Pushes assemblies to device
#   3. Triggers XAML hot reload
# ============================================

param(
    [string]$TargetFramework = "net10.0-android",

    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$ProjectDir = $PSScriptRoot
$ProjectName = "MauiProductManager"

$AndroidSdkPath = $env:ANDROID_HOME
if (-not $AndroidSdkPath) {
    $AndroidSdkPath = "C:\Users\Eurico\AppData\Local\Android\Sdk"
}

$AdbPath = Join-Path $AndroidSdkPath "platform-tools\adb.exe"
$PackageName = "com.companyname.mauiproductmanager"
$Activity = "crc642cb1018e68ca6182.MainActivity"

function Write-Step { param([string]$Msg) Write-Host "[$(Get-Date -Format 'HH:mm:ss')] $Msg" -ForegroundColor Cyan }

Write-Host ""
Write-Host "=========================================" -ForegroundColor Magenta
Write-Host "  MAUI Hot Reload - Android" -ForegroundColor Magenta
Write-Host "=========================================" -ForegroundColor Magenta
Write-Host ""

# Check device
Write-Step "Checking device..."
$deviceCheck = & $AdbPath devices 2>&1 | Select-String "device$"
if ($deviceCheck.Count -eq 0) {
    Write-Host "[ERROR] No device connected!" -ForegroundColor Red
    exit 1
}
Write-Host "  Device: $(($deviceCheck -split '\s+')[0])" -ForegroundColor Green

# Build once initially
Write-Step "Initial build..."
dotnet build (Join-Path $ProjectDir "$ProjectName.csproj") -c $Configuration -f $TargetFramework --no-restore -v q
if ($LASTEXITCODE -ne 0) { exit 1 }

# Uninstall + Install
Write-Step "Installing to device..."
& $AdbPath uninstall $PackageName 2>&1 | Out-Null
& $AdbPath install -r (Join-Path $ProjectDir "bin\$Configuration\$TargetFramework\$PackageName-Signed.apk") 2>&1 | Out-Null

# Launch
Write-Step "Launching app..."
& $AdbPath shell am start -n "$PackageName/$Activity" 2>&1 | Out-Null

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  Hot reload active!" -ForegroundColor Green
Write-Host "  Edit XAML/C# files and save to reload." -ForegroundColor Green
Write-Host "  Press Ctrl+C to stop." -ForegroundColor Yellow
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

# Use dotnet watch for hot reload
$watchArgs = @(
    "watch",
    "run",
    "--no-build",
    "-c", $Configuration,
    "-f", $TargetFramework,
    "--",
    "--package", $PackageName,
    "--activity", $Activity
)

dotnet @watchArgs
