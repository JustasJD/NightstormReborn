# Fix PowerShell PATH for Docker
# Run this script as Administrator

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Docker PATH Fix for PowerShell" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "✗ This script must be run as Administrator" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please:" -ForegroundColor Yellow
    Write-Host "1. Close this PowerShell window" -ForegroundColor White
    Write-Host "2. Right-click PowerShell and select 'Run as Administrator'" -ForegroundColor White
    Write-Host "3. Run this script again" -ForegroundColor White
    exit 1
}

# Docker path location
$dockerPath = "C:\Program Files\Docker\Docker\resources\bin"

Write-Host "Checking Docker installation..." -ForegroundColor Yellow

# Verify Docker exists at expected location
if (Test-Path "$dockerPath\docker.exe") {
    Write-Host "✓ Docker found at $dockerPath" -ForegroundColor Green
} else {
    Write-Host "✗ Docker not found at expected location" -ForegroundColor Red
    Write-Host ""
    Write-Host "Searching for docker.exe..." -ForegroundColor Yellow
    
    # Try to find Docker
    $dockerLocations = @(
        "C:\Program Files\Docker\Docker\resources\bin",
        "C:\Program Files\Docker\Docker\resources",
        "C:\ProgramData\DockerDesktop\version-bin"
    )
    
    $foundDocker = $false
    foreach ($location in $dockerLocations) {
        if (Test-Path "$location\docker.exe") {
            $dockerPath = $location
            $foundDocker = $true
            Write-Host "✓ Found Docker at $dockerPath" -ForegroundColor Green
            break
        }
    }
    
    if (-not $foundDocker) {
        Write-Host "✗ Could not locate docker.exe" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please ensure Docker Desktop is installed" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host ""
Write-Host "Updating System PATH..." -ForegroundColor Yellow

# Get current system PATH
$currentPath = [Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::Machine)

# Split PATH into array
$pathArray = $currentPath -split ';'

# Remove any problematic System32 Docker entries
$pathArray = $pathArray | Where-Object { $_ -notlike '*System32*docker*' }

# Check if Docker bin is already in PATH
$dockerPathExists = $pathArray | Where-Object { $_ -eq $dockerPath }

if ($dockerPathExists) {
    Write-Host "✓ Docker is already in System PATH" -ForegroundColor Green
} else {
    # Add Docker path at the beginning
    $pathArray = @($dockerPath) + $pathArray
    Write-Host "✓ Docker added to System PATH" -ForegroundColor Green
}

# Rebuild and set PATH
$newPath = ($pathArray | Where-Object { $_ -ne '' }) -join ';'
[Environment]::SetEnvironmentVariable("Path", $newPath, [EnvironmentVariableTarget]::Machine)

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  PATH Fix Complete!" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✓ Docker has been added to your System PATH" -ForegroundColor Green
Write-Host "✓ Removed problematic System32 Docker entries" -ForegroundColor Green
Write-Host ""
Write-Host "⚠ IMPORTANT: Close and reopen PowerShell for changes to take effect" -ForegroundColor Yellow
Write-Host ""
Write-Host "After reopening PowerShell, verify the fix with:" -ForegroundColor White
Write-Host "  docker --version" -ForegroundColor Cyan
Write-Host ""