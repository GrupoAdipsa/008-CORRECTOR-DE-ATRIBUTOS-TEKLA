@echo off
echo ========================================
echo   SINCRONIZADOR DE ATRIBUTOS
echo   ESTATUS_PIEZA y PRIORIDAD
echo ========================================
echo.

set EXE_PATH=bin\Release\net48\SINCRONIZADOR_ATRIBUTOS.exe

if not exist "%EXE_PATH%" (
    echo [ERROR] No se encuentra el ejecutable compilado
    echo.
    echo Por favor compila el proyecto primero:
    echo   compilar.bat
    echo.
    pause
    exit /b 1
)

echo Ejecutando...
echo.
start "" "%EXE_PATH%"

echo Aplicacion iniciada
echo.
