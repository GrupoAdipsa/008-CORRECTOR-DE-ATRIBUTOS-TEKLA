# MACRO PARA CAMBIAR PHASE DE SOLDADURAS

Esta macro de Tekla te permite cambiar el Phase de soldaduras seleccionadas.

## ?? UBICACIÓN Y FORMATO DE LA MACRO

Las macros deben estar en:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\USA\Common\General\Macros\modeling\
```

**FORMATO**: Las macros de Tekla usan extensión **`.ul`** (no `.cs`)

Ejemplo: `AutoChangeWeldsToPhase2.ul`

**IMPORTANTE**: El sistema guarda automáticamente las macros en formato `.ul` en esta ubicación para que sean visibles inmediatamente en Tekla.

## ?? Cómo el Sistema Genera la Macro Automáticamente

Cuando ejecutas el sincronizador y hay soldaduras pendientes:

1. ? El sistema detecta automáticamente las soldaduras que necesitan actualización
2. ? Genera un archivo `.ul` con el código de la macro
3. ? Lo guarda en el directorio correcto de Tekla
4. ? La macro aparece automáticamente en `Tools > Macros...`

**NO necesitas crear la macro manualmente** - el sistema lo hace todo automáticamente.

## ?? Cómo Crear la Macro MANUALMENTE (si lo prefieres):

Solo si prefieres crear la macro manualmente en lugar de usar el sistema automático:

1. **Crea un archivo de texto** con el código de abajo
2. **Guárdalo con extensión `.ul`** en la ubicación especificada
3. **Nombre**: `AutoChangeWeldsToPhase2.ul` (cambia el 2 por tu phase)
4. **La macro aparecerá** en `Tools > Macros...` en Tekla

## Código de la Macro:

```csharp
#pragma warning disable 1633
#pragma reference "Tekla.Macros.Wpf.Runtime"
#pragma reference "Tekla.Macros.Akit"
#pragma reference "Tekla.Macros.Runtime"
#pragma warning restore 1633

namespace UserMacros {
    public sealed class Macro {
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            
            // Abrir Phase Manager
            wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");
            
            // IMPORTANTE: Cambia el número según tu Phase deseada
            // Para Phase 2, usa: new int[] { 2 }
            // Para Phase 3, usa: new int[] { 3 }
            // etc.
            akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { 2 });
            
            // Aplicar Phase a los objetos seleccionados
            akit.PushButton("butModifyObjects", "diaPhaseManager");
            akit.PushButton("butOk", "diaPhaseManager");
        }
    }
}
```

## Cómo usar la macro:

1. **Selecciona las soldaduras** que necesitan cambio de Phase en Tekla
2. **Ejecuta la macro**: `Tools` > `Macros...` > Selecciona `ChangeWeldsToPhase` > Run
3. La macro automáticamente:
   - Abre el Phase Manager
   - Selecciona Phase 2 (o el que hayas configurado)
   - Aplica el cambio a las soldaduras seleccionadas
   - Cierra el diálogo

## Crear macros para diferentes Phases:

Puedes crear múltiples versiones de esta macro para diferentes phases:

- `ChangeWeldsToPhase1.cs` ? `new int[] { 1 }`
- `ChangeWeldsToPhase2.cs` ? `new int[] { 2 }`
- `ChangeWeldsToPhase3.cs` ? `new int[] { 3 }`
- etc.

## Flujo de trabajo recomendado:

1. **Ejecuta el sincronizador de Phase** (`ejecutar.bat`)
   - Esto sincronizará automáticamente Parts y Bolts
   
2. **Lee el reporte** que muestra qué assemblies tienen soldaduras pendientes

3. **Para cada assembly con soldaduras pendientes**:
   - Selecciona todas las soldaduras del assembly
   - Ejecuta la macro `ChangeWeldsToPhase`
   
4. **Listo!** Todas las piezas, tornillos y soldaduras ahora tienen el Phase correcto

## Limitación de Tekla API:

La API de Tekla Structures **NO permite** cambiar el Phase de soldaduras programáticamente.
Por eso necesitamos usar macros que invocan el Phase Manager de Tekla.
Esta es una limitación conocida de Tekla, no un error en nuestro código.
