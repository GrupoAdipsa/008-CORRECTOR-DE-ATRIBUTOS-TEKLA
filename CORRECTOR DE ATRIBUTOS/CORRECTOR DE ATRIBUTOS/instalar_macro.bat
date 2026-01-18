@echo off
setlocal enabledelayedexpansion
REM Script de instalacion de macro para Tekla
REM Version: 5.2 - Logica corregida de busqueda

echo ========================================
echo   INSTALAR MACRO: SyncWeldPhaseFromParts
echo ========================================
echo.

REM Determinar el directorio correcto
set SCRIPT_DIR=%~dp0
echo Directorio del script: %SCRIPT_DIR%
echo.

REM Detectar si estamos en el subdirectorio y ajustar
REM NOTA: MacroPlantilla esta en la raiz del repositorio (008-CORRECTOR-DE-ATRIBUTOS-TEKLA\MacroPlantilla\)

set SOURCE=
set BASE_DIR=

REM Probar dos niveles arriba (desde CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\)
if exist "%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\..\ 
    set SOURCE=%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo [OK] Macro encontrada: dos niveles arriba - raiz del repositorio
    goto :FOUND
)

REM Probar un nivel arriba (desde CORRECTOR DE ATRIBUTOS\)
if exist "%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\
    set SOURCE=%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo [OK] Macro encontrada: un nivel arriba
    goto :FOUND
)

REM Probar mismo directorio
if exist "%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%
    set SOURCE=%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo [OK] Macro encontrada: mismo directorio
    goto :FOUND
)

REM Si llegamos aqui, no se encontro el archivo
echo [ERROR] No se puede encontrar el archivo de la macro
echo.
echo Buscando en:
echo   - %SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
echo   - %SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
echo   - %SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
echo.
echo Directorio actual: %CD%
echo Directorio del script: %SCRIPT_DIR%
echo.
echo NOTA: Asegurate de estar ejecutando el script desde:
echo   CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\instalar_macro.bat
echo.
pause
exit /b 1

:FOUND
echo Directorio base: %BASE_DIR%
echo Archivo fuente: %SOURCE%
echo.

echo [1/5] Verificando archivo fuente...
echo.

if exist "%SOURCE%" (
    echo    [OK] Archivo fuente encontrado
    for %%F in ("%SOURCE%") do (
        echo    Nombre: %%~nxF
        echo    Tamano: %%~zF bytes
        echo    Fecha:  %%~tF
    )
) else (
    echo    [ERROR] Archivo fuente no encontrado
    echo    Ruta: %SOURCE%
    echo.
    pause
    exit /b 1
)

echo.
echo [2/5] Verificando directorio destino de Tekla...
echo.

set TARGETDIR=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling
set TARGET=%TARGETDIR%\SyncWeldPhaseFromParts.cs

echo Directorio destino: %TARGETDIR%
echo.

REM Verificar si ya existe una version anterior
if exist "%TARGET%" (
    echo    [INFO] Ya existe una version de la macro
    for %%F in ("%TARGET%") do (
        echo    Tamano actual: %%~zF bytes
        echo    Fecha actual:  %%~tF
    )
    echo    Se sobrescribira...
    echo.
)

echo.
echo [3/5] Preparando directorio destino...
echo.

REM Crear directorio si no existe
if not exist "%TARGETDIR%" (
    echo    Creando directorio: modeling\
    mkdir "%TARGETDIR%"
    if errorlevel 1 (
        echo    [ERROR] No se pudo crear el directorio
        echo    Ejecuta este script como Administrador (click derecho)
        pause
        exit /b 1
    )
    echo    [OK] Directorio creado
) else (
    echo    [OK] Directorio existe
)

echo.
echo [4/5] Copiando archivo...
echo.

echo    De: %SOURCE%
echo    A:  %TARGET%
echo.
echo    NOTA: El archivo _OLD.cs se copia como SyncWeldPhaseFromParts.cs
echo.

copy /Y "%SOURCE%" "%TARGET%"

if errorlevel 1 (
    echo.
    echo    [ERROR] No se pudo copiar el archivo
    echo.
    echo    Codigo de error: %ERRORLEVEL%
    echo.
    echo    Posibles causas:
    echo    1. Permisos insuficientes - Ejecutar como Administrador
    echo    2. Archivo en uso por Tekla
    echo    3. Tekla no instalado en ruta estandar
    echo.
    pause
    exit /b 1
)

echo.
echo [5/5] Verificando instalacion...
echo.

if exist "%TARGET%" (
    echo    [OK] Archivo copiado correctamente
    echo.
    REM Comparar tamanos
    for %%F in ("%SOURCE%") do set SOURCE_SIZE=%%~zF
    for %%F in ("%TARGET%") do set TARGET_SIZE=%%~zF
    
    if !SOURCE_SIZE! EQU !TARGET_SIZE! (
        echo    [OK] Tamanos coinciden: !SOURCE_SIZE! bytes
    ) else (
        echo    [WARN] Tamanos diferentes:
        echo      Fuente: !SOURCE_SIZE! bytes
        echo      Destino: !TARGET_SIZE! bytes
    )
    echo.
    echo ========================================
    echo   INSTALACION EXITOSA
    echo ========================================
    echo.
    echo Ubicacion: %TARGET%
    echo.
    echo ========================================
    echo   IMPORTANTE: REINICIAR TEKLA
    echo ========================================
    echo.
    echo Tekla solo detecta macros nuevas al iniciar.
    echo.
    echo PASOS SIGUIENTES:
    echo 1. Cerrar Tekla Structures completamente
    echo 2. Volver a abrir Tekla
    echo 3. Tools ^> Macros... ^> SyncWeldPhaseFromParts
    echo 4. Seleccionar la macro y hacer clic en Run
    echo.
    echo ========================================
) else (
    echo    [ERROR] El archivo no se encuentra en destino
    echo    Algo salio mal durante la copia
    echo.
)

echo.
pause
endlocal


