@echo off
echo ========================================
echo   COMPILAR (con cierre de proceso)
echo ========================================
echo.

echo [1/4] Cerrando proceso anterior...
taskkill /F /IM SINCRONIZADOR_ATRIBUTOS.exe 2>nul
timeout /t 2 /nobreak >nul

echo [2/4] Restaurando dependencias...
dotnet restore SINCRONIZADOR_ATRIBUTOS.csproj

echo.
echo [3/4] Limpiando proyecto...
dotnet clean SINCRONIZADOR_ATRIBUTOS.csproj --configuration Release

echo.
echo [4/4] Compilando proyecto...
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
