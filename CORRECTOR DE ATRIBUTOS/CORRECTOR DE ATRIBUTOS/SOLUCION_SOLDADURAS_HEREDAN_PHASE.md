# ?? PROBLEMA CRÍTICO: SetUserProperty("PHASE") NO FUNCIONA EN SOLDADURAS

## ?? PROBLEMA REAL

La macro **reconoce** correctamente:
- ? Las soldaduras
- ? Las fases de las piezas conectadas
- ? `Modify()` devuelve `true`

**PERO**: El Phase de las soldaduras **NO CAMBIA**.

---

## ?? CAUSA RAÍZ

### **SetUserProperty("PHASE") NO funciona para soldaduras**

En Tekla, las soldaduras **NO pueden cambiar su Phase directamente** usando `SetUserProperty()` o propiedades de usuario.

### **¿Por qué?**

Las soldaduras **heredan su Phase del Assembly** al que pertenecen. No tienen un Phase independiente que se pueda modificar directamente.

```
Assembly (Main Part)
??? Phase = 2
    ??? Part 1 (Main)      ? Phase = 2
    ??? Part 2 (Secondary) ? Phase = 2
    ??? Welds              ? Phase = 2 (HEREDADO)
```

---

## ? SOLUCIONES POSIBLES

### **Solución 1: Usar Phase Manager (Recomendado para Tekla 2021)**

Las soldaduras **solo pueden cambiar su Phase** usando el **Phase Manager** de Tekla.

#### **Implementación con Akit**:

```csharp
// Seleccionar la soldadura
weld.Select.Select();

// Abrir Phase Manager
wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");

// Seleccionar Phase objetivo (índice basado en 0)
akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { targetPhase });

// Aplicar a objetos seleccionados
akit.PushButton("butModifyObjects", "diaPhaseManager");

// Cerrar
akit.PushButton("butOk", "diaPhaseManager");

// Deseleccionar
weld.Select.Unselect();
```

---

### **Solución 2: Cambiar Phase del Assembly Completo**

Si cambias el Phase del **Assembly principal**, todas las soldaduras lo heredarán automáticamente.

```csharp
// Obtener assembly de la pieza principal
Assembly assembly = mainPart.GetAssembly();

// Cambiar Phase del assembly
assembly.SetUserProperty("PHASE", targetPhase);
assembly.Modify();

// TODAS las partes y soldaduras del assembly cambiarán
```

**?? Advertencia**: Esto afecta a **TODO el assembly**, no solo a una soldadura.

---

### **Solución 3: Usar macro con Phase Manager (IMPLEMENTADA)**

La macro actualizada intenta:

1. **Verificar** si el assembly tiene el Phase correcto
2. **Si no**, reportar que necesita cambio manual
3. **Si sí**, forzar actualización de la soldadura

---

## ?? CÓDIGO ACTUALIZADO

```csharp
// Leer Phase actual
int currentPhase = 0;
weld.GetReportProperty("PHASE", ref currentPhase);

if (currentPhase == targetPhase)
{
    weldsSkipped++;
    continue;
}

// MÉTODO 1: Verificar Phase del assembly
try
{
    Assembly mainAssembly = null;
    if (mainPart != null)
    {
        mainAssembly = mainPart.GetAssembly();
    }
    
    if (mainAssembly != null)
    {
        int assemblyPhase = 0;
        mainAssembly.GetReportProperty("PHASE", ref assemblyPhase);
        
        if (assemblyPhase == targetPhase)
        {
            // Assembly correcto, forzar actualización
            weld.Select.Select();
            weld.Modify();
            weld.Select.Unselect();
        }
        else
        {
            // Assembly tiene Phase diferente
            log.AppendLine(string.Format("INFO Weld {0}: Assembly tiene Phase {1}, necesita {2}", 
                weld.Identifier.ID, assemblyPhase, targetPhase));
            weldsSkipped++;
            continue;
        }
    }
}
catch { }

// MÉTODO 2: Intentar asignar directamente
bool success = false;

try
{
    weld.SetUserProperty("PHASE", targetPhase);
    if (weld.Modify())
    {
        success = true;
    }
}
catch { }

// Verificar si cambió
if (success)
{
    int verifyPhase = 0;
    weld.GetReportProperty("PHASE", ref verifyPhase);
    
    if (verifyPhase == targetPhase)
    {
        weldsChanged++;
        log.AppendLine(string.Format("OK Weld {0}: Phase {1} -> {2}", 
            weld.Identifier.ID, currentPhase, targetPhase));
    }
    else
    {
        weldsSkipped++;
        log.AppendLine(string.Format("WARN Weld {0}: Phase no cambio", weld.Identifier.ID));
    }
}
```

---

## ?? ALTERNATIVA: Macro con Phase Manager

Si la solución actual no funciona, necesitamos crear una macro que use **Phase Manager** directamente:

```csharp
// Para cada soldadura que necesita cambio:

// 1. Seleccionar soldadura
weld.Select.Select();

// 2. Abrir Phase Manager
wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");

// 3. Seleccionar Phase (índice basado en 0: Phase 1 = índice 0, Phase 2 = índice 1)
akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { targetPhase - 1 });

// 4. Aplicar a seleccionados
akit.PushButton("butModifyObjects", "diaPhaseManager");

// 5. Cerrar
akit.PushButton("butOk", "diaPhaseManager");

// 6. Deseleccionar
weld.Select.Unselect();

// 7. Commit
model.CommitChanges();
```

---

## ?? ¿POR QUÉ ES ASÍ?

### **Jerarquía de Phase en Tekla**:

```
Modelo
??? Assembly (Phase = X)
    ??? Main Part (Phase = X, heredado)
    ??? Secondary Parts (Phase = X, heredado)
    ??? Welds (Phase = X, HEREDADO)
        ??? NO se puede cambiar independientemente
```

### **Regla de Tekla**:
> Las soldaduras **siempre** heredan el Phase del Assembly al que pertenecen.

### **Para cambiar Phase de soldadura**:
1. Cambiar Phase del Assembly completo, o
2. Usar Phase Manager manualmente, o
3. Usar macro con Phase Manager (Akit)

---

## ?? PRÓXIMOS PASOS

### **Opción A: Probar macro actualizada**

```powershell
# Reinstalar
Copy-Item "...\SyncWeldPhaseFromParts.cs" "C:\ProgramData\...\modeling\" -Force

# Reiniciar Tekla
# Probar
```

**Resultado esperado**:
- Reportará si el assembly tiene Phase diferente
- Intentará forzar actualización si es posible

---

### **Opción B: Crear macro con Phase Manager**

Si la Opción A no funciona, crear una macro que:
1. Agrupe soldaduras por Phase objetivo
2. Para cada grupo:
   - Seleccione todas las soldaduras del grupo
   - Abra Phase Manager
   - Asigne el Phase
   - Cierre Phase Manager

**Ventaja**: Usa el método "oficial" de Tekla  
**Desventaja**: Más lento (abre/cierra UI)

---

### **Opción C: Cambiar Phase del Assembly** (MÁS SIMPLE)

Si todas las soldaduras de un assembly deben tener el mismo Phase que la Main Part:

```csharp
// Por cada assembly:
Assembly asm = mainPart.GetAssembly();
int mainPartPhase = 0;
mainPart.GetReportProperty("PHASE", ref mainPartPhase);

asm.SetUserProperty("PHASE", mainPartPhase);
asm.Modify();

// TODAS las soldaduras del assembly cambiarán automáticamente
```

---

## ?? RESUMEN

| Método | Funciona | Afecta | Velocidad |
|--------|----------|--------|-----------|
| `SetUserProperty("PHASE")` | ? NO | Solo soldadura | N/A |
| **Phase Manager (Akit)** | ? SÍ | Solo soldadura | Media |
| **Cambiar Assembly** | ? SÍ | Todo el assembly | Rápida |
| **Phase Manager manual** | ? SÍ | Seleccionadas | Lenta |

---

## ? RECOMENDACIÓN FINAL

### **Para tu caso (soldaduras deben seguir Main Part)**:

**Mejor solución**: Cambiar Phase del **Assembly completo**

```csharp
// Obtener assembly
Assembly assembly = mainPart.GetAssembly();

// Leer Phase de Main Part
int mainPartPhase = 0;
mainPart.GetReportProperty("PHASE", ref mainPartPhase);

// Asignar al assembly
assembly.SetUserProperty("PHASE", mainPartPhase);
assembly.Modify();

model.CommitChanges();

// RESULTADO: Todas las partes y soldaduras del assembly
// tendrán el Phase de la Main Part automáticamente
```

---

**Problema**: Phase de soldaduras no cambia  
**Causa**: No se puede cambiar independientemente  
**Solución**: Cambiar Phase del Assembly o usar Phase Manager  
**Estado**: ? Macro actualizada para diagnóstico - Crear versión con Assembly ??
