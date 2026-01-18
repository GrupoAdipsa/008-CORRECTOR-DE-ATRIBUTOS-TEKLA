@echo off
cd /d "%~dp0"
echo ======================================
echo   DIAGNOSTICO DE PROPIEDADES DE SOLDADURAS
echo ======================================
echo.
echo Este programa te mostrara TODAS las
echo propiedades de una soldadura en Tekla.
echo.
echo Compilando...
dotnet build "..\CORRECTOR DE ATRIBUTOS.csproj" --configuration Debug /p:StartupObject=CORRECTOR_DE_ATRIBUTOS.WeldPropertyDiagnostic

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Compilacion exitosa!
    echo.
    echo INSTRUCCIONES:
    echo 1. Selecciona UNA SOLDADURA en Tekla
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
    echo Revisa los errores arriba.
    echo.
    pause
)
