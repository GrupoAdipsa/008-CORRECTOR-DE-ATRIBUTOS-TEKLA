@echo off
setlocal enabledelayedexpansion
REM Script de instalación de macro para Tekla
REM Versión: 5.0 - Funciona desde cualquier ubicación

echo ========================================
echo   INSTALAR MACRO: SyncWeldPhaseFromParts
echo ========================================
echo.

REM Determinar el directorio correcto
REM El script debe estar en: CORRECTOR DE ATRIBUTOS\
REM Pero puede ejecutarse desde: CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\

set SCRIPT_DIR=%~dp0
echo Directorio del script: %SCRIPT_DIR%
echo.

REM Detectar si estamos en el subdirectorio y ajustar
REM NOTA: MacroPlantilla ahora está FUERA del proyecto C#
if exist "%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\
    echo ? Macro encontrada (un nivel arriba)
) else if exist "%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts.cs" (
    set BASE_DIR=%SCRIPT_DIR%
    echo ? Macro encontrada (mismo directorio)
) else (
    REM Intentar con ruta absoluta
    set BASE_DIR=C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\
    if exist "!BASE_DIR!MacroPlantilla\SyncWeldPhaseFromParts.cs" (
        echo ? Usando ruta absoluta (raíz del repositorio)
    ) else (
        echo ? ERROR: No se puede determinar la ubicación del archivo
        echo.
        echo Directorio actual: %CD%
        echo Directorio del script: %SCRIPT_DIR%
        echo.
        pause
        exit /b 1
    )
)

echo Base directory: %BASE_DIR%
echo.

echo [1/5] Verificando archivo fuente...
echo.

set SOURCE=%BASE_DIR%MacroPlantilla\SyncWeldPhaseFromParts.cs

if exist "%SOURCE%" (
    echo    ? Archivo fuente encontrado
    for %%F in ("%SOURCE%") do (
        echo    Nombre: %%~nxF
        echo    Tamaño: %%~zF bytes
        echo    Fecha:  %%~tF
    )
) else (
    echo    ? ERROR: Archivo fuente no encontrado
    echo    Ruta: %SOURCE%
    echo.
    pause
    exit /b 1
)

echo.
echo [2/5] Verificando archivo destino...
echo.

set TARGET=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs
set TARGETDIR=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling

REM Verificar si ya existe una versión anterior
if exist "%TARGET%" (
    echo    ? Ya existe una versión de la macro
    for %%F in ("%TARGET%") do (
        echo    Tamaño actual: %%~zF bytes
        echo    Fecha actual:  %%~tF
    )
    echo    Se sobrescribirá...
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
        echo    ? ERROR: No se pudo crear el directorio
        echo    Ejecuta este script como Administrador (click derecho)
        pause
        exit /b 1
    )
    echo    ? Directorio creado
) else (
    echo    ? Directorio existe
)

echo.
echo [4/5] Copiando archivo...
echo.

echo    De: %SOURCE%
echo    A:  %TARGET%
echo.

copy /Y "%SOURCE%" "%TARGET%"

if errorlevel 1 (
    echo.
    echo    ? ERROR: No se pudo copiar el archivo
    echo.
    echo    Código de error: %ERRORLEVEL%
    echo.
    echo    Posibles causas:
    echo    1. Permisos insuficientes - Ejecutar como Administrador
    echo    2. Archivo en uso por Tekla
    echo    3. Tekla no instalado en ruta estándar
    echo.
    pause
    exit /b 1
)

echo.
echo [5/5] Verificando instalación...
echo.

if exist "%TARGET%" (
    echo    ? Archivo copiado correctamente
    echo.
    REM Comparar tamaños
    for %%F in ("%SOURCE%") do set SOURCE_SIZE=%%~zF
    for %%F in ("%TARGET%") do set TARGET_SIZE=%%~zF
    
    if !SOURCE_SIZE! EQU !TARGET_SIZE! (
        echo    ? Tamaños coinciden: !SOURCE_SIZE! bytes
    ) else (
        echo    ? Tamaños diferentes:
        echo      Fuente: !SOURCE_SIZE! bytes
        echo      Destino: !TARGET_SIZE! bytes
    )
    echo.
    echo ========================================
    echo   INSTALACIÓN EXITOSA
    echo ========================================
    echo.
    echo Ubicación: %TARGET%
    echo.
    echo ========================================
    echo   IMPORTANTE: REINICIAR TEKLA
    echo ========================================
    echo.
    echo Tekla solo detecta macros nuevas al iniciar.
    echo.
    echo 1. Cerrar Tekla Structures completamente
    echo 2. Volver a abrir Tekla
    echo 3. Tools ^> Macros... ^> SyncWeldPhaseFromParts
    echo.
    echo ========================================
) else (
    echo    ? ERROR: El archivo no se encuentra en destino
    echo    Algo salió mal durante la copia
    echo.
)

echo.
pause
endlocal

