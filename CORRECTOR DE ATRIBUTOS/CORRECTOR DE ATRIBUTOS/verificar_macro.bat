@echo off
setlocal enabledelayedexpansion
REM Verificacion de instalacion de macro
REM Version: 2.0 - Corregido para buscar archivo _OLD.cs

echo ========================================
echo   VERIFICACION DE MACRO
echo ========================================
echo.

REM Determinar directorio del script
set SCRIPT_DIR=%~dp0

echo [1] Verificando archivo fuente...
echo.

REM Buscar archivo fuente (mismo enfoque que instalar_macro.bat)
if exist "%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set SOURCE=%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
) else if exist "%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set SOURCE=%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
) else if exist "%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set SOURCE=%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
) else (
    set SOURCE=NO_ENCONTRADO
)

if exist "%SOURCE%" (
    echo    [OK] Archivo fuente existe
    echo    Ubicacion: %SOURCE%
    for %%F in ("%SOURCE%") do (
        echo    Nombre: %%~nxF
        echo    Tamano: %%~zF bytes
        echo    Fecha:  %%~tF
    )
) else (
    echo    [ERROR] Archivo fuente NO encontrado
    echo    Buscando en:
    echo    - %SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo    - %SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo    - %SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
)

echo.
echo [2] Verificando archivo destino...
echo.

set TARGET=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs

if exist "%TARGET%" (
    echo    [OK] Macro instalada en Tekla
    echo    Ubicacion: %TARGET%
    for %%F in ("%TARGET%") do (
        echo    Nombre: %%~nxF
        echo    Tamano: %%~zF bytes
        echo    Fecha:  %%~tF
    )
) else (
    echo    [ERROR] Macro NO instalada en Tekla
    echo    Ubicacion esperada: %TARGET%
)

echo.
echo [3] Comparando archivos...
echo.

if exist "%SOURCE%" (
    if exist "%TARGET%" (
        REM Comparar tamanos
        for %%F in ("%SOURCE%") do set SOURCE_SIZE=%%~zF
        for %%F in ("%TARGET%") do set TARGET_SIZE=%%~zF
        
        if !SOURCE_SIZE! EQU !TARGET_SIZE! (
            echo    [OK] Los archivos tienen el mismo tamano: !SOURCE_SIZE! bytes
            echo    La macro parece estar actualizada
        ) else (
            echo    [WARN] LOS ARCHIVOS TIENEN DIFERENTE TAMANO
            echo    Fuente:  !SOURCE_SIZE! bytes
            echo    Destino: !TARGET_SIZE! bytes
            echo    Necesitas ejecutar instalar_macro.bat
        )
    ) else (
        echo    [INFO] No se puede comparar - macro no instalada
    )
) else (
    echo    [INFO] No se puede comparar - archivo fuente no encontrado
)

echo.
echo [4] Verificando directorio de macros de Tekla...
echo.

set MACRODIR=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling

if exist "%MACRODIR%" (
    echo    [OK] Directorio de macros existe
    echo    Ubicacion: %MACRODIR%
    echo.
    echo    Macros encontradas en modeling\:
    dir /b "%MACRODIR%\*.cs" 2>nul
    if errorlevel 1 (
        echo    [INFO] No hay archivos .cs en el directorio
    )
) else (
    echo    [WARN] Directorio de macros NO existe
    echo    Ubicacion esperada: %MACRODIR%
    echo    El directorio se creara al ejecutar instalar_macro.bat
)

echo.
echo ========================================
echo   RESUMEN
echo ========================================
echo.

if exist "%SOURCE%" (
    echo [OK] Archivo fuente: SI
) else (
    echo [ERROR] Archivo fuente: NO
)

if exist "%TARGET%" (
    echo [OK] Macro instalada: SI
) else (
    echo [ERROR] Macro instalada: NO
)

if exist "%SOURCE%" if exist "%TARGET%" (
    for %%F in ("%SOURCE%") do set SOURCE_SIZE=%%~zF
    for %%F in ("%TARGET%") do set TARGET_SIZE=%%~zF
    if !SOURCE_SIZE! EQU !TARGET_SIZE! (
        echo [OK] Archivos coinciden: SI
    ) else (
        echo [WARN] Archivos coinciden: NO
    )
)

echo.
echo ========================================
echo   ACCIONES RECOMENDADAS
echo ========================================
echo.

if not exist "%SOURCE%" (
    echo [!] CRITICO: Archivo fuente no encontrado
    echo     Asegurate de estar en el directorio correcto:
    echo     CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\
    echo.
)

if not exist "%TARGET%" (
    echo [1] Ejecutar: instalar_macro.bat
    echo [2] Reiniciar Tekla Structures
    echo [3] Verificar en Tekla: Tools ^> Macros... ^> SyncWeldPhaseFromParts
    echo.
) else (
    if exist "%SOURCE%" (
        for %%F in ("%SOURCE%") do set SOURCE_SIZE=%%~zF
        for %%F in ("%TARGET%") do set TARGET_SIZE=%%~zF
        if !SOURCE_SIZE! NEQ !TARGET_SIZE! (
            echo [1] Ejecutar: instalar_macro.bat (para actualizar)
            echo [2] Reiniciar Tekla Structures
            echo.
        ) else (
            echo [OK] La macro esta instalada y actualizada
            echo.
            echo Para usar la macro:
            echo [1] Abrir Tekla Structures
            echo [2] Tools ^> Macros...
            echo [3] Buscar: SyncWeldPhaseFromParts
            echo [4] Seleccionar y hacer clic en Run
            echo.
        )
    )
)

echo ========================================
echo.
pause
endlocal
