# ?? SOLUCIÓN: instalar_macro.bat NO ACTUALIZA EL ARCHIVO

## ?? PROBLEMA

El script `instalar_macro.bat` no actualiza correctamente la macro en Tekla.

---

## ? SOLUCIONES

### **SOLUCIÓN 1: Script Mejorado (v4.0)** ? IMPLEMENTADA

El script ahora:
- ? Muestra información detallada del archivo fuente
- ? Muestra si ya existe una versión anterior
- ? Compara tamaños y fechas
- ? No oculta errores (`>nul` removido)
- ? Usa rutas absolutas como fallback
- ? Mejor diagnóstico de errores

---

### **SOLUCIÓN 2: Usar verificar_macro.bat** (NUEVO)

Antes de usar la macro, ejecuta:

```cmd
verificar_macro.bat
```

Este script:
1. ? Verifica que el archivo fuente existe
2. ? Verifica que la macro está instalada
3. ? **Compara los dos archivos** (más importante)
4. ? Te dice si necesitas actualizar

---

### **SOLUCIÓN 3: Método Manual (100% Seguro)**

Si el script `.bat` falla, usa PowerShell:

```powershell
# Copiar con sobrescritura forzada
Copy-Item "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs" "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\" -Force

# Verificar
$source = "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs"
$dest = "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"

Write-Host "Tamaño fuente: $((Get-Item $source).Length) bytes"
Write-Host "Tamaño destino: $((Get-Item $dest).Length) bytes"

if ((Get-FileHash $source).Hash -eq (Get-FileHash $dest).Hash) {
    Write-Host "? Archivos idénticos - Actualizado correctamente" -ForegroundColor Green
} else {
    Write-Host "? Archivos diferentes - Problema en la copia" -ForegroundColor Red
}
```

---

## ?? DIAGNOSTICAR EL PROBLEMA

### **Paso 1: Ejecutar verificar_macro.bat**

```cmd
cd "CORRECTOR DE ATRIBUTOS"
verificar_macro.bat
```

El script mostrará:
```
[1] Archivo fuente: ?/?
    Tamaño: XXX bytes
    Fecha: ...

[2] Archivo destino: ?/?
    Tamaño: XXX bytes
    Fecha: ...

[3] Comparación: IDÉNTICOS / DIFERENTES
```

---

### **Paso 2: Identificar la causa**

| Resultado | Causa | Solución |
|-----------|-------|----------|
| Fuente no existe | Archivo eliminado/movido | Regenerar archivo |
| Destino no existe | Nunca instalado | Ejecutar instalador |
| **Archivos diferentes** | **No se actualizó** | **Reinstalar** |
| Archivos idénticos | Todo correcto | Solo reiniciar Tekla |

---

## ?? FLUJO CORRECTO DE ACTUALIZACIÓN

### **Si modificaste la macro**:

```
1. Editar: MacroPlantilla\SyncWeldPhaseFromParts.cs

2. Verificar cambios:
   verificar_macro.bat
   ? Debe decir "DIFERENTES"

3. Actualizar:
   instalar_macro.bat (v4.0)
   ? Verás el proceso completo

4. Verificar de nuevo:
   verificar_macro.bat
   ? Ahora debe decir "IDÉNTICOS"

5. Reiniciar Tekla
```

---

## ?? PROBLEMAS COMUNES

### **Problema 1: "Archivo en uso"**

**Causa**: Tekla tiene la macro cargada

**Solución**:
```
1. Cerrar Tekla completamente
2. Ejecutar instalar_macro.bat
3. Volver a abrir Tekla
```

---

### **Problema 2: "Permisos denegados"**

**Causa**: Necesitas permisos de Administrador

**Solución**:
```
Click derecho en instalar_macro.bat
? "Ejecutar como administrador"
```

---

### **Problema 3: "El archivo se copia pero Tekla no ve los cambios"**

**Causa**: Tekla cachea las macros compiladas

**Solución**:
```
1. Cerrar Tekla
2. Eliminar archivos compilados:
   Del "C:\ProgramData\...\modeling\SyncWeldPhaseFromParts.cs.dll"
   Del "C:\ProgramData\...\modeling\SyncWeldPhaseFromParts.cs.pdb"
3. Reinstalar con instalar_macro.bat
4. Abrir Tekla
```

---

## ?? SCRIPTS DISPONIBLES

### **1. instalar_macro.bat (v4.0)** - Instalar/Actualizar
```
- Busca archivo fuente
- Muestra tamaños y fechas
- Copia con sobrescritura
- Verifica resultado
```

### **2. verificar_macro.bat (NUEVO)** - Diagnosticar
```
- Compara fuente vs instalado
- Detecta si necesitas actualizar
- Muestra información detallada
```

### **3. Comando PowerShell Manual** - 100% Seguro
```
Copy-Item con -Force
Verificación con Get-FileHash
```

---

## ? VERIFICACIÓN FINAL

Después de actualizar, ejecuta:

### **En PowerShell**:
```powershell
$source = "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs"
$dest = "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"

Write-Host "Hash fuente:  $((Get-FileHash $source).Hash)"
Write-Host "Hash destino: $((Get-FileHash $dest).Hash)"

if ((Get-FileHash $source).Hash -eq (Get-FileHash $dest).Hash) {
    Write-Host "? ACTUALIZADO CORRECTAMENTE" -ForegroundColor Green
} else {
    Write-Host "? NO ACTUALIZADO" -ForegroundColor Red
}
```

---

## ?? CHECKLIST DE ACTUALIZACIÓN

- [ ] Ejecutar `verificar_macro.bat` (ver estado actual)
- [ ] Si dice "DIFERENTES":
  - [ ] Cerrar Tekla
  - [ ] Ejecutar `instalar_macro.bat` (v4.0)
  - [ ] Verificar que dice "copiado correctamente"
  - [ ] Ejecutar `verificar_macro.bat` otra vez
  - [ ] Debe decir "IDÉNTICOS"
- [ ] Reiniciar Tekla
- [ ] Probar macro

---

## ?? RESUMEN

**Scripts actualizados**:
- ? `instalar_macro.bat` v4.0 - Mejor diagnóstico
- ? `verificar_macro.bat` - Comparación de archivos

**Proceso recomendado**:
1. `verificar_macro.bat` ? Ver si necesitas actualizar
2. `instalar_macro.bat` ? Actualizar si es necesario
3. `verificar_macro.bat` ? Confirmar actualización
4. Reiniciar Tekla

---

**Versión**: v4.0  
**Scripts**: instalar_macro.bat + verificar_macro.bat  
**Estado**: ? MEJORADO Y VERIFICABLE
