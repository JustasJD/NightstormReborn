# Fix PowerShell Docker PATH Issue

## ?? **Problem:**
Docker works in CMD but not in PowerShell. This happens because PowerShell uses its own PATH environment variable that doesn't automatically sync with Windows PATH.

---

## ? **Quick Fix - Use The PathFix Script:**

```powershell
cd src\Scripts
.\Setup-PostgreSQL-PathFix.ps1
```

This script automatically detects the issue and uses CMD as a fallback.

---

## ?? **Permanent Fix - Add Docker to PowerShell PATH:**

### **Option 1: Restart PowerShell (Simplest)**

Close PowerShell completely and open a new window. PowerShell reads PATH on startup.

**To test:**
```powershell
docker --version
```

If this works, you're done! ?

---

### **Option 2: Reload PATH in Current Session**

```powershell
# Refresh PowerShell environment
$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")

# Test Docker
docker --version
```

---

### **Option 3: Add Docker to PowerShell Profile (Permanent)**

1. **Check if profile exists:**
   ```powershell
   Test-Path $PROFILE
   ```

2. **Create profile if needed:**
   ```powershell
   if (!(Test-Path $PROFILE)) {
       New-Item -Path $PROFILE -Type File -Force
   }
   ```

3. **Edit profile:**
   ```powershell
   notepad $PROFILE
   ```

4. **Add this line:**
   ```powershell
   $env:Path += ";C:\Program Files\Docker\Docker\resources\bin"
   ```

5. **Save and reload:**
   ```powershell
   . $PROFILE
   ```

6. **Test:**
   ```powershell
   docker --version
   ```

---

### **Option 4: Fix Windows Environment Variables (System-wide)**

1. **Open System Properties:**
   - Press `Win + R`
   - Type `sysdm.cpl` and press Enter
   - Go to "Advanced" tab
   - Click "Environment Variables"

2. **Check User PATH:**
   - Under "User variables for [YourName]"
   - Select "Path"
   - Click "Edit"
   - Look for Docker entries:
     - `C:\Program Files\Docker\Docker\resources\bin`
     - `C:\ProgramData\DockerDesktop\version-bin`

3. **Add if missing:**
   - Click "New"
   - Add: `C:\Program Files\Docker\Docker\resources\bin`
   - Click "OK"

4. **Restart PowerShell**

5. **Test:**
   ```powershell
   docker --version
   ```

---

### **Option 5: Reinstall Docker Desktop (Nuclear Option)**

Sometimes Docker Desktop doesn't properly set up PATH during installation.

1. Uninstall Docker Desktop
2. Restart computer
3. Download latest Docker Desktop from [docker.com](https://www.docker.com/products/docker-desktop/)
4. Install with **"Use WSL 2 instead of Hyper-V"** option (if on Windows 10/11)
5. Restart computer
6. Test:
   ```powershell
   docker --version
   ```

---

## ?? **Verify Docker PATH:**

### **Check where Docker is installed:**
```powershell
# Option A: Using Get-Command (if working)
(Get-Command docker).Path

# Option B: Using CMD
cmd /c where docker
```

Expected locations:
- `C:\Program Files\Docker\Docker\resources\bin\docker.exe`
- `C:\ProgramData\DockerDesktop\version-bin\docker.exe`

### **Check current PATH:**
```powershell
# View PATH
$env:Path -split ';'

# Search for Docker
$env:Path -split ';' | Select-String -Pattern "Docker"
```

---

## ?? **Which Fix Should I Use?**

| Situation | Recommended Fix |
|-----------|----------------|
| Need it working NOW | **Option 1**: Use `Setup-PostgreSQL-PathFix.ps1` |
| Just restart PowerShell | **Option 1**: Close and reopen PowerShell |
| Want permanent fix | **Option 3**: Add to PowerShell profile |
| Multiple users affected | **Option 4**: Fix system environment variables |
| Nothing else works | **Option 5**: Reinstall Docker Desktop |

---

## ?? **Testing Your Fix:**

After applying any fix, test with these commands:

```powershell
# Test 1: Version
docker --version

# Test 2: Daemon
docker info

# Test 3: List containers
docker ps

# Test 4: Run test container
docker run hello-world
```

All should work without errors! ?

---

## ?? **Why Does This Happen?**

PowerShell maintains its own environment that's separate from CMD:
- **CMD** uses Windows PATH directly
- **PowerShell** caches PATH on startup
- **Docker Desktop** sometimes only updates Windows PATH, not PowerShell's cached version

---

## ?? **Still Not Working?**

### **Check Docker Installation Path:**

```powershell
# Check if Docker executable exists
Test-Path "C:\Program Files\Docker\Docker\resources\bin\docker.exe"

# If false, Docker might be in different location
cmd /c where docker
```

### **Manually Add Correct Path:**

If Docker is in a different location, use that path:

```powershell
$env:Path += ";YOUR_ACTUAL_DOCKER_PATH"
```

---

## ? **After Fix:**

Once fixed, you can use the regular setup script:

```powershell
cd src\Scripts
.\Setup-PostgreSQL.ps1
```

And all Docker commands will work normally in PowerShell! ??

---

## ?? **Related Issues:**

- [Docker for Windows - PATH not set](https://github.com/docker/for-win/issues/1038)
- [PowerShell doesn't see Docker](https://stackoverflow.com/questions/43684495)
- [Docker Desktop PATH issues](https://docs.docker.com/desktop/troubleshoot/topics/#docker-command-not-found)

---

**Quick Test:**
```powershell
docker --version
```

If this works ? Problem solved! ?  
If not ? Use `Setup-PostgreSQL-PathFix.ps1` as workaround
