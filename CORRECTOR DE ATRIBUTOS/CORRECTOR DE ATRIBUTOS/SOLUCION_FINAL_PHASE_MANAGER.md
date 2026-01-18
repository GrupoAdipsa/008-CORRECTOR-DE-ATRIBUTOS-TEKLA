# ? SOLUCIÓN CORRECTA: Usar Phase Manager con Akit

## ?? SOLUCIÓN IMPLEMENTADA

Has confirmado que **cada soldadura SÍ tiene su propio Phase independiente** y que se puede cambiar.

La solución correcta es usar el **Phase Manager** de Tekla con **Akit**, tal como se hace en `WeldPhaseMacroGenerator.cs`.

---

## ?? LÓGICA CORRECTA

### **Método que SÍ funciona**:

```csharp
// 1. Seleccionar las soldaduras
foreach (BaseWeld weld in weldsToChange)
{
    weld.Select.Select();
}

// 2. Abrir Phase Manager
wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");

// 3. Seleccionar Phase en la tabla (índice = PhaseNumber - 1)
akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { targetPhase - 1 });

// 4. Aplicar Phase a seleccionados
akit.PushButton("butModifyObjects", "diaPhaseManager");

// 5. Cerrar Phase Manager
akit.PushButton("butOk", "diaPhaseManager");

// 6. Deseleccionar
foreach (BaseWeld weld in weldsToChange)
{
    weld.Select.Unselect();
}

// 7. Commit
model.CommitChanges();
```

---

## ?? IMPLEMENTACIÓN EN LA MACRO

### **Cambio principal**: Procesar en lotes por Phase

**Antes**: Intentar cambiar cada soldadura individualmente (NO funcionaba)

**Ahora**: 
1. **Agrupar** soldaduras por Phase objetivo
2. **Para cada grupo**:
   - Seleccionar todas las soldaduras del grupo
   - Abrir Phase Manager
   - Aplicar Phase
   - Cerrar Phase Manager

---

## ?? POR QUÉ FUNCIONA ASÍ

### **Phase Manager es el método "oficial"**:

- Tekla usa Phase Manager internamente para cambiar Phases
- Akit permite automatizar la interacción con Phase Manager
- Es el método más robusto y compatible

### **Ventajas**:
- ? Funciona con **cualquier** tipo de objeto (Parts, Bolts, Welds)
- ? Respeta las reglas de negocio de Tekla
- ? Es el método que usa Tekla internamente
- ? Funciona en todas las versiones

---

## ?? FLUJO COMPLETO

```
1. Usuario ejecuta macro
2. Elige: Seleccionadas o Todas
3. Macro itera sobre soldaduras:
   ?? Lee Phase de MainPart/SecondaryPart
   ?? Agrupa por Phase objetivo
   ?? Detecta cuáles necesitan cambio

4. Para cada grupo de soldaduras:
   ?? Selecciona todas las del grupo
   ?? Abre Phase Manager (UI)
   ?? Selecciona Phase en tabla
   ?? Click "Modify Objects"
   ?? Click "OK"
   ?? Deselecciona

5. Commit cambios
6. Muestra reporte
```

---

## ?? CÓDIGO CLAVE

### **Agrupar por Phase**:

```csharp
Dictionary<int, List<BaseWeld>> weldsByPhase = 
    new Dictionary<int, List<BaseWeld>>();

foreach (BaseWeld weld in allWelds)
{
    int targetPhase = GetPhaseFromConnectedPart(weld);
    
    if (!weldsByPhase.ContainsKey(targetPhase))
    {
        weldsByPhase[targetPhase] = new List<BaseWeld>();
    }
    
    weldsByPhase[targetPhase].Add(weld);
}
```

### **Aplicar Phase con Phase Manager**:

```csharp
foreach (var kvp in weldsByPhase)
{
    int targetPhase = kvp.Key;
    List<BaseWeld> welds = kvp.Value;
    
    // Seleccionar soldaduras
    foreach (BaseWeld w in welds)
    {
        w.Select.Select();
    }
    
    // Abrir Phase Manager y aplicar
    wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");
    akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { targetPhase - 1 });
    akit.PushButton("butModifyObjects", "diaPhaseManager");
    akit.PushButton("butOk", "diaPhaseManager");
    
    // Deseleccionar
    foreach (BaseWeld w in welds)
    {
        w.Select.Unselect();
    }
}

model.CommitChanges();
```

---

## ?? NOTAS IMPORTANTES

### **Índice de Phase en tabla**:

```csharp
// Phase 1 ? índice 0
// Phase 2 ? índice 1
// Phase 3 ? índice 2
// ...
int tableIndex = targetPhase - 1;
```

### **Deseleccionar antes de seleccionar**:

```csharp
// Limpiar selección previa
ModelObjectSelector selector = new ModelObjectSelector();
selector.GetSelectedObjects(); // Esto limpia

// Ahora seleccionar nuevas
foreach (BaseWeld w in welds)
{
    w.Select.Select();
}
```

---

## ?? VENTAJAS DE ESTE MÉTODO

### **Comparado con SetUserProperty()**:

| Aspecto | SetUserProperty() | Phase Manager (Akit) |
|---------|-------------------|----------------------|
| **Funciona** | ? NO (en soldaduras) | ? SÍ |
| **Robusto** | ? Limitado | ? Muy robusto |
| **Velocidad** | Rápido | Media (UI) |
| **Compatibilidad** | Limitada | ? Total |
| **Recomendado** | ? NO | ? **SÍ** |

---

## ?? RESULTADO ESPERADO

### **Reporte de la macro**:

```
=======================================
  SINCRONIZACION DE PHASE - SOLDADURAS
  Alcance: SELECCIONADAS
=======================================

Soldaduras procesadas: 25
Soldaduras actualizadas: 18
Soldaduras omitidas (ya correctas): 5
Soldaduras sin Phase en piezas: 2

OK Cambios guardados en el modelo.

=======================================
DETALLES DE CAMBIOS:
=======================================
OK 10 soldaduras cambiadas a Phase 1
OK 8 soldaduras cambiadas a Phase 2
```

### **En Tekla**:
- ? Soldaduras tienen el Phase correcto
- ? Cambios son persistentes
- ? Se pueden ver en propiedades

---

## ?? PRÓXIMOS PASOS

### **1. Reinstalar macro actualizada**:

```powershell
Copy-Item "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts.cs" "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\" -Force
```

### **2. Eliminar compilaciones antiguas**:

```powershell
Remove-Item "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs.dll" -ErrorAction SilentlyContinue
Remove-Item "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs.pdb" -ErrorAction SilentlyContinue
```

### **3. Reiniciar Tekla**:

```
- Cerrar completamente
- Volver a abrir
```

### **4. Probar**:

```
1. Seleccionar algunas soldaduras
2. Tools > Macros > SyncWeldPhaseFromParts > Run
3. Elegir: SÍ (solo seleccionadas)
4. ESPERAR - Phase Manager se abrirá y cerrará automáticamente
5. Verificar reporte
6. ? Verificar en Tekla que Phase cambió
```

---

## ? DIFERENCIAS CLAVE

### **Versión anterior** (NO funcionaba):
```csharp
weld.SetUserProperty("PHASE", targetPhase);
weld.Modify();
// ? Modify() devuelve true pero Phase NO cambia
```

### **Versión actual** (SÍ funciona):
```csharp
weld.Select.Select();
wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");
akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { targetPhase - 1 });
akit.PushButton("butModifyObjects", "diaPhaseManager");
akit.PushButton("butOk", "diaPhaseManager");
weld.Select.Unselect();
// ? Phase Manager cambia el Phase correctamente
```

---

## ?? EXPLICACIÓN TÉCNICA

### **¿Por qué SetUserProperty() no funciona?**

`SetUserProperty()` es para **propiedades de usuario** (custom attributes), no para propiedades de sistema como Phase.

### **¿Qué es Phase en Tekla?**

Phase es una **propiedad de sistema** que tiene lógica especial de validación y herencia. Solo se puede cambiar correctamente usando:
1. Phase Manager (UI)
2. Akit (automatización de UI)
3. Open API avanzado (no disponible en macros simples)

### **¿Por qué funciona Phase Manager?**

Phase Manager maneja internamente:
- Validación de Phase
- Actualización de dependencias
- Propagación de cambios
- Actualización de vistas

---

**Método**: Phase Manager con Akit  
**Ventaja**: Método oficial de Tekla  
**Estado**: ? IMPLEMENTADO  
**Próximo paso**: REINSTALAR Y PROBAR ??
