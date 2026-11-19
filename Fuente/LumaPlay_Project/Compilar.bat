@echo off
setlocal
cd /d "%~dp0"
title Compilar LumaPlay 1.1.0
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Build.ps1"
if errorlevel 1 (
  echo.
  echo La compilacion fallo. Consulta los mensajes anteriores.
  pause
  exit /b 1
)
echo.
echo Compilacion completada en la carpeta publish.
pause
