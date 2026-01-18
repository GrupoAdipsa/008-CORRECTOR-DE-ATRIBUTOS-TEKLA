@echo off
cd /d "%~dp0"
echo ======================================
echo   DIAGNOSTICO DE PROPIEDADES PHASE
echo ======================================
echo.
echo Este programa te dira exactamente que
echo propiedades de Phase tiene una pieza.
echo.
echo Compilando...
dotnet build "..\CORRECTOR DE ATRIBUTOS.csproj" --configuration Debug /p:StartupObject=CORRECTOR_DE_ATRIBUTOS.PhasePropertyDiagnostic

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Compilacion exitosa!
    echo.
    echo INSTRUCCIONES:
    echo 1. Selecciona UNA pieza en Tekla
    echo 2. La herramienta te mostrara todas sus propiedades
    echo.
    echo Ejecutando diagnostico...
    echo.
    cd ..
    start /wait "" "Installer\BuildDrop\net48\CORRECTOR_DE_ATRIBUTOS.exe"
    echo.
    echo El diagnostico ha finalizado.
    pause
) else (
    echo.
    echo ERROR: La compilacion fallo
    echo Revisa que Visual Studio 2022 este cerrado
    echo o ejecuta desde PowerShell:
    echo   cd "CORRECTOR DE ATRIBUTOS"
    echo   dotnet build
    echo.
    pause
)
