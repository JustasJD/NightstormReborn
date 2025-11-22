# Nightstorm RPG - Docker Diagnostics
# This script helps diagnose Docker connectivity issues

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Docker Diagnostics" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Docker command exists
Write-Host "Test 1: Checking if Docker CLI is available..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version 2>&1
    Write-Host "  ? Docker CLI found: $dockerVersion" -ForegroundColor Green
}
catch {
    Write-Host "  ? Docker CLI not found in PATH" -ForegroundColor Red
    Write-Host "  Solution: Reinstall Docker Desktop or add Docker to PATH" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Test 2: Docker daemon responding
Write-Host "Test 2: Checking if Docker daemon is responding..." -ForegroundColor Yellow
try {
    $dockerInfo = docker info 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Docker daemon is responding" -ForegroundColor Green
    }
    else {
        throw "Docker daemon not responding"
    }
}
catch {
    Write-Host "  ? Docker daemon is not responding" -ForegroundColor Red
    Write-Host "  Error: $dockerInfo" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Possible causes:" -ForegroundColor Yellow
    Write-Host "  - Docker Desktop is not running" -ForegroundColor White
    Write-Host "  - Docker is still starting up (wait 30-60 seconds)" -ForegroundColor White
    Write-Host "  - Docker service crashed" -ForegroundColor White
    Write-Host "  - Hyper-V or WSL2 issues" -ForegroundColor White
    Write-Host ""
    Write-Host "  Solutions:" -ForegroundColor Yellow
    Write-Host "  1. Open Docker Desktop from Start menu" -ForegroundColor White
    Write-Host "  2. Wait for Docker Desktop icon in system tray to show 'Docker Desktop is running'" -ForegroundColor White
    Write-Host "  3. Try: Restart-Service docker (requires admin)" -ForegroundColor White
    Write-Host "  4. Try: Restart Docker Desktop" -ForegroundColor White
    Write-Host "  5. Check Windows Services: Docker Desktop Service should be running" -ForegroundColor White
    exit 1
}

Write-Host ""

# Test 3: Can list containers
Write-Host "Test 3: Testing container operations..." -ForegroundColor Yellow
try {
    $containers = docker ps -a 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Can list containers" -ForegroundColor Green
        
        $count = (docker ps -a --format "{{.Names}}" | Measure-Object).Count
        Write-Host "  Found $count container(s)" -ForegroundColor Gray
    }
    else {
        throw "Cannot list containers"
    }
}
catch {
    Write-Host "  ? Cannot list containers" -ForegroundColor Red
    Write-Host "  Error: $containers" -ForegroundColor Gray
}

Write-Host ""

# Test 4: Can list volumes
Write-Host "Test 4: Testing volume operations..." -ForegroundColor Yellow
try {
    $volumes = docker volume ls 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Can list volumes" -ForegroundColor Green
        
        $count = (docker volume ls --format "{{.Name}}" | Measure-Object).Count
        Write-Host "  Found $count volume(s)" -ForegroundColor Gray
    }
    else {
        throw "Cannot list volumes"
    }
}
catch {
    Write-Host "  ? Cannot list volumes" -ForegroundColor Red
    Write-Host "  Error: $volumes" -ForegroundColor Gray
}

Write-Host ""

# Test 5: Check for Nightstorm containers
Write-Host "Test 5: Checking for Nightstorm containers..." -ForegroundColor Yellow
$nightstormContainers = docker ps -a --filter "name=nightstorm" --format "{{.Names}}" 2>&1

if ($LASTEXITCODE -eq 0) {
    if ($nightstormContainers) {
        Write-Host "  ? Found Nightstorm container(s):" -ForegroundColor Green
        $nightstormContainers | ForEach-Object { Write-Host "    - $_" -ForegroundColor White }
    }
    else {
        Write-Host "  ? No Nightstorm containers found" -ForegroundColor Gray
    }
}

Write-Host ""

# System Information
Write-Host "System Information:" -ForegroundColor Yellow
Write-Host "  PowerShell Version: $($PSVersionTable.PSVersion)" -ForegroundColor White
Write-Host "  OS: $([System.Environment]::OSVersion.VersionString)" -ForegroundColor White
Write-Host "  User: $env:USERNAME" -ForegroundColor White
Write-Host "  Admin: $(([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator))" -ForegroundColor White

Write-Host ""

# Docker Information
Write-Host "Docker Information:" -ForegroundColor Yellow
try {
    $dockerInfo = docker info --format "{{json .}}" 2>&1 | ConvertFrom-Json
    Write-Host "  Docker Version: $($dockerInfo.ServerVersion)" -ForegroundColor White
    Write-Host "  Operating System: $($dockerInfo.OperatingSystem)" -ForegroundColor White
    Write-Host "  Total Memory: $([math]::Round($dockerInfo.MemTotal / 1GB, 2)) GB" -ForegroundColor White
    Write-Host "  CPUs: $($dockerInfo.NCPU)" -ForegroundColor White
}
catch {
    Write-Host "  Could not retrieve Docker info" -ForegroundColor Gray
}

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Diagnostics Complete" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

if ($LASTEXITCODE -eq 0) {
    Write-Host "? All tests passed! Docker is working correctly." -ForegroundColor Green
    Write-Host "You can now run: .\Setup-PostgreSQL.ps1" -ForegroundColor Cyan
}
else {
    Write-Host "? Some tests failed. Please fix the issues above." -ForegroundColor Yellow
}

Write-Host ""
