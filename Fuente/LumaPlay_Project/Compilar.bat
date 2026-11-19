@echo off
setlocal
cd /d "%~dp0"
title Compilar LumaPlay
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Build.ps1"
echo.
pause
