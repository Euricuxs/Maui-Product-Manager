# ============================================
# MAUI Product Manager - Build & Deploy Script
# ============================================
# Usage: .\build-android.ps1
#
# What it does:
#   1. Builds the MAUI project for Android
#   2. Uninstalls the existing app from device
#   3. Installs the new APK to device
#   4. Launches the app
# ============================================

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [string]$TargetFramework = "net10.0-android",

    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
$ProjectDir = $PSScriptRoot
$ProjectName = "MauiProductManager"

# Colors
function Write-Step { param([string]$Msg) Write-Host "[BUILD] $Msg" -ForegroundColor Cyan }
function Write-Success { param([string]$Msg) Write-Host "[OK] $Msg" -ForegroundColor Green }
function Write-Error { param([string]$Msg) Write-Host "[ERROR] $Msg" -ForegroundColor Red }
function Write-Info { param([string]$Msg) Write-Host "  $Msg" }

# Detect Android SDK path
$AndroidSdkPath = $env:ANDROID_HOME
if (-not $AndroidSdkPath) {
    $AndroidSdkPath = "C:\Users\Eurico\AppData\Local\Android\Sdk"
}

$AdbPath = Join-Path $AndroidSdkPath "platform-tools\adb.exe"
$PackageName = "com.companyname.mauiproductmanager"

# Resolve project path
$CsprojPath = Join-Path $ProjectDir "$ProjectName.csproj"
if (-not (Test-Path $CsprojPath)) {
    Write-Error "Project file not found: $CsprojPath"
    exit 1
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Magenta
Write-Host "  MAUI Android Build & Deploy" -ForegroundColor Magenta
Write-Host "  Configuration: $Configuration" -ForegroundColor Magenta
Write-Host "=========================================" -ForegroundColor Magenta
Write-Host ""

# Step 1: Build
if (-not $SkipBuild) {
    Write-Step "Building $ProjectName ($Configuration)..."
    $buildResult = dotnet build $CsprojPath -c $Configuration -f $TargetFramework --nologo -v q 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed!"
        Write-Host $buildResult
        exit 1
    }

    Write-Success "Build successful"
} else {
    Write-Step "Skipping build (using existing binaries)"
}

# Step 2: Find APK
$BinDir = Join-Path $ProjectDir "bin\$Configuration\$TargetFramework"
$ApkPath = $null

# Try signed APK first, then unsigned
foreach ($apk in @("$PackageName-Signed.apk", "$PackageName.apk")) {
    $test = Join-Path $BinDir $apk
    if (Test-Path $test) {
        $ApkPath = $test
        break
    }
}

if (-not $ApkPath) {
    Write-Error "APK not found in: $BinDir"
    Write-Info "Available files:"
    Get-ChildItem $BinDir -Filter "*.apk" | ForEach-Object { Write-Info "  $($_.Name)" }
    exit 1
}

$ApkSize = [math]::Round((Get-Item $ApkPath).Length / 1MB, 2)
Write-Step "APK: $ApkPath ($ApkSize MB)"

# Step 3: Check device connection
Write-Step "Checking device connection..."
$deviceCheck = & $AdbPath devices 2>&1
$deviceCount = ($deviceCheck | Select-String "device$").Count

if ($deviceCount -eq 0) {
    Write-Error "No Android device connected!"
    Write-Host "Please connect your device with USB debugging enabled."
    exit 1
}

Write-Success "Device connected ($deviceCount device(s))"

# Step 4: Uninstall existing app
Write-Step "Uninstalling existing app..."
$uninstallResult = & $AdbPath uninstall $PackageName 2>&1
if ($uninstallResult -match "Success") {
    Write-Info "Previous app removed"
} else {
    Write-Info "App was not installed (OK)"
}

# Step 5: Install new APK
Write-Step "Installing APK..."
$installResult = & $AdbPath install -r $ApkPath 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Install failed: $installResult"
    exit 1
}
Write-Success "APK installed"

# Step 6: Launch app
Write-Step "Launching app..."
$activity = "crc642cb1018e68ca6182.MainActivity"
$launchResult = & $AdbPath shell am start -n "$PackageName/$activity" 2>&1
Write-Success "App started!"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  Done! App is running on your device." -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "To rebuild after code changes, run this script again:"
Write-Host "  .\build-android.ps1" -ForegroundColor Yellow
Write-Host ""
Write-Host "Or for continuous development, use:"
Write-Host "  dotnet run -f $TargetFramework" -ForegroundColor Yellow
Write-Host ""
