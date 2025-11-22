# Nightstorm RPG - PostgreSQL Status Check
# This script shows the current status of the PostgreSQL container

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Nightstorm RPG - Database Status" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$CONTAINER_NAME = "nightstorm-postgres"
$VOLUME_NAME = "nightstorm-pgdata"
$POSTGRES_USER = "nightstorm"
$POSTGRES_DB = "NightstormDb"

# Check Docker
Write-Host "Docker Status:" -ForegroundColor Yellow
try {
    docker version --format "  Version: {{.Server.Version}}" | Out-Null
    Write-Host "  ? Docker is running" -ForegroundColor Green
}
catch {
    Write-Host "  ? Docker is not running" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Check Container
Write-Host "Container Status:" -ForegroundColor Yellow
$containerStatus = docker ps -a --filter "name=$CONTAINER_NAME" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

if ($containerStatus -match $CONTAINER_NAME) {
    $containerStatus | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
    
    # Check if running
    $running = docker ps --filter "name=$CONTAINER_NAME" --format "{{.Names}}"
    if ($running -eq $CONTAINER_NAME) {
        Write-Host "  ? Container is running" -ForegroundColor Green
        
        # Check database connection
        Write-Host ""
        Write-Host "Database Health:" -ForegroundColor Yellow
        $healthCheck = docker exec $CONTAINER_NAME pg_isready -U $POSTGRES_USER 2>&1
        
        if ($healthCheck -match "accepting connections") {
            Write-Host "  ? Database is accepting connections" -ForegroundColor Green
            
            # Get database size
            $dbSize = docker exec $CONTAINER_NAME psql -U $POSTGRES_USER -d $POSTGRES_DB -t -c "SELECT pg_size_pretty(pg_database_size('$POSTGRES_DB'));" 2>&1
            if ($dbSize) {
                Write-Host "  Database size: $($dbSize.Trim())" -ForegroundColor White
            }
            
            # Count tables
            $tableCount = docker exec $CONTAINER_NAME psql -U $POSTGRES_USER -d $POSTGRES_DB -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';" 2>&1
            if ($tableCount) {
                Write-Host "  Tables: $($tableCount.Trim())" -ForegroundColor White
            }
        }
        else {
            Write-Host "  ? Database is not responding" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  ? Container exists but is not running" -ForegroundColor Yellow
        Write-Host "  To start: docker start $CONTAINER_NAME" -ForegroundColor Cyan
    }
}
else {
    Write-Host "  ? Container '$CONTAINER_NAME' not found" -ForegroundColor Red
    Write-Host "  To create: .\Setup-PostgreSQL.ps1" -ForegroundColor Cyan
}

Write-Host ""

# Check Volume
Write-Host "Data Volume:" -ForegroundColor Yellow
$volumeExists = docker volume ls --filter "name=$VOLUME_NAME" --format "{{.Name}}"

if ($volumeExists -eq $VOLUME_NAME) {
    Write-Host "  ? Volume '$VOLUME_NAME' exists" -ForegroundColor Green
    
    # Get volume size (approximate)
    $volumeInfo = docker volume inspect $VOLUME_NAME | ConvertFrom-Json
    $mountpoint = $volumeInfo[0].Mountpoint
    Write-Host "  Mountpoint: $mountpoint" -ForegroundColor Gray
}
else {
    Write-Host "  ? Volume not found" -ForegroundColor Red
}

Write-Host ""

# Connection info
if ($running -eq $CONTAINER_NAME) {
    Write-Host "Connection Information:" -ForegroundColor Yellow
    Write-Host "  Host:     localhost" -ForegroundColor White
    Write-Host "  Port:     5432" -ForegroundColor White
    Write-Host "  Database: $POSTGRES_DB" -ForegroundColor White
    Write-Host "  Username: $POSTGRES_USER" -ForegroundColor White
    Write-Host "  Password: NightstormDev2024!" -ForegroundColor White
    Write-Host ""
    Write-Host "  Connection String:" -ForegroundColor Cyan
    Write-Host "  Host=localhost;Port=5432;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=NightstormDev2024!" -ForegroundColor Gray
    Write-Host ""
}

# Quick commands
Write-Host "Quick Commands:" -ForegroundColor Yellow
Write-Host "  Connect:  docker exec -it $CONTAINER_NAME psql -U $POSTGRES_USER -d $POSTGRES_DB" -ForegroundColor Gray
Write-Host "  Logs:     docker logs $CONTAINER_NAME" -ForegroundColor Gray
Write-Host "  Restart:  docker restart $CONTAINER_NAME" -ForegroundColor Gray
Write-Host ""
