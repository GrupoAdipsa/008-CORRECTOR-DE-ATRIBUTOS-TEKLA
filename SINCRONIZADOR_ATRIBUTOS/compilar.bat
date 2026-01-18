@echo off
echo ========================================
echo   COMPILAR: Sincronizador de Atributos
echo ========================================
echo.

echo [1/3] Restaurando dependencias...
dotnet restore SINCRONIZADOR_ATRIBUTOS.csproj

if errorlevel 1 (
    echo.
    echo [ERROR] Fallo al restaurar dependencias
    pause
    exit /b 1
)

echo.
echo [2/3] Limpiando proyecto...
dotnet clean SINCRONIZADOR_ATRIBUTOS.csproj --configuration Release

echo.
echo [3/3] Compilando proyecto...
dotnet build SINCRONIZADOR_ATRIBUTOS.csproj --configuration Release --no-restore

if errorlevel 1 (
    echo.
    echo [ERROR] Fallo la compilacion
    pause
    exit /b 1
)

echo.
echo ========================================
echo   COMPILACION EXITOSA
echo ========================================
echo.
echo Ejecutable generado en:
echo bin\Release\net48\SINCRONIZADOR_ATRIBUTOS.exe
echo.
pause
