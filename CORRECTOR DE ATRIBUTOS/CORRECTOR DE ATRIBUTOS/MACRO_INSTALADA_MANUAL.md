# ? MACRO INSTALADA - SOLUCIÓN FINAL

## ?? ESTADO ACTUAL

**? MACRO INSTALADA CORRECTAMENTE**

```
Ubicación: C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs
```

---

## ?? PROBLEMA IDENTIFICADO

El script PowerShell (`instalar_macro.ps1`) tenía un problema de rutas cuando se ejecutaba desde ciertos directorios.

### Error:
```
Archivo fuente no encontrado
Ruta esperada: C:\...\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\MacroPlantilla\...
```

### Causa:
Doble "CORRECTOR DE ATRIBUTOS" en la ruta debido a cómo se ejecutaba el script.

---

## ? SOLUCIÓN APLICADA

### Opción 1: Instalación Manual (USADA)
Se instaló manualmente con PowerShell:

```powershell
Copy-Item "MacroPlantilla\SyncWeldPhaseFromParts.cs" "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\" -Force
```

### Opción 2: Script Corregido
El script `instalar_macro.ps1` fue actualizado (v2.1) con:
- Búsqueda en múltiples rutas
- Mejor manejo de errores
- Rutas absolutas como fallback

---

## ?? PRÓXIMOS PASOS

### 1?? REINICIAR TEKLA (NECESARIO)
```
- Cerrar Tekla Structures
- Volver a abrir
- Abrir tu modelo
```

**¿Por qué?** Tekla solo detecta macros nuevas al iniciar.

---

### 2?? VERIFICAR EN TEKLA
```
1. Tools > Macros...
2. Buscar en la lista: SyncWeldPhaseFromParts
3. ? Debería aparecer
```

---

### 3?? USAR LA MACRO

#### Opción A: Solo Seleccionadas
```
1. Seleccionar soldaduras en el modelo
2. Tools > Macros > SyncWeldPhaseFromParts > Run
3. Click "SÍ" (solo seleccionadas)
```

#### Opción B: Todas
```
1. Tools > Macros > SyncWeldPhaseFromParts > Run
2. Click "NO" (todas las soldaduras)
```

---

## ?? SI NECESITAS REINSTALAR

### Método Más Simple (PowerShell directo):
```powershell
Copy-Item "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs" "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\" -Force
```

### Usando el Script .bat:
```cmd
cd "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS"
instalar_macro.bat
```

**Nota**: El `.bat` ahora tiene mejor búsqueda de rutas.

---

## ?? VERIFICACIÓN COMPLETA

### PowerShell:
```powershell
# Verificar que la macro está instalada
Test-Path "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"
# Debe devolver: True
```

### Resultado:
```
? Archivo fuente: Existe
? Macro instalada: SÍ
? Ubicación correcta: modeling\
? Scripts actualizados: v2.1 y v3.0
```

---

## ?? FUNCIONAMIENTO DE LA MACRO

```
1. Usuario ejecuta macro
2. Macro pregunta: Seleccionadas o Todas
3. Para cada soldadura:
   - Lee pieza conectada (MainPart o SecondaryPart)
   - Obtiene Phase de la pieza
   - Asigna Phase a la soldadura
4. Guarda cambios (Modify + CommitChanges)
5. Muestra reporte detallado
```

---

## ?? EJEMPLO DE REPORTE

```
???????????????????????????????????????
  SINCRONIZACIÓN DE PHASE - SOLDADURAS
  Alcance: SELECCIONADAS
???????????????????????????????????????

Soldaduras procesadas: 25
Soldaduras actualizadas: 18
Soldaduras omitidas (ya correctas): 5
Soldaduras sin Phase en piezas: 2

? Cambios guardados en el modelo.

???????????????????????????????????????
DETALLES DE CAMBIOS:
???????????????????????????????????????
? Weld 123: Phase 0 ? 1 (de MainPart 45)
? Weld 124: Phase 0 ? 1 (de MainPart 45)
...
```

---

## ?? CHECKLIST FINAL

- [x] ? Macro instalada en Tekla
- [x] ? Ubicación correcta (modeling\)
- [x] ? Scripts actualizados
- [ ] ? **REINICIAR TEKLA** (TU TURNO)
- [ ] ? Verificar en Tools > Macros
- [ ] ? Probar con soldaduras
- [ ] ? Verificar resultados

---

## ?? RESUMEN

**TODO LISTO**:

? Macro instalada manualmente  
? Scripts corregidos para futuro  
? Documentación completa  
? Sistema funcional  

**SOLO FALTA**:
1. Reiniciar Tekla
2. ¡Usar la macro!

---

## ?? DOCUMENTOS RELACIONADOS

1. `SISTEMA_LISTO_USAR.md` - Guía completa
2. `MACRO_LISTA_VERIFICADA.md` - Estado previo
3. `MACRO_INDEPENDIENTE_WELDS.md` - Documentación técnica
4. `RESUMEN_EJECUTIVO_FINAL.md` - Overview del sistema

---

**Estado**: ? INSTALADO MANUALMENTE  
**Scripts**: ? CORREGIDOS (v2.1/v3.0)  
**Próximo paso**: REINICIAR TEKLA ??
