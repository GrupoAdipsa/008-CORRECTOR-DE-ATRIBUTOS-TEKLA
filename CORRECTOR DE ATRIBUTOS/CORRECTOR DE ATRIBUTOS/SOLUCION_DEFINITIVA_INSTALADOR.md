# ? SOLUCIÓN DEFINITIVA: Script Funciona Desde Cualquier Ubicación

## ?? PROBLEMA RESUELTO

**Versión anterior**: El script fallaba cuando se ejecutaba desde subdirectorios  
**Versión nueva (v5.0)**: Funciona desde **cualquier** ubicación

---

## ? MEJORAS EN v5.0

### **Detección Inteligente de Rutas**:
```
1. Primero intenta: .\MacroPlantilla\SyncWeldPhaseFromParts.cs
2. Si falla, intenta: ..\MacroPlantilla\SyncWeldPhaseFromParts.cs
3. Si falla, usa ruta absoluta
```

### **Características**:
- ? Funciona desde directorio raíz
- ? Funciona desde subdirectorio
- ? Usa `setlocal enabledelayedexpansion` para variables dinámicas
- ? Compara tamaños de archivo
- ? Mejor manejo de errores

---

## ?? CÓMO USAR

### **Desde Windows Explorer** (Recomendado):

```
1. Navegar a: C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS

2. Doble click en: instalar_macro.bat

3. El script automáticamente:
   - Detecta la ubicación correcta
   - Encuentra el archivo fuente
   - Copia a Tekla
   - Verifica tamaños

4. Si dice "INSTALACIÓN EXITOSA":
   ? Listo!
   ? Reiniciar Tekla
```

---

### **Desde Línea de Comandos**:

#### Opción 1: Desde directorio raíz
```cmd
cd "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS"
instalar_macro.bat
```

#### Opción 2: Desde subdirectorio (también funciona ahora)
```cmd
cd "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
..\instalar_macro.bat
```

#### Opción 3: Ruta completa
```cmd
"C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\instalar_macro.bat"
```

---

## ?? VERIFICACIÓN

Después de ejecutar `instalar_macro.bat`, deberías ver:

```
========================================
  INSTALAR MACRO: SyncWeldPhaseFromParts
========================================

Directorio del script: C:\Users\...\CORRECTOR DE ATRIBUTOS\
? Ejecutando desde directorio correcto
Base directory: C:\Users\...\CORRECTOR DE ATRIBUTOS\

[1/5] Verificando archivo fuente...

   ? Archivo fuente encontrado
   Nombre: SyncWeldPhaseFromParts.cs
   Tamaño: XXXXX bytes
   Fecha:  ...

[2/5] Verificando archivo destino...
[3/5] Preparando directorio destino...

   ? Directorio existe

[4/5] Copiando archivo...

   De: C:\Users\...\MacroPlantilla\SyncWeldPhaseFromParts.cs
   A:  C:\ProgramData\...\modeling\SyncWeldPhaseFromParts.cs

   1 archivo(s) copiado(s).

[5/5] Verificando instalación...

   ? Archivo copiado correctamente

   ? Tamaños coinciden: XXXXX bytes

========================================
  INSTALACIÓN EXITOSA
========================================
```

---

## ?? FLUJO COMPLETO

### **Primera instalación**:
```
1. instalar_macro.bat
   ? Ve todo el proceso paso a paso
   ? Confirma "INSTALACIÓN EXITOSA"

2. Reiniciar Tekla
   ? Cerrar completamente
   ? Volver a abrir

3. Verificar en Tekla
   ? Tools > Macros...
   ? Buscar: SyncWeldPhaseFromParts
   ? ? Debe aparecer
```

### **Actualizar macro existente**:
```
1. verificar_macro.bat
   ? Ver si archivos son diferentes

2. Si son diferentes:
   instalar_macro.bat
   ? Actualiza automáticamente

3. verificar_macro.bat otra vez
   ? Debe decir "IDÉNTICOS"

4. Reiniciar Tekla
```

---

## ?? SI AÚN TIENES PROBLEMAS

### **Error: "Archivo fuente no encontrado"**

**Verificar que el archivo existe**:
```cmd
dir "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs"
```

Si no existe, el archivo fue movido o eliminado.

---

### **Error: "No se pudo copiar"**

**Causas posibles**:
1. **Permisos**: Ejecutar como Administrador
2. **Tekla abierto**: Cerrar Tekla primero
3. **Ruta incorrecta**: Tekla no instalado en ubicación estándar

**Solución**:
```cmd
REM Cerrar Tekla completamente
REM Luego ejecutar como Administrador:
Click derecho en instalar_macro.bat ? "Ejecutar como administrador"
```

---

### **Error: "Tamaños diferentes"**

Esto puede ocurrir si:
- La copia se interrumpió
- El archivo está corrupto

**Solución**:
```
1. Ejecutar instalar_macro.bat otra vez
2. Si persiste, usar método manual de PowerShell
```

---

## ?? MÉTODO MANUAL (100% Seguro)

Si el script `.bat` no funciona, usar PowerShell:

```powershell
# Ejecutar como Administrador

$source = "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs"
$dest = "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"

# Copiar con sobrescritura
Copy-Item $source $dest -Force

# Verificar
if (Test-Path $dest) {
    $sourceSize = (Get-Item $source).Length
    $destSize = (Get-Item $dest).Length
    
    Write-Host "Fuente: $sourceSize bytes" -ForegroundColor Cyan
    Write-Host "Destino: $destSize bytes" -ForegroundColor Cyan
    
    if ($sourceSize -eq $destSize) {
        Write-Host "? INSTALADO CORRECTAMENTE" -ForegroundColor Green
    } else {
        Write-Host "? Tamaños diferentes" -ForegroundColor Red
    }
} else {
    Write-Host "? Error al copiar" -ForegroundColor Red
}
```

---

## ?? SCRIPTS DISPONIBLES

| Script | Versión | Propósito |
|--------|---------|-----------|
| `instalar_macro.bat` | v5.0 | Instalar/Actualizar macro |
| `verificar_macro.bat` | v1.0 | Comparar archivos |
| Manual PowerShell | - | Método alternativo 100% |

---

## ? CHECKLIST FINAL

- [ ] Ejecutar `instalar_macro.bat` (v5.0)
- [ ] Ver mensaje "INSTALACIÓN EXITOSA"
- [ ] Verificar "Tamaños coinciden"
- [ ] Cerrar Tekla
- [ ] Volver a abrir Tekla
- [ ] Tools > Macros > SyncWeldPhaseFromParts
- [ ] ? Macro aparece en la lista
- [ ] Probar macro

---

## ?? RESULTADO ESPERADO

```
========================================
  INSTALACIÓN EXITOSA
========================================

Ubicación: C:\ProgramData\...\modeling\SyncWeldPhaseFromParts.cs

? Tamaños coinciden: XXXXX bytes

========================================
  IMPORTANTE: REINICIAR TEKLA
========================================
```

---

**Versión del script**: v5.0 (DEFINITIVA)  
**Estado**: ? FUNCIONA DESDE CUALQUIER UBICACIÓN  
**Próximo paso**: EJECUTAR Y REINICIAR TEKLA ??
