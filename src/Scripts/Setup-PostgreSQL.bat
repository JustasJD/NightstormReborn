@echo off
REM Nightstorm RPG - PostgreSQL Docker Setup Script (CMD Version)
REM This script creates and configures a PostgreSQL database in Docker

setlocal enabledelayedexpansion

echo ==================================
echo   Nightstorm RPG - Database Setup
echo ==================================
echo.

REM Configuration
set CONTAINER_NAME=nightstorm-postgres
set POSTGRES_VERSION=17-alpine
set POSTGRES_USER=nightstorm
set POSTGRES_PASSWORD=NightstormDev2024!
set POSTGRES_DB=NightstormDb
set POSTGRES_PORT=5432
set VOLUME_NAME=nightstorm-pgdata

REM Check if Docker is running
echo Checking Docker status...
docker info >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not running or not accessible
    echo.
    echo Troubleshooting:
    echo 1. Ensure Docker Desktop is running ^(check system tray^)
    echo 2. Wait 30-60 seconds for Docker to fully start
    echo 3. Restart Docker Desktop if needed
    echo 4. Verify Docker CLI works: docker --version
    echo.
    exit /b 1
)
echo [OK] Docker is running
echo.

REM Check if container already exists
echo Checking for existing container...
for /f "delims=" %%i in ('docker ps -a --filter "name=%CONTAINER_NAME%" --format "{{.Names}}" 2^>nul') do set EXISTING_CONTAINER=%%i

if "%EXISTING_CONTAINER%"=="%CONTAINER_NAME%" (
    echo [WARNING] Container '%CONTAINER_NAME%' already exists
    set /p RECREATE="Do you want to remove and recreate it? (y/N): "
    
    if /i "!RECREATE!"=="y" (
        echo Stopping container...
        docker stop %CONTAINER_NAME% >nul 2>&1
        
        echo Removing container...
        docker rm %CONTAINER_NAME% >nul 2>&1
        
        echo [OK] Container removed
    ) else (
        echo Keeping existing container
        
        REM Check if container is running
        for /f "delims=" %%i in ('docker ps --filter "name=%CONTAINER_NAME%" --format "{{.Names}}" 2^>nul') do set RUNNING_CONTAINER=%%i
        
        if not "!RUNNING_CONTAINER!"=="%CONTAINER_NAME%" (
            echo Starting existing container...
            docker start %CONTAINER_NAME% >nul 2>&1
            echo [OK] Container started
        ) else (
            echo [OK] Container is already running
        )
        
        echo.
        echo ==================================
        echo   Connection Information
        echo ==================================
        echo Host:     localhost
        echo Port:     %POSTGRES_PORT%
        echo Database: %POSTGRES_DB%
        echo Username: %POSTGRES_USER%
        echo Password: %POSTGRES_PASSWORD%
        echo.
        echo Connection String:
        echo Host=localhost;Port=%POSTGRES_PORT%;Database=%POSTGRES_DB%;Username=%POSTGRES_USER%;Password=%POSTGRES_PASSWORD%
        exit /b 0
    )
)

REM Create Docker volume for persistence
echo.
echo Creating Docker volume...
for /f "delims=" %%i in ('docker volume ls --filter "name=%VOLUME_NAME%" --format "{{.Name}}" 2^>nul') do set EXISTING_VOLUME=%%i

if "%EXISTING_VOLUME%"=="%VOLUME_NAME%" (
    echo [OK] Volume '%VOLUME_NAME%' already exists
) else (
    docker volume create %VOLUME_NAME% >nul 2>&1
    echo [OK] Volume created
)

REM Pull PostgreSQL image
echo.
echo Pulling PostgreSQL image...
echo This may take a few minutes...
docker pull postgres:%POSTGRES_VERSION%

REM Create and start container
echo.
echo Creating PostgreSQL container...
docker run -d ^
    --name %CONTAINER_NAME% ^
    -e POSTGRES_USER=%POSTGRES_USER% ^
    -e POSTGRES_PASSWORD=%POSTGRES_PASSWORD% ^
    -e POSTGRES_DB=%POSTGRES_DB% ^
    -p %POSTGRES_PORT%:5432 ^
    -v %VOLUME_NAME%:/var/lib/postgresql/data ^
    --restart unless-stopped ^
    postgres:%POSTGRES_VERSION% >nul 2>&1

if errorlevel 1 (
    echo [ERROR] Failed to create container
    exit /b 1
)
echo [OK] Container created successfully

REM Wait for PostgreSQL to be ready
echo.
echo Waiting for PostgreSQL to be ready...
set MAX_ATTEMPTS=30
set ATTEMPT=0

:wait_loop
set /a ATTEMPT+=1

docker exec %CONTAINER_NAME% pg_isready -U %POSTGRES_USER% 2>nul | findstr /C:"accepting connections" >nul
if not errorlevel 1 (
    echo [OK] PostgreSQL is ready!
    goto :ready
)

echo   Attempt %ATTEMPT%/%MAX_ATTEMPTS% - waiting...
timeout /t 2 /nobreak >nul

if %ATTEMPT% lss %MAX_ATTEMPTS% goto :wait_loop

echo [ERROR] PostgreSQL failed to start within timeout
echo Check container logs with: docker logs %CONTAINER_NAME%
exit /b 1

:ready

REM Verify database exists
echo.
echo Setting up database...
docker exec %CONTAINER_NAME% psql -U %POSTGRES_USER% -lqt 2>nul | findstr /C:"%POSTGRES_DB%" >nul
if not errorlevel 1 (
    echo [OK] Database '%POSTGRES_DB%' is ready
) else (
    echo [WARNING] Database not found, but container is running
)

REM Display connection information
echo.
echo ==================================
echo   Setup Complete!
echo ==================================
echo.
echo PostgreSQL is now running in Docker
echo.
echo Connection Information:
echo ----------------------
echo Host:     localhost
echo Port:     %POSTGRES_PORT%
echo Database: %POSTGRES_DB%
echo Username: %POSTGRES_USER%
echo Password: %POSTGRES_PASSWORD%
echo.
echo Connection String (copy to appsettings.json):
echo Host=localhost;Port=%POSTGRES_PORT%;Database=%POSTGRES_DB%;Username=%POSTGRES_USER%;Password=%POSTGRES_PASSWORD%
echo.
echo Useful Commands:
echo   Stop:    docker stop %CONTAINER_NAME%
echo   Start:   docker start %CONTAINER_NAME%
echo   Restart: docker restart %CONTAINER_NAME%
echo   Logs:    docker logs %CONTAINER_NAME%
echo   Shell:   docker exec -it %CONTAINER_NAME% psql -U %POSTGRES_USER% -d %POSTGRES_DB%
echo   Remove:  docker stop %CONTAINER_NAME% ^&^& docker rm %CONTAINER_NAME%
echo.
echo Next Steps:
echo 1. Update src/Nightstorm.Bot/appsettings.json with the connection string above
echo 2. Run database migrations:
echo    dotnet ef database update --project src/Nightstorm.Data --startup-project src/Nightstorm.Bot
echo.

endlocal