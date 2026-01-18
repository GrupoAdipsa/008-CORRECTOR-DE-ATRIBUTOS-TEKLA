@echo off
cd /d "%~dp0"
echo ======================================
echo   Compilando aplicacion...
echo ======================================
dotnet build "..\CORRECTOR DE ATRIBUTOS.csproj" --configuration Debug

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ======================================
    echo   Compilacion exitosa!
    echo   Ejecutando aplicacion...
    echo ======================================
    echo.
    cd ..
    start "" "Installer\BuildDrop\net48\CORRECTOR_DE_ATRIBUTOS.exe"
    echo.
    echo La aplicacion se ha iniciado.
    echo Asegurate de que Tekla Structures este abierto con un modelo.
) else (
    echo.
    echo ======================================
    echo   ERROR: La compilacion fallo
    echo ======================================
    pause
)
