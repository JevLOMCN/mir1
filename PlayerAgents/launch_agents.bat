@echo off
set "LISTFILE=%~dp0account_password_charlist.txt"
if not exist "%LISTFILE%" (
    echo List file "%LISTFILE%" not found.
    exit /b 1
)

for /f "usebackq tokens=1,2,3" %%A in ("%LISTFILE%") do (
    start "Agent %%A-%%C" "%~dp0PlayerAgents.exe" --account "%%A" --password "%%B" --character "%%C"
)