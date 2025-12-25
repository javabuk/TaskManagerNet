@echo off
REM Lanzador simple para TaskManager en Windows
REM Guardar como: TaskManager.bat
REM Uso: TaskManager.bat proyecto listar

setlocal enabledelayedexpansion

REM Obtener la ruta del directorio del script
set SCRIPT_DIR=%~dp0

REM Ruta del ejecutable
set EXE=%SCRIPT_DIR%TaskManager.exe

REM Verificar si el ejecutable existe
if not exist "%EXE%" (
    color 0C
    echo.
    echo Error: TaskManager.exe no encontrado en:
    echo "%EXE%"
    echo.
    echo Verifica que descargaste los archivos correctamente.
    echo.
    pause
    exit /b 1
)

REM Ejecutar con los argumentos
"%EXE%" %*
