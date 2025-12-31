@echo off
setlocal enabledelayedexpansion

echo ---------------------------------------------------------
echo [Impact Hub ERP] Deep Cleaning started...
echo ---------------------------------------------------------

:: Move to the directory where the batch file is located
cd /d "%~dp0"

:: 1. Forcefully remove any Read-Only/Hidden/System attributes from bin/obj
:: (This ensures the 'rd' command won't get Access Denied)
for /d /r . %%d in (bin,obj) do (
    if exist "%%d" (
        attrib -h -r -s "%%d\*.*" /s /d >nul 2>&1
        echo Deleting: "%%d"
        rd /s /q "%%d" >nul 2>&1
    )
)

echo ---------------------------------------------------------
echo Done! Verifying current structure...
echo ---------------------------------------------------------

tree /f
pause