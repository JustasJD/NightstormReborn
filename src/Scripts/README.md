# Nightstorm RPG - Database Scripts

This folder contains PowerShell scripts for managing the PostgreSQL database in Docker.

---

## ?? **Available Scripts**

### **1. Setup-PostgreSQL.ps1**
Creates and configures a PostgreSQL database in Docker.

**Usage:**
```powershell
.\Setup-PostgreSQL.ps1
```

**What it does:**
- Pulls PostgreSQL 17 Alpine image
- Creates persistent volume for data
- Creates and starts container
- Configures database with proper credentials
- Displays connection information

**Configuration:**
- Container Name: `nightstorm-postgres`
- Port: `5432`
- Database: `NightstormDb`
- Username: `nightstorm`
- Password: `NightstormDev2024!`

---

### **2. Status-PostgreSQL.ps1**
Checks the status of the PostgreSQL container and database.

**Usage:**
```powershell
.\Status-PostgreSQL.ps1
```

**Shows:**
- Docker status
- Container status (running/stopped)
- Database health
- Database size
- Table count
- Connection information

---

### **3. Cleanup-PostgreSQL.ps1**
Stops and removes the PostgreSQL container.

**Usage:**
```powershell
.\Cleanup-PostgreSQL.ps1
```

**Options:**
1. Stop container (keep data)
2. Stop and remove container (keep data volume)
3. Stop, remove container AND delete all data ??

---

## ?? **Quick Start**

### **First Time Setup:**

1. **Start Docker Desktop**

2. **Run setup script:**
   ```powershell
   cd src\Scripts
   .\Setup-PostgreSQL.ps1
   ```

3. **Copy the connection string** displayed at the end

4. **Update appsettings.json:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=NightstormDb;Username=nightstorm;Password=NightstormDev2024!"
     }
   }
   ```

5. **Run migrations** (from solution root):
   ```powershell
   dotnet ef database update --project src/Nightstorm.Data --startup-project src/Nightstorm.Bot
   ```

---

## ?? **Common Tasks**

### **Check if database is running:**
```powershell
.\Status-PostgreSQL.ps1
```

### **Stop database:**
```powershell
docker stop nightstorm-postgres
```

### **Start database:**
```powershell
docker start nightstorm-postgres
```

### **Restart database:**
```powershell
docker restart nightstorm-postgres
```

### **View logs:**
```powershell
docker logs nightstorm-postgres
```

### **Connect to database shell:**
```powershell
docker exec -it nightstorm-postgres psql -U nightstorm -d NightstormDb
```

Once in psql shell:
```sql
-- List all tables
\dt

-- List all databases
\l

-- Show table structure
\d table_name

-- Run SQL query
SELECT * FROM "Characters" LIMIT 10;

-- Exit
\q
```

---

## ?? **Troubleshooting**

### **Script won't run - Execution Policy Error**
```powershell
# Check current policy
Get-ExecutionPolicy

# Allow scripts (run as Administrator)
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser

# Or run with bypass
powershell -ExecutionPolicy Bypass -File .\Setup-PostgreSQL.ps1
```

### **Docker not found**
- Ensure Docker Desktop is installed
- Restart Docker Desktop
- Check Docker is running: `docker --version`

### **Port 5432 already in use**
Another PostgreSQL instance is running:
```powershell
# Find what's using port 5432
netstat -ano | findstr :5432

# Stop the process or use different port in Setup script
```

### **Container won't start**
```powershell
# Check logs
docker logs nightstorm-postgres

# Remove and recreate
.\Cleanup-PostgreSQL.ps1  # Choose option 2
.\Setup-PostgreSQL.ps1
```

### **Database connection failed**
```powershell
# Check container is running
docker ps

# Check PostgreSQL is ready
docker exec nightstorm-postgres pg_isready -U nightstorm

# Check logs for errors
docker logs nightstorm-postgres
```

### **Lost connection string**
Run status script to see connection info:
```powershell
.\Status-PostgreSQL.ps1
```

---

## ?? **Data Persistence**

Database data is stored in Docker volume: `nightstorm-pgdata`

- **Stopping container** = Data is safe ?
- **Removing container** = Data is safe ? (if volume kept)
- **Removing volume** = Data is lost ?

To backup data:
```powershell
# Backup to file
docker exec nightstorm-postgres pg_dump -U nightstorm NightstormDb > backup.sql

# Restore from file
docker exec -i nightstorm-postgres psql -U nightstorm NightstormDb < backup.sql
```

---

## ?? **Security Notes**

**Development Credentials:**
- Username: `nightstorm`
- Password: `NightstormDev2024!`

?? **These are for LOCAL DEVELOPMENT ONLY!**

For production:
- Use strong, unique passwords
- Use environment variables
- Enable SSL/TLS
- Restrict network access
- Use managed database services (recommended)

---

## ?? **Need Help?**

1. Run `.\Status-PostgreSQL.ps1` to check current state
2. Check Docker Desktop is running
3. Check `docker logs nightstorm-postgres` for errors
4. Ensure port 5432 is available
5. Try recreating container with `.\Setup-PostgreSQL.ps1`

---

## ?? **Script Customization**

To change default settings, edit the configuration variables at the top of each script:

```powershell
$CONTAINER_NAME = "nightstorm-postgres"
$POSTGRES_VERSION = "17-alpine"
$POSTGRES_USER = "nightstorm"
$POSTGRES_PASSWORD = "NightstormDev2024!"
$POSTGRES_DB = "NightstormDb"
$POSTGRES_PORT = "5432"
```

---

**Last Updated:** 2024-01-20
**PostgreSQL Version:** 17-alpine
**Docker Required:** Yes
