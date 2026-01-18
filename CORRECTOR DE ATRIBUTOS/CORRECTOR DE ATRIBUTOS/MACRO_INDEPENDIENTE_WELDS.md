# ?? MACRO INDEPENDIENTE: SINCRONIZAR PHASE DE SOLDADURAS

## ? SOLUCIÓN INDEPENDIENTE

Esta es una **macro completamente independiente** del sincronizador de assemblies.

**Propósito**: Sincronizar el Phase de las soldaduras leyendo automáticamente el Phase de las piezas conectadas.

---

## ?? ¿QUÉ HACE LA MACRO?

### Funcionalidad Principal:
1. ? **Lee soldaduras** (seleccionadas o todas del modelo)
2. ? **Para cada soldadura**:
   - Obtiene la pieza principal (MainPart)
   - Lee el Phase de esa pieza
   - Si no tiene Phase, lee la pieza secundaria (SecondaryPart)
3. ? **Asigna automáticamente** el Phase detectado a la soldadura
4. ? **Guarda cambios** con `weld.Modify()` y `model.CommitChanges()`

### Alcance Configurable:
- **Opción 1**: Solo soldaduras **SELECCIONADAS**
- **Opción 2**: **TODAS** las soldaduras del modelo

---

## ?? ARCHIVOS

### Plantilla de la Macro:
```
CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs
```

### Ubicación Final en Tekla:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\SyncWeldPhaseFromParts.cs
```

---

## ?? INSTALACIÓN

### Método 1: Script Automático (Recomendado)
```cmd
cd "CORRECTOR DE ATRIBUTOS"
.\instalar_macro.bat
```

El script:
1. ? Copia la macro al directorio de Tekla
2. ? Crea el directorio si no existe
3. ? Muestra instrucciones de uso

### Método 2: Manual
```cmd
copy "MacroPlantilla\SyncWeldPhaseFromParts.cs" "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\"
```

---

## ?? CÓMO USAR

### Paso 1: Instalar (UNA VEZ)
```cmd
.\instalar_macro.bat
```

### Paso 2: Reiniciar Tekla (UNA VEZ)
```
- Cerrar Tekla Structures
- Volver a abrir Tekla
```

### Paso 3: Usar (SIEMPRE QUE NECESITES)

#### Opción A: Procesar Seleccionadas
```
1. Seleccionar soldaduras en Tekla
2. Tools > Macros...
3. Seleccionar: SyncWeldPhaseFromParts
4. Click "Run"
5. Click "SÍ" (solo seleccionadas)
6. ? Reporte muestra cambios
```

#### Opción B: Procesar Todo el Modelo
```
1. Tools > Macros...
2. Seleccionar: SyncWeldPhaseFromParts
3. Click "Run"
4. Click "NO" (todas las soldaduras)
5. ? Reporte muestra cambios
```

---

## ?? EJEMPLO DE USO

### Escenario: Modelo con 100 soldaduras en diferentes Phases

#### Usuario ejecuta:
```
Tools > Macros > SyncWeldPhaseFromParts > Run
```

#### Macro pregunta:
```
¿Qué deseas procesar?

SÍ = Solo soldaduras SELECCIONADAS
NO = TODAS las soldaduras del modelo

Cancelar = Salir
```

#### Usuario elige: "NO" (todas)

#### Macro procesa:
```
Procesando 100 soldaduras...

Soldadura 1: Phase 0 ? 1 (de MainPart 45)
Soldadura 2: Phase 0 ? 1 (de MainPart 45)
Soldadura 3: Phase 2 ? 2 (ya correcta)
Soldadura 4: Phase 0 ? 2 (de SecondaryPart 78)
...
```

#### Reporte final:
```
???????????????????????????????????????
  SINCRONIZACIÓN DE PHASE - SOLDADURAS
  Alcance: TODO EL MODELO
???????????????????????????????????????

Soldaduras procesadas: 100
Soldaduras actualizadas: 65
Soldaduras omitidas (ya correctas): 30
Soldaduras sin Phase en piezas: 5

? Cambios guardados en el modelo.

???????????????????????????????????????
DETALLES DE CAMBIOS:
???????????????????????????????????????
? Weld 123: Phase 0 ? 1 (de MainPart 45)
? Weld 124: Phase 0 ? 1 (de MainPart 45)
? Weld 125: Phase 0 ? 2 (de SecondaryPart 78)
...
```

---

## ?? ALGORITMO

```csharp
// Pseudo-código simplificado

foreach (soldadura in soldaduras_a_procesar)
{
    // 1. Leer Phase de piezas conectadas
    Part mainPart = soldadura.MainObject;
    Part secondaryPart = soldadura.SecondaryObject;
    
    int targetPhase = 0;
    
    // Prioridad: MainPart
    if (mainPart != null)
    {
        targetPhase = mainPart.GetPhase();
    }
    
    // Si MainPart no tiene Phase, usar SecondaryPart
    if (targetPhase == 0 && secondaryPart != null)
    {
        targetPhase = secondaryPart.GetPhase();
    }
    
    // Si ninguna tiene Phase, omitir
    if (targetPhase == 0)
    {
        continue;
    }
    
    // 2. Leer Phase actual de la soldadura
    int currentPhase = soldadura.GetPhase();
    
    // 3. Si ya tiene el Phase correcto, omitir
    if (currentPhase == targetPhase)
    {
        continue;
    }
    
    // 4. Asignar nuevo Phase
    soldadura.SetUserProperty("PHASE", targetPhase);
    soldadura.Modify();
    
    // Registrar cambio
    Log($"? Weld {soldadura.ID}: Phase {currentPhase} ? {targetPhase}");
}

// 5. Guardar todos los cambios
model.CommitChanges();

// 6. Mostrar reporte
ShowReport();
```

---

## ? VENTAJAS

| Característica | Descripción |
|----------------|-------------|
| **Independiente** | No depende del sincronizador de assemblies |
| **Flexible** | Procesa seleccionadas o todo el modelo |
| **Automática** | Detecta Phase de piezas conectadas |
| **Universal** | Funciona para CUALQUIER Phase |
| **Directa** | Usa `Modify()` directamente en soldaduras |
| **Completa** | Guarda cambios automáticamente |
| **Informativa** | Reporte detallado de cambios |

---

## ?? DIFERENCIA CON EL SINCRONIZADOR

### Sincronizador de Assemblies:
- ? Procesa Parts, Bolts, Welds
- ? Basado en Main Part del assembly
- ? Procesamiento por assembly
- ?? Welds usan Phase Manager (limitación API)

### Esta Macro (Independiente):
- ? Solo procesa Welds
- ? Lee Phase de piezas conectadas
- ? Procesamiento directo con `Modify()`
- ? Alcance configurable (seleccionadas o todas)
- ? **Funciona directamente sin Phase Manager**

---

## ?? CASOS DE USO

### Caso 1: Sincronizar Seleccionadas
```
Situación: Algunas soldaduras tienen Phase incorrecto
Solución:
1. Seleccionar solo esas soldaduras
2. Ejecutar macro
3. Elegir "SÍ" (solo seleccionadas)
4. ? Solo esas se actualizan
```

### Caso 2: Sincronizar Todo el Modelo
```
Situación: Modelo nuevo, soldaduras sin Phase
Solución:
1. Ejecutar macro
2. Elegir "NO" (todas)
3. ? Todas las soldaduras se sincronizan
```

### Caso 3: Verificación Periódica
```
Situación: Verificar que todo esté sincronizado
Solución:
1. Ejecutar macro (todas)
2. Reporte muestra:
   - Cuántas ya están correctas
   - Cuántas se actualizaron
   - Cuáles no tienen Phase
```

---

## ?? REPORTE DETALLADO

La macro genera un reporte con:

### Estadísticas:
- ? Total de soldaduras procesadas
- ? Soldaduras actualizadas
- ? Soldaduras ya correctas (omitidas)
- ? Soldaduras sin Phase en piezas

### Detalles (solo si hay cambios):
- ? ID de cada soldadura actualizada
- ? Phase anterior y nuevo
- ? De qué pieza se obtuvo el Phase

---

## ?? MANEJO DE ERRORES

### Soldaduras sin Piezas Conectadas:
```
? Weld 123: No se pudieron obtener las piezas conectadas
```

### Piezas sin Phase:
```
? Weld 124: Las piezas conectadas no tienen Phase asignada
```

### Error en Modify():
```
? Weld 125: Modify() devolvió false
```

---

## ? VERIFICACIÓN

### Para verificar que la macro funciona:

```
1. Seleccionar una soldadura
2. Verificar su Phase actual
3. Ejecutar macro
4. Verificar que el Phase cambió
5. Confirmar que coincide con la pieza conectada
```

---

## ?? INSTALACIÓN RÁPIDA

```cmd
cd "CORRECTOR DE ATRIBUTOS"
.\instalar_macro.bat
```

Luego:
1. Reiniciar Tekla
2. Tools > Macros > SyncWeldPhaseFromParts
3. ¡Listo!

---

## ?? RESUMEN

**UNA SOLA MACRO** que:
- ? Funciona independiente del sincronizador
- ? Lee Phase de piezas conectadas
- ? Asigna automáticamente a soldaduras
- ? Procesa seleccionadas o todas
- ? Guarda cambios directamente
- ? Reporte detallado
- ? Una para TODAS las fases

---

**Archivo**: `SyncWeldPhaseFromParts.cs`  
**Tipo**: Macro independiente  
**Alcance**: Configurable (seleccionadas o todas)  
**Estado**: ? LISTA PARA USAR
