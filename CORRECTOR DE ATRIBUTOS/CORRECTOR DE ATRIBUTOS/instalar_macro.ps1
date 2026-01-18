# Script de instalación de macro para Tekla
# Versión: 2.1 - Corregida búsqueda de rutas

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  INSTALAR MACRO: SyncWeldPhaseFromParts" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Obtener directorio del script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Write-Host "Directorio del script: $scriptDir" -ForegroundColor Gray
Write-Host ""

# Buscar el archivo en múltiples ubicaciones
$possiblePaths = @(
    Join-Path $scriptDir "MacroPlantilla\SyncWeldPhaseFromParts.cs"
    Join-Path $scriptDir "..\MacroPlantilla\SyncWeldPhaseFromParts.cs"
    "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs"
)

$source = $null
Write-Host "Buscando archivo fuente..." -ForegroundColor Yellow

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $source = $path
        Write-Host "? Encontrado en: $path" -ForegroundColor Green
        break
    }
}

if (-not $source) {
    Write-Host "? ERROR: Archivo fuente no encontrado" -ForegroundColor Red
    Write-Host ""
    Write-Host "Ubicaciones verificadas:" -ForegroundColor Yellow
    foreach ($path in $possiblePaths) {
        Write-Host "  - $path" -ForegroundColor White
    }
    Write-Host ""
    Write-Host "Solución:" -ForegroundColor Yellow
    Write-Host "  1. Verifica que el archivo existe en MacroPlantilla\" -ForegroundColor White
    Write-Host "  2. Ejecuta desde el directorio correcto" -ForegroundColor White
    Write-Host ""
    Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host ""
Write-Host "Preparando instalación..." -ForegroundColor Yellow
Write-Host ""

$dest = "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"

# Crear directorio si no existe
$destDir = Split-Path -Parent $dest
if (!(Test-Path $destDir)) {
    Write-Host "Creando directorio de macros..." -ForegroundColor Yellow
    try {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        Write-Host "? Directorio creado" -ForegroundColor Green
    }
    catch {
        Write-Host "? ERROR: No se pudo crear el directorio" -ForegroundColor Red
        Write-Host "  $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Solución: Ejecutar como Administrador" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        exit 1
    }
    Write-Host ""
} else {
    Write-Host "? Directorio existe" -ForegroundColor Green
    Write-Host ""
}

# Copiar archivo
Write-Host "Copiando macro..." -ForegroundColor Yellow
Write-Host "  Desde: $source" -ForegroundColor White
Write-Host "  Hacia: $dest" -ForegroundColor White
Write-Host ""

try {
    Copy-Item $source $dest -Force
    
    # Verificar
    if (Test-Path $dest) {
        Write-Host ""
        Write-Host "??? MACRO INSTALADA CORRECTAMENTE ???" -ForegroundColor Green
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "  INSTALACIÓN EXITOSA" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Ubicación:" -ForegroundColor Cyan
        Write-Host "  $dest" -ForegroundColor White
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host "  PRÓXIMOS PASOS" -ForegroundColor Cyan
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "1. REINICIAR TEKLA STRUCTURES" -ForegroundColor Yellow
        Write-Host "   (necesario para detectar la macro)" -ForegroundColor White
        Write-Host ""
        Write-Host "2. En Tekla:" -ForegroundColor Yellow
        Write-Host "   - Tools > Macros..." -ForegroundColor White
        Write-Host "   - Buscar: SyncWeldPhaseFromParts" -ForegroundColor White
        Write-Host "   - Seleccionar y Run" -ForegroundColor White
        Write-Host ""
        Write-Host "3. Elegir alcance:" -ForegroundColor Yellow
        Write-Host "   - SÍ = Solo soldaduras seleccionadas" -ForegroundColor White
        Write-Host "   - NO = Todas las soldaduras del modelo" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "? ERROR: No se pudo verificar la instalación" -ForegroundColor Red
    }
}
catch {
    Write-Host "? ERROR: No se pudo copiar el archivo" -ForegroundColor Red
    Write-Host "  $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Solución: Ejecutar como Administrador" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
