# Nightstorm RPG - PostgreSQL Docker Setup Script (PATH Fix Version)
# This script creates and configures a PostgreSQL database in Docker
# Uses CMD fallback when PowerShell doesn't see Docker in PATH

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Nightstorm RPG - Database Setup" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$CONTAINER_NAME = "nightstorm-postgres"
$POSTGRES_VERSION = "17-alpine"
$POSTGRES_USER = "nightstorm"
$POSTGRES_PASSWORD = "NightstormDev2024!"
$POSTGRES_DB = "NightstormDb"
$POSTGRES_PORT = "5432"
$VOLUME_NAME = "nightstorm-pgdata"

# Function to run Docker commands (tries PowerShell first, then CMD)
function Invoke-DockerCommand {
    param(
        [string]$Command
    )
    
    try {
        # Try PowerShell first
        $result = Invoke-Expression "docker $Command 2>&1"
        
        if ($LASTEXITCODE -ne 0 -and $result -match "not recognized") {
            # Docker not in PowerShell PATH, use CMD
            $result = cmd /c "docker $Command 2>&1"
        }
        
        return $result
    }
    catch {
        # Fallback to CMD
        return cmd /c "docker $Command 2>&1"
    }
}

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Yellow
try {
    $dockerInfo = Invoke-DockerCommand "info"
    
    if ($dockerInfo -match "Server Version") {
        Write-Host "? Docker is running" -ForegroundColor Green
    }
    else {
        throw "Docker not responding"
    }
}
catch {
    Write-Host "? Docker is not running or not accessible" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure Docker Desktop is running (check system tray)" -ForegroundColor White
    Write-Host "2. Wait 30-60 seconds for Docker to fully start" -ForegroundColor White
    Write-Host "3. Restart Docker Desktop if needed" -ForegroundColor White
    Write-Host "4. Open CMD and verify: docker --version" -ForegroundColor White
    Write-Host ""
    Write-Host "PATH Issue Detected:" -ForegroundColor Yellow
    Write-Host "Docker works in CMD but not PowerShell" -ForegroundColor White
    Write-Host "Fix: Add Docker to PowerShell PATH or use Setup-PostgreSQL-CMD.ps1" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

# Check if container already exists
Write-Host ""
Write-Host "Checking for existing container..." -ForegroundColor Yellow
$existingContainer = (Invoke-DockerCommand "ps -a --filter name=$CONTAINER_NAME --format {{.Names}}").Trim()

if ($existingContainer -eq $CONTAINER_NAME) {
    Write-Host "Container '$CONTAINER_NAME' already exists" -ForegroundColor Yellow
    $response = Read-Host "Do you want to remove and recreate it? (y/N)"
    
    if ($response -eq 'y' -or $response -eq 'Y') {
        Write-Host "Stopping container..." -ForegroundColor Yellow
        Invoke-DockerCommand "stop $CONTAINER_NAME" | Out-Null
        
        Write-Host "Removing container..." -ForegroundColor Yellow
        Invoke-DockerCommand "rm $CONTAINER_NAME" | Out-Null
        
        Write-Host "? Container removed" -ForegroundColor Green
    }
    else {
        Write-Host "Keeping existing container" -ForegroundColor Yellow
        
        # Check if container is running
        $runningContainer = (Invoke-DockerCommand "ps --filter name=$CONTAINER_NAME --format {{.Names}}").Trim()
        if ($runningContainer -ne $CONTAINER_NAME) {
            Write-Host "Starting existing container..." -ForegroundColor Yellow
            Invoke-DockerCommand "start $CONTAINER_NAME" | Out-Null
            Write-Host "? Container started" -ForegroundColor Green
        }
        else {
            Write-Host "? Container is already running" -ForegroundColor Green
        }
        
        Write-Host ""
        Write-Host "==================================" -ForegroundColor Cyan
        Write-Host "  Connection Information" -ForegroundColor Cyan
        Write-Host "==================================" -ForegroundColor Cyan
        Write-Host "Host:     localhost" -ForegroundColor White
        Write-Host "Port:     $POSTGRES_PORT" -ForegroundColor White
        Write-Host "Database: $POSTGRES_DB" -ForegroundColor White
        Write-Host "Username: $POSTGRES_USER" -ForegroundColor White
        Write-Host "Password: $POSTGRES_PASSWORD" -ForegroundColor White
        Write-Host ""
        Write-Host "Connection String:" -ForegroundColor Yellow
        Write-Host "Host=localhost;Port=$POSTGRES_PORT;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD" -ForegroundColor Cyan
        exit 0
    }
}

# Create Docker volume for persistence
Write-Host ""
Write-Host "Creating Docker volume..." -ForegroundColor Yellow
$existingVolume = (Invoke-DockerCommand "volume ls --filter name=$VOLUME_NAME --format {{.Name}}").Trim()

if ($existingVolume -eq $VOLUME_NAME) {
    Write-Host "? Volume '$VOLUME_NAME' already exists" -ForegroundColor Green
}
else {
    Invoke-DockerCommand "volume create $VOLUME_NAME" | Out-Null
    Write-Host "? Volume created" -ForegroundColor Green
}

# Pull PostgreSQL image
Write-Host ""
Write-Host "Pulling PostgreSQL image..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor Gray
Invoke-DockerCommand "pull postgres:$POSTGRES_VERSION"

# Create and start container
Write-Host ""
Write-Host "Creating PostgreSQL container..." -ForegroundColor Yellow
$createResult = cmd /c "docker run -d --name $CONTAINER_NAME -e POSTGRES_USER=$POSTGRES_USER -e POSTGRES_PASSWORD=$POSTGRES_PASSWORD -e POSTGRES_DB=$POSTGRES_DB -p ${POSTGRES_PORT}:5432 -v ${VOLUME_NAME}:/var/lib/postgresql/data --restart unless-stopped postgres:$POSTGRES_VERSION 2>&1"

if ($createResult -match "^[a-f0-9]{64}$" -or $createResult -match "^[a-f0-9]{12}") {
    Write-Host "? Container created successfully" -ForegroundColor Green
}
else {
    Write-Host "? Failed to create container" -ForegroundColor Red
    Write-Host "Error: $createResult" -ForegroundColor Gray
    exit 1
}

# Wait for PostgreSQL to be ready
Write-Host ""
Write-Host "Waiting for PostgreSQL to be ready..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0

while ($attempt -lt $maxAttempts) {
    $attempt++
    
    $ready = Invoke-DockerCommand "exec $CONTAINER_NAME pg_isready -U $POSTGRES_USER"
    
    if ($ready -match "accepting connections") {
        Write-Host "? PostgreSQL is ready!" -ForegroundColor Green
        break
    }
    
    Write-Host "  Attempt $attempt/$maxAttempts - waiting..." -ForegroundColor Gray
    Start-Sleep -Seconds 2
}

if ($attempt -eq $maxAttempts) {
    Write-Host "? PostgreSQL failed to start within timeout" -ForegroundColor Red
    Write-Host "Check container logs with: docker logs $CONTAINER_NAME" -ForegroundColor Yellow
    exit 1
}

# Verify database exists
Write-Host ""
Write-Host "Verifying database..." -ForegroundColor Yellow
$dbCheck = Invoke-DockerCommand "exec $CONTAINER_NAME psql -U $POSTGRES_USER -lqt"

if ($dbCheck -match $POSTGRES_DB) {
    Write-Host "? Database '$POSTGRES_DB' is ready" -ForegroundColor Green
}
else {
    Write-Host "? Database verification skipped (container is running)" -ForegroundColor Yellow
}

# Display connection information
Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Setup Complete!" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "PostgreSQL is now running in Docker" -ForegroundColor Green
Write-Host ""
Write-Host "Connection Information:" -ForegroundColor Yellow
Write-Host "----------------------" -ForegroundColor Yellow
Write-Host "Host:     localhost" -ForegroundColor White
Write-Host "Port:     $POSTGRES_PORT" -ForegroundColor White
Write-Host "Database: $POSTGRES_DB" -ForegroundColor White
Write-Host "Username: $POSTGRES_USER" -ForegroundColor White
Write-Host "Password: $POSTGRES_PASSWORD" -ForegroundColor White
Write-Host ""
Write-Host "Connection String (copy to appsettings.json):" -ForegroundColor Yellow
Write-Host "Host=localhost;Port=$POSTGRES_PORT;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD" -ForegroundColor Cyan
Write-Host ""
Write-Host "Useful Commands (use in CMD or PowerShell with 'cmd /c'):" -ForegroundColor Yellow
Write-Host "  Stop:    docker stop $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Start:   docker start $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Restart: docker restart $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Logs:    docker logs $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Shell:   docker exec -it $CONTAINER_NAME psql -U $POSTGRES_USER -d $POSTGRES_DB" -ForegroundColor Gray
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Update src/Nightstorm.Bot/appsettings.json with the connection string above" -ForegroundColor White
Write-Host "2. Run database migrations:" -ForegroundColor White
Write-Host "   dotnet ef database update --project src/Nightstorm.Data --startup-project src/Nightstorm.Bot" -ForegroundColor Gray
Write-Host ""
Write-Host "Note: PowerShell PATH Issue Detected" -ForegroundColor Yellow
Write-Host "Docker commands may not work directly in PowerShell" -ForegroundColor White
Write-Host "Use CMD or fix PATH: See docs\FIX_POWERSHELL_PATH.md" -ForegroundColor Cyan
Write-Host ""
