# Nightstorm RPG - PostgreSQL Cleanup Script
# This script stops and removes the PostgreSQL container

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Nightstorm RPG - Database Cleanup" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$CONTAINER_NAME = "nightstorm-postgres"
$VOLUME_NAME = "nightstorm-pgdata"

# Check if container exists
$existingContainer = docker ps -a --filter "name=$CONTAINER_NAME" --format "{{.Names}}"

if ($existingContainer -ne $CONTAINER_NAME) {
    Write-Host "? Container '$CONTAINER_NAME' does not exist" -ForegroundColor Green
    Write-Host "Nothing to clean up" -ForegroundColor Yellow
    exit 0
}

Write-Host "Found container: $CONTAINER_NAME" -ForegroundColor Yellow
Write-Host ""
Write-Host "What would you like to do?" -ForegroundColor Yellow
Write-Host "1. Stop container (keep data)" -ForegroundColor White
Write-Host "2. Stop and remove container (keep data volume)" -ForegroundColor White
Write-Host "3. Stop, remove container AND delete all data (WARNING: PERMANENT)" -ForegroundColor Red
Write-Host "4. Cancel" -ForegroundColor Gray
Write-Host ""

$choice = Read-Host "Enter choice (1-4)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "Stopping container..." -ForegroundColor Yellow
        docker stop $CONTAINER_NAME
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "? Container stopped" -ForegroundColor Green
            Write-Host ""
            Write-Host "To start it again, run: docker start $CONTAINER_NAME" -ForegroundColor Cyan
        }
        else {
            Write-Host "? Failed to stop container" -ForegroundColor Red
        }
    }
    
    "2" {
        Write-Host ""
        Write-Host "Stopping container..." -ForegroundColor Yellow
        docker stop $CONTAINER_NAME | Out-Null
        
        Write-Host "Removing container..." -ForegroundColor Yellow
        docker rm $CONTAINER_NAME
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "? Container removed (data volume preserved)" -ForegroundColor Green
            Write-Host ""
            Write-Host "To recreate with same data, run: .\Setup-PostgreSQL.ps1" -ForegroundColor Cyan
        }
        else {
            Write-Host "? Failed to remove container" -ForegroundColor Red
        }
    }
    
    "3" {
        Write-Host ""
        Write-Host "??  WARNING: This will PERMANENTLY delete all database data!" -ForegroundColor Red
        $confirm = Read-Host "Type 'DELETE' to confirm"
        
        if ($confirm -eq "DELETE") {
            Write-Host ""
            Write-Host "Stopping container..." -ForegroundColor Yellow
            docker stop $CONTAINER_NAME | Out-Null
            
            Write-Host "Removing container..." -ForegroundColor Yellow
            docker rm $CONTAINER_NAME | Out-Null
            
            Write-Host "Deleting data volume..." -ForegroundColor Yellow
            docker volume rm $VOLUME_NAME
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "? Container and data completely removed" -ForegroundColor Green
                Write-Host ""
                Write-Host "To start fresh, run: .\Setup-PostgreSQL.ps1" -ForegroundColor Cyan
            }
            else {
                Write-Host "? Failed to remove volume" -ForegroundColor Red
                Write-Host "You may need to remove it manually: docker volume rm $VOLUME_NAME" -ForegroundColor Yellow
            }
        }
        else {
            Write-Host "? Operation cancelled" -ForegroundColor Yellow
        }
    }
    
    "4" {
        Write-Host "? Operation cancelled" -ForegroundColor Yellow
    }
    
    default {
        Write-Host "? Invalid choice" -ForegroundColor Red
    }
}

Write-Host ""
