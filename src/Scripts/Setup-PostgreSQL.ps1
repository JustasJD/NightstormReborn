# Nightstorm RPG - PostgreSQL Docker Setup Script
# This script creates and configures a PostgreSQL database in Docker

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

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Yellow
try {
    # Try multiple methods to detect Docker
    $dockerInfo = docker info 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        throw "Docker command failed"
    }
    
    Write-Host "? Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "? Docker is not running or not accessible" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Ensure Docker Desktop is running (check system tray)" -ForegroundColor White
    Write-Host "2. Wait 30-60 seconds for Docker to fully start" -ForegroundColor White
    Write-Host "3. Restart Docker Desktop if needed" -ForegroundColor White
    Write-Host "4. Try opening a new PowerShell window" -ForegroundColor White
    Write-Host "5. Verify Docker CLI works: docker --version" -ForegroundColor White
    Write-Host ""
    Write-Host "Error details: $($_.Exception.Message)" -ForegroundColor Gray
    exit 1
}

# Check if container already exists
Write-Host ""
Write-Host "Checking for existing container..." -ForegroundColor Yellow
$existingContainer = docker ps -a --filter "name=$CONTAINER_NAME" --format "{{.Names}}"

if ($existingContainer -eq $CONTAINER_NAME) {
    Write-Host "Container '$CONTAINER_NAME' already exists" -ForegroundColor Yellow
    $response = Read-Host "Do you want to remove and recreate it? (y/N)"
    
    if ($response -eq 'y' -or $response -eq 'Y') {
        Write-Host "Stopping container..." -ForegroundColor Yellow
        docker stop $CONTAINER_NAME | Out-Null
        
        Write-Host "Removing container..." -ForegroundColor Yellow
        docker rm $CONTAINER_NAME | Out-Null
        
        Write-Host "? Container removed" -ForegroundColor Green
    }
    else {
        Write-Host "Keeping existing container" -ForegroundColor Yellow
        
        # Check if container is running
        $runningContainer = docker ps --filter "name=$CONTAINER_NAME" --format "{{.Names}}"
        if ($runningContainer -ne $CONTAINER_NAME) {
            Write-Host "Starting existing container..." -ForegroundColor Yellow
            docker start $CONTAINER_NAME | Out-Null
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
$existingVolume = docker volume ls --filter "name=$VOLUME_NAME" --format "{{.Name}}"

if ($existingVolume -eq $VOLUME_NAME) {
    Write-Host "? Volume '$VOLUME_NAME' already exists" -ForegroundColor Green
}
else {
    docker volume create $VOLUME_NAME | Out-Null
    Write-Host "? Volume created" -ForegroundColor Green
}

# Pull PostgreSQL image
Write-Host ""
Write-Host "Pulling PostgreSQL image..." -ForegroundColor Yellow
docker pull postgres:$POSTGRES_VERSION

# Create and start container
Write-Host ""
Write-Host "Creating PostgreSQL container..." -ForegroundColor Yellow
docker run -d `
    --name $CONTAINER_NAME `
    -e POSTGRES_USER=$POSTGRES_USER `
    -e POSTGRES_PASSWORD=$POSTGRES_PASSWORD `
    -e POSTGRES_DB=$POSTGRES_DB `
    -p "${POSTGRES_PORT}:5432" `
    -v "${VOLUME_NAME}:/var/lib/postgresql/data" `
    --restart unless-stopped `
    postgres:$POSTGRES_VERSION

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Container created successfully" -ForegroundColor Green
}
else {
    Write-Host "? Failed to create container" -ForegroundColor Red
    exit 1
}

# Wait for PostgreSQL to be ready
Write-Host ""
Write-Host "Waiting for PostgreSQL to be ready..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0

while ($attempt -lt $maxAttempts) {
    $attempt++
    
    $ready = docker exec $CONTAINER_NAME pg_isready -U $POSTGRES_USER 2>&1
    
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

# Create additional database user (optional, for security)
Write-Host ""
Write-Host "Setting up database..." -ForegroundColor Yellow

# Verify database exists
$dbCheck = docker exec $CONTAINER_NAME psql -U $POSTGRES_USER -lqt 2>&1 | Select-String -Pattern $POSTGRES_DB

if ($dbCheck) {
    Write-Host "? Database '$POSTGRES_DB' is ready" -ForegroundColor Green
}
else {
    Write-Host "? Database not found, but container is running" -ForegroundColor Yellow
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
Write-Host "Useful Commands:" -ForegroundColor Yellow
Write-Host "  Stop:    docker stop $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Start:   docker start $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Restart: docker restart $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Logs:    docker logs $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Shell:   docker exec -it $CONTAINER_NAME psql -U $POSTGRES_USER -d $POSTGRES_DB" -ForegroundColor Gray
Write-Host "  Remove:  docker stop $CONTAINER_NAME && docker rm $CONTAINER_NAME" -ForegroundColor Gray
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Update src/Nightstorm.Bot/appsettings.json with the connection string above" -ForegroundColor White
Write-Host "2. Run database migrations:" -ForegroundColor White
Write-Host "   dotnet ef database update --project src/Nightstorm.Data --startup-project src/Nightstorm.Bot" -ForegroundColor Gray
Write-Host ""
