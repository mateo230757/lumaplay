@echo off
setlocal
cd /d "%~dp0"
title Instalar LumaPlay
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Instalar-LumaPlay.ps1"
echo.
pause
