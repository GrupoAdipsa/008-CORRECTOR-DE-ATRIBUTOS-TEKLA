@echo off
REM Verificación de instalación de macro
echo ========================================
echo   VERIFICACIÓN DE MACRO
echo ========================================
echo.

echo [1] Verificando archivo fuente...
echo.

set SOURCE=C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs

if exist "%SOURCE%" (
    echo    ? Archivo fuente existe
    for %%F in ("%SOURCE%") do (
        echo    Tamaño: %%~zF bytes
        echo    Fecha:  %%~tF
    )
) else (
    echo    ? Archivo fuente NO encontrado
    echo    Ruta: %SOURCE%
)

echo.
echo [2] Verificando archivo destino...
echo.

set TARGET=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs

if exist "%TARGET%" (
    echo    ? Macro instalada en Tekla
    for %%F in ("%TARGET%") do (
        echo    Tamaño: %%~zF bytes
        echo    Fecha:  %%~tF
    )
) else (
    echo    ? Macro NO instalada en Tekla
    echo    Ruta: %TARGET%
)

echo.
echo [3] Comparando archivos...
echo.

if exist "%SOURCE%" if exist "%TARGET%" (
    fc /b "%SOURCE%" "%TARGET%" >nul
    if errorlevel 1 (
        echo    ? LOS ARCHIVOS SON DIFERENTES
        echo    El archivo instalado NO coincide con el fuente
        echo    Necesitas ejecutar instalar_macro.bat
    ) else (
        echo    ? Los archivos son idénticos
        echo    La macro está actualizada
    )
) else (
    echo    ? No se puede comparar - falta uno de los archivos
)

echo.
echo ========================================
echo   RESUMEN
echo ========================================
echo.

if exist "%TARGET%" (
    echo ? Macro instalada: SÍ
) else (
    echo ? Macro instalada: NO
)

if exist "%SOURCE%" (
    echo ? Archivo fuente: SÍ
) else (
    echo ? Archivo fuente: NO
)

echo.
echo ========================================
echo   ACCIONES
echo ========================================
echo.

if not exist "%TARGET%" (
    echo 1. Ejecutar: instalar_macro.bat
    echo 2. Reiniciar Tekla
)

if exist "%SOURCE%" if exist "%TARGET%" (
    fc /b "%SOURCE%" "%TARGET%" >nul
    if errorlevel 1 (
        echo 1. Ejecutar: instalar_macro.bat
        echo 2. Reiniciar Tekla
    ) else (
        echo 1. Reiniciar Tekla ^(si aún no lo hiciste^)
        echo 2. Tools ^> Macros ^> SyncWeldPhaseFromParts
    )
)

echo.
pause
