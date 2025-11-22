# Docker Troubleshooting Guide

## ?? "Docker is not running or not installed"

This error occurs when PowerShell cannot communicate with Docker Desktop.

---

## ? **Quick Fixes (Try in Order):**

### **1. Verify Docker Desktop is Actually Running**

Check the system tray (bottom-right of taskbar):
- Look for Docker whale icon ??
- Click it and verify it says "Docker Desktop is running"
- If icon shows "Starting..." ? Wait 1-2 minutes

### **2. Wait for Docker to Fully Start**

Docker takes 30-60 seconds to fully initialize after opening:
```powershell
# Wait a bit, then test
Start-Sleep -Seconds 30
docker --version
```

### **3. Restart PowerShell**

Close PowerShell completely and open a new window:
- Sometimes PATH updates require new session
- Run as **Administrator** if regular user doesn't work

### **4. Test Docker Manually**

```powershell
# Test 1: Check version
docker --version

# Test 2: Check daemon
docker info

# Test 3: List containers
docker ps
```

If any fail ? Docker is not properly running.

---

## ?? **Diagnostic Script**

We created a diagnostic script to help identify the issue:

```powershell
cd src\Scripts
.\Diagnose-Docker.ps1
```

This will test:
- ? Docker CLI availability
- ? Docker daemon responding
- ? Container operations
- ? Volume operations
- ? System information

---

## ??? **Common Solutions:**

### **Solution A: Restart Docker Desktop**

1. Right-click Docker icon in system tray
2. Select "Quit Docker Desktop"
3. Open Start menu ? Search "Docker Desktop"
4. Launch Docker Desktop
5. Wait for "Docker Desktop is running" message
6. Try script again

### **Solution B: Check Docker Desktop Settings**

Open Docker Desktop ? Settings:

1. **General Tab:**
   - ? "Start Docker Desktop when you log in" (optional)
   - ? "Use WSL 2 based engine" (recommended for Windows 10/11)

2. **Resources Tab:**
   - Ensure sufficient memory allocated (minimum 2GB)
   - Ensure sufficient CPUs allocated (minimum 2)

3. **Apply & Restart**

### **Solution C: Reset Docker Desktop**

?? **This will delete all containers and images!**

1. Docker Desktop ? Troubleshoot (bug icon)
2. Click "Reset to factory defaults"
3. Confirm and wait for reset
4. Restart Docker Desktop
5. Try script again

### **Solution D: Check Windows Services**

1. Press `Win + R`
2. Type `services.msc` and press Enter
3. Find "Docker Desktop Service"
4. Ensure Status is "Running"
5. If not, right-click ? Start

### **Solution E: Reinstall Docker Desktop**

If all else fails:

1. Uninstall Docker Desktop
2. Delete `C:\Program Files\Docker`
3. Download latest from [docker.com](https://www.docker.com/products/docker-desktop/)
4. Install with default settings
5. Restart computer
6. Try script again

---

## ?? **Permission Issues**

### **"Access Denied" Errors**

Run PowerShell as Administrator:
1. Right-click PowerShell
2. Select "Run as Administrator"
3. Navigate to scripts folder
4. Try again

### **WSL 2 Issues (Windows 10/11)**

If using WSL 2 backend:

```powershell
# Check WSL status
wsl --status

# Update WSL
wsl --update

# Set default version
wsl --set-default-version 2
```

---

## ?? **Network Issues**

### **Corporate Firewall/Proxy**

If behind corporate firewall:

1. Docker Desktop ? Settings ? Resources ? Proxies
2. Enable "Manual proxy configuration"
3. Enter your proxy details
4. Apply & Restart

### **VPN Issues**

Some VPNs block Docker:
1. Disconnect VPN temporarily
2. Start Docker Desktop
3. Reconnect VPN
4. Try script

---

## ?? **Still Not Working?**

### **Collect Information:**

Run these commands and save output:

```powershell
# System info
systeminfo | Out-File docker-debug.txt

# Docker version
docker --version >> docker-debug.txt

# Docker info
docker info >> docker-debug.txt 2>&1

# Diagnostic script
.\Diagnose-Docker.ps1 >> docker-debug.txt

# Event logs (last 10 Docker events)
Get-EventLog -LogName Application -Source Docker -Newest 10 >> docker-debug.txt
```

### **Check Docker Desktop Logs:**

Location: `%LOCALAPPDATA%\Docker\log\`

Look for errors in:
- `host\`
- `vm\`

### **Common Error Messages:**

| Error | Solution |
|-------|----------|
| "Docker daemon not responding" | Restart Docker Desktop |
| "Cannot connect to Docker daemon" | Check Docker service is running |
| "No such file or directory" | Docker socket issue - restart Docker |
| "Access denied" | Run PowerShell as Administrator |
| "WSL 2 installation is incomplete" | Update WSL: `wsl --update` |

---

## ? **Verify Fix Worked**

After applying fixes, run:

```powershell
# 1. Test Docker
docker --version
docker info
docker ps

# 2. Run diagnostic
.\Diagnose-Docker.ps1

# 3. Try setup again
.\Setup-PostgreSQL.ps1
```

---

## ?? **Last Resort**

If absolutely nothing works:

1. **Use PostgreSQL without Docker:**
   - Install PostgreSQL directly from [postgresql.org](https://www.postgresql.org/download/windows/)
   - Use connection string: `Host=localhost;Port=5432;Database=NightstormDb;Username=postgres;Password=yourpassword`

2. **Use cloud-hosted PostgreSQL:**
   - [Neon](https://neon.tech/) - Free tier
   - [Supabase](https://supabase.com/) - Free tier
   - Use provided connection string in appsettings.json

---

## ?? **Need More Help?**

1. Check Docker Desktop version (should be latest)
2. Check Windows version (Windows 10 1903+ or Windows 11)
3. Check Hyper-V is enabled (for Windows Home: use WSL 2)
4. Search Docker GitHub issues: [github.com/docker/for-win](https://github.com/docker/for-win/issues)

---

**Most Common Fix:** Just restart Docker Desktop and wait 60 seconds! ??
