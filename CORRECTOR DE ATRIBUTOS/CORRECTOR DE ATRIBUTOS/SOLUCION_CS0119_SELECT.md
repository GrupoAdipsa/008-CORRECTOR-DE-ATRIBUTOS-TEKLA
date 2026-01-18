# ? SOLUCIÓN: Error CS0119 - Select() en Tekla 2021

## ?? PROBLEMA

**Error**: CS0119 - 'Select' is not valid in the given context

**Líneas**: 192, 210

```csharp
// ? INCORRECTO (causaba CS0119)
w.Select.Select();
w.Select.Unselect();
```

---

## ?? CAUSA

En Tekla 2021, no se puede usar `objeto.Select.Select()` o `objeto.Select.Unselect()`.

**Método correcto**: Usar `ModelObjectSelector.Select(ArrayList)`

---

## ? SOLUCIÓN IMPLEMENTADA

### **Antes** (causaba CS0119):

```csharp
// Seleccionar objetos (INCORRECTO)
foreach (BaseWeld w in weldsToChange)
{
    w.Select.Select();  // ? Error CS0119
}

// ...Phase Manager...

// Deseleccionar (INCORRECTO)
foreach (BaseWeld w in weldsToChange)
{
    w.Select.Unselect();  // ? Error CS0119
}
```

---

### **Ahora** (CORRECTO para Tekla 2021):

```csharp
// Crear selector de UI
ModelObjectSelector uiSelector = new ModelObjectSelector();

// Crear ArrayList con objetos a seleccionar
ArrayList objectsToSelect = new ArrayList();
foreach (BaseWeld w in weldsToChange)
{
    objectsToSelect.Add(w);
}

// Seleccionar usando el selector
uiSelector.Select(objectsToSelect);

// ...Phase Manager...

// Deseleccionar (seleccionar ArrayList vacío)
uiSelector.Select(new ArrayList());
```

---

## ?? CÓDIGO COMPLETO CORREGIDO

```csharp
// PASO 2: Aplicar Phase usando Phase Manager por grupos
if (weldsByPhase.Count > 0)
{
    // Crear selector de UI una vez
    ModelObjectSelector uiSelector = new ModelObjectSelector();
    
    foreach (var kvp in weldsByPhase)
    {
        int targetPhase = kvp.Key;
        List<BaseWeld> weldsToChange = kvp.Value;

        try
        {
            // Crear ArrayList con los objetos a seleccionar
            ArrayList objectsToSelect = new ArrayList();
            foreach (BaseWeld w in weldsToChange)
            {
                objectsToSelect.Add(w);
            }
            
            // ? CORRECTO: Seleccionar usando el selector
            uiSelector.Select(objectsToSelect);

            // Usar Phase Manager
            wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");
            akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { targetPhase - 1 });
            akit.PushButton("butModifyObjects", "diaPhaseManager");
            akit.PushButton("butOk", "diaPhaseManager");

            // ? CORRECTO: Deseleccionar (ArrayList vacío)
            uiSelector.Select(new ArrayList());

            weldsChanged += weldsToChange.Count;
            log.AppendLine(string.Format("==> {0} soldaduras cambiadas a Phase {1}", 
                weldsToChange.Count, targetPhase));
        }
        catch (Exception ex)
        {
            weldsSkipped += weldsToChange.Count;
            log.AppendLine(string.Format("ERROR al aplicar Phase {0}: {1}", targetPhase, ex.Message));
        }
    }

    model.CommitChanges();
}
```

---

## ?? MÉTODOS DE SELECCIÓN EN TEKLA 2021

### **? NO funciona**:
```csharp
object.Select();           // No existe
object.Select.Select();    // CS0119
object.Select.Unselect();  // CS0119
```

### **? SÍ funciona**:
```csharp
// Método 1: Selector con ArrayList
ModelObjectSelector selector = new ModelObjectSelector();
ArrayList objects = new ArrayList();
objects.Add(weld1);
objects.Add(weld2);
selector.Select(objects);

// Método 2: Deseleccionar todo
selector.Select(new ArrayList());

// Método 3: Obtener selección actual
ModelObjectEnumerator selected = selector.GetSelectedObjects();
```

---

## ?? POR QUÉ ArrayList

Tekla 2021 usa **ArrayList** (colección antigua de .NET) en lugar de `List<T>`.

**Razón**: Compatibilidad con versiones antiguas de Tekla.

```csharp
// ? CORRECTO para Tekla 2021
ArrayList objects = new ArrayList();

// ? NO funciona directamente
List<BaseWeld> objects = new List<BaseWeld>();
// (necesitaría convertir a ArrayList)
```

---

## ?? CHECKLIST DE SELECCIÓN

Cuando trabajes con selección en Tekla 2021:

- [ ] ? Usar `ModelObjectSelector` de `Tekla.Structures.Model.UI`
- [ ] ? Crear `ArrayList` para los objetos
- [ ] ? Llamar `selector.Select(arrayList)`
- [ ] ? Para deseleccionar: `selector.Select(new ArrayList())`
- [ ] ? **NO** usar `objeto.Select.Select()`
- [ ] ? **NO** usar `objeto.Select.Unselect()`

---

## ?? PRÓXIMOS PASOS

### **1. Reinstalar macro corregida**:

```cmd
cd "C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
instalar_macro.bat
```

### **2. Reiniciar Tekla**:
```
- Cerrar completamente
- Volver a abrir
```

### **3. Probar**:
```
Tools > Macros > SyncWeldPhaseFromParts > Run
```

---

## ? VERIFICACIÓN

Después de reinstalar, la macro debe:
1. ? Compilar sin errores CS0119
2. ? Seleccionar soldaduras correctamente
3. ? Abrir Phase Manager
4. ? Aplicar Phase
5. ? Deseleccionar
6. ? Mostrar reporte

---

## ?? COMPARACIÓN

| Método | Tekla 2021 | Notas |
|--------|------------|-------|
| `objeto.Select.Select()` | ? CS0119 | No existe |
| `objeto.Select.Unselect()` | ? CS0119 | No existe |
| `selector.Select(ArrayList)` | ? **Correcto** | **Usar este** |
| `selector.Select(List<T>)` | ? No compatible | Necesita ArrayList |

---

## ?? RESUMEN

**Error**: CS0119 con `Select.Select()`  
**Causa**: Método incorrecto para Tekla 2021  
**Solución**: Usar `ModelObjectSelector.Select(ArrayList)`  
**Estado**: ? CORREGIDO

---

**Archivo actualizado**: `MacroPlantilla\SyncWeldPhaseFromParts.cs`  
**Próximo paso**: Ejecutar `instalar_macro.bat` y reiniciar Tekla ??
