# ?? DIAGNÓSTICO: Phase Manager selecciona el Phase incorrecto

## ?? PROBLEMA REPORTADO

**Síntoma**: La macro actualiza la soldadura **al mismo Phase que ya tenía**, NO al Phase que debería tener (el de la pieza conectada).

**Ejemplo**:
```
Soldadura actual: Phase 2
MainPart: Phase 2
Macro dice: "Phase 0 -> 2"
Resultado: Soldadura sigue en Phase 2 (no cambia)
```

---

## ?? POSIBLES CAUSAS

### **Causa 1: Lectura incorrecta del Phase actual**

El reporte dice "Phase 0 -> 2" pero la soldadura **realmente tiene Phase 2**.

**Verificación**:
1. Seleccionar la soldadura en Tekla
2. Ver propiedades ? Phase actual
3. Comparar con lo que dice el reporte

Si el reporte dice "Phase 0" pero Tekla muestra "Phase 2":
- **Problema**: `GetReportProperty("PHASE")` devuelve 0 siempre
- **Solución**: Ver Causa 4 abajo

---

### **Causa 2: Índice de tabla incorrecto**

Phase Manager usa índices basados en 0:
```
Phase 1 ? índice 0
Phase 2 ? índice 1
Phase 3 ? índice 2
```

Si targetPhase = 2:
```csharp
tableIndex = targetPhase - 1 = 2 - 1 = 1  // ? Correcto para Phase 2
```

Pero si los Phases no son consecutivos (1, 3, 5):
```
Phase 1 ? índice 0 ?
Phase 3 ? índice 1 ? (debería ser índice 2 si hay Phase 2 en tabla)
Phase 5 ? índice 2 ?
```

---

### **Causa 3: Phase Manager no aplica cambios**

Phase Manager se abre pero no hace el cambio porque:
- Delays insuficientes
- Tabla no cargó completamente
- Nombre de controles incorrectos

---

### **Causa 4: GetReportProperty devuelve 0 siempre**

En algunas versiones de Tekla, `GetReportProperty("PHASE")` puede devolver 0 incluso si la soldadura tiene Phase asignado.

---

## ? DIAGNÓSTICO PASO A PASO

### **Prueba 1: Verificar lectura de Phase**

Crea una macro de prueba simple:

```csharp
#pragma warning disable 1633
#pragma reference "Tekla.Macros.Runtime"
#pragma warning restore 1633

using System;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace UserMacros
{
    public sealed class Macro
    {
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime)
        {
            Model model = new Model();
            ModelObjectSelector selector = new ModelObjectSelector();
            ModelObjectEnumerator objs = selector.GetSelectedObjects();
            
            System.Text.StringBuilder log = new System.Text.StringBuilder();
            
            while (objs.MoveNext())
            {
                BaseWeld weld = objs.Current as BaseWeld;
                if (weld == null) continue;
                
                // Leer Phase actual
                int currentPhase = 0;
                bool success = weld.GetReportProperty("PHASE", ref currentPhase);
                
                // Leer piezas
                Part mainPart = weld.MainObject as Part;
                int mainPhase = 0;
                if (mainPart != null)
                {
                    mainPart.GetReportProperty("PHASE", ref mainPhase);
                }
                
                log.AppendLine(string.Format("Weld {0}:", weld.Identifier.ID));
                log.AppendLine(string.Format("  GetReportProperty success: {0}", success));
                log.AppendLine(string.Format("  Weld Phase: {0}", currentPhase));
                log.AppendLine(string.Format("  MainPart Phase: {0}", mainPhase));
                log.AppendLine("");
            }
            
            System.Windows.Forms.MessageBox.Show(log.ToString(), "Diagnostico Phase");
        }
    }
}
```

**Guardar como**: `DiagnosticPhase.cs` en `modeling\`

**Ejecutar**:
1. Seleccionar 1 soldadura
2. Ejecutar macro
3. Ver si `GetReportProperty` devuelve el Phase correcto

---

### **Prueba 2: Verificar índice de tabla**

Agregar logs en la macro principal (ya agregado):

```csharp
int tableIndex = targetPhase - 1;
log.AppendLine(string.Format("DEBUG: targetPhase={0}, tableIndex={1}", targetPhase, tableIndex));
akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { tableIndex });
```

**Ejecutar macro** y ver en el reporte:
```
DEBUG: targetPhase=2, tableIndex=1
```

Esto confirma qué índice se está usando.

---

### **Prueba 3: Verificar que Phase Manager aplica cambios**

**Método manual**:
1. Seleccionar 1 soldadura en Tekla
2. Tools > Phase Manager
3. Seleccionar Phase 2 manualmente
4. Click "Modify Objects"
5. Click "OK"
6. Verificar en propiedades si Phase cambió

Si **NO cambia manualmente**, entonces:
- La soldadura no se puede cambiar de Phase así
- Necesitas otro método

---

## ?? SOLUCIÓN SEGÚN CAUSA

### **Si Causa 1** (GetReportProperty devuelve 0):

```csharp
// Alternativa: usar GetUserProperty
int currentPhase = 0;
weld.GetUserProperty("PHASE", ref currentPhase);
```

---

### **Si Causa 2** (Índice incorrecto):

El problema es que `targetPhase - 1` asume Phases consecutivos.

**Solución**: Leer la tabla de Phases antes:

```csharp
// TODO: Obtener lista real de Phases del modelo
// y mapear Phase number ? table index
```

Esto es complejo. **Alternativa más simple**: No usar índice, usar otro método.

---

### **Si Causa 3** (Delays insuficientes):

Ya agregamos delays. Si no funciona, aumentar:

```csharp
System.Threading.Thread.Sleep(1000); // en vez de 500
System.Threading.Thread.Sleep(500);  // en vez de 300
```

---

### **Si Causa 4** (Phase Manager no funciona para soldaduras):

**Alternativa**: Cambiar el Phase del **Assembly completo**:

```csharp
// Obtener assembly
Assembly assembly = mainPart.GetAssembly();

// Cambiar Phase del assembly
assembly.SetUserProperty("PHASE", targetPhase);
assembly.Modify();

// TODAS las soldaduras heredarán el Phase
```

---

## ?? PRÓXIMOS PASOS

### **1. Ejecutar macro actualizada con logs de diagnóstico**:

```cmd
instalar_macro.bat
```

### **2. Probar con 1 soldadura**:

Seleccionar una soldadura y ejecutar.

### **3. Revisar el reporte**:

Buscar líneas como:
```
Weld 5312730: Actual=2, Target=2 (de MainPart 5312185)
DEBUG: targetPhase=2, tableIndex=1
```

### **4. Analizar**:

- Si `Actual = Target`: La soldadura **YA tiene** el Phase correcto (no debería estar en la lista)
- Si `Actual != Target` pero no cambia: Phase Manager no está funcionando

---

## ?? INFORMACIÓN NECESARIA

Para ayudarte mejor, necesito saber:

1. **¿Qué dice el reporte exacto?**
   ```
   Weld X: Actual=?, Target=? (de MainPart Y)
   DEBUG: targetPhase=?, tableIndex=?
   ```

2. **¿Qué Phase tiene realmente la soldadura en Tekla?**
   (Ver propiedades)

3. **¿Qué Phase tiene la MainPart?**
   (Ver propiedades)

4. **¿Cambia si lo haces manualmente con Phase Manager?**
   (Seleccionar soldadura ? Phase Manager ? Cambiar ? OK)

---

## ?? TEORÍA: ¿Por qué podría estar pasando esto?

### **Escenario A**: Soldadura ya tiene el Phase correcto

```
Weld Phase = 2 (actual)
MainPart Phase = 2
Macro: "Phase 0 -> 2" (lee mal el actual)
Resultado: Queda en 2 (no cambia porque ya está correcto)
```

**Solución**: Arreglar la lectura de `currentPhase`.

---

### **Escenario B**: Phase Manager selecciona el Phase equivocado

```
Weld Phase = 2 (actual)
MainPart Phase = 3
Macro: "Phase 2 -> 3"
Phase Manager selecciona: índice 2 = Phase 3 (correcto)
Pero aplica: Phase 2 (incorrecto)
```

**Solución**: Revisar delays o método de aplicación.

---

### **Escenario C**: Soldaduras no se pueden cambiar con Phase Manager

```
Weld Phase = 2 (heredado del Assembly)
MainPart Phase = 3
Phase Manager intenta cambiar pero Tekla lo ignora
```

**Solución**: Cambiar Phase del Assembly en lugar de la soldadura individual.

---

**Estado**: ? DIAGNÓSTICO EN CURSO  
**Próximo paso**: Ejecutar macro con logs y analizar reporte  
**Información necesaria**: Reporte exacto + Phase real en Tekla ??
