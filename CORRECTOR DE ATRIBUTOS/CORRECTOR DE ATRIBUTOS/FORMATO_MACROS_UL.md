# ?? INFORMACIÓN IMPORTANTE: FORMATO DE ARCHIVOS DE TEKLA

## ? ACLARACIÓN IMPORTANTE

Tekla Structures usa **DOS tipos diferentes** de archivos ejecutables:

### 1. **MACROS** ? Extensión `.cs`
- **Ubicación**: `C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\`
- **Formato**: Archivos C# estándar con extensión `.cs`
- **Propósito**: Automatizar tareas en Tekla
- **Ejemplo**: `SyncWeldPhaseFromParts.cs`

### 2. **COMPONENTES** ? Extensión `.uel`
- **Ubicación**: `C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\USA\Common\General\Macros\modeling\`
- **Formato**: Archivos de componentes personalizados con extensión `.uel`
- **Propósito**: Crear componentes personalizados (custom components)
- **Ejemplo**: `CustomComponent.uel`

---

## ?? ESTE PROYECTO USA: MACROS (`.cs`)

Nuestro sistema genera **UNA MACRO UNIVERSAL INTELIGENTE** (no componentes), por lo tanto:

? **Extensión correcta**: `.cs`  
? **Ubicación correcta**: `C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\`  
? **Nombre**: `SyncWeldPhaseFromParts.cs`  
? **Ventaja**: UNA sola macro para TODOS los Phases

---

## ?? UBICACIÓN Y FORMATO CORRECTO

### Ruta Completa para la MACRO UNIVERSAL:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs
```

### Archivo Generado:
```
?? common\macros\modeling\
   ??? SyncWeldPhaseFromParts.cs                    ? Macro universal - Formato correcto
   ??? INSTRUCCIONES_SyncWeldPhaseFromParts.txt    ? Guía de uso
```

---

## ?? DIFERENCIAS: MACROS vs COMPONENTES

| Aspecto | MACROS (`.cs`) | COMPONENTES (`.uel`) |
|---------|----------------|----------------------|
| **Propósito** | Automatizar tareas | Crear objetos personalizados |
| **Extensión** | `.cs` | `.uel` |
| **Ubicación** | `common\macros\` | `USA\Common\General\Macros\modeling\` |
| **Acceso en Tekla** | `Tools > Macros...` | `Applications & components` |
| **Nuestro uso** | ? SÍ | ? NO |

---

## ?? QUÉ CAMBIÓ EN EL CÓDIGO

### Corrección Final:
```csharp
// Ubicación correcta para MACROS
string commonMacroDir = @"C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros";

// Extensión correcta para MACROS
string macroPath = Path.Combine(userMacroDir, $"{macroName}.cs");
```

---

## ? VALIDACIÓN DEL SISTEMA

El sistema ahora:

1. ? Genera macros con extensión `.cs` (correcto para macros)
2. ? Las guarda en `common\macros\` (correcto para macros)
3. ? Las macros aparecen en `Tools > Macros...` en Tekla
4. ? Se pueden ejecutar inmediatamente

---

## ?? FLUJO ACTUALIZADO

```
1. Ejecutar sincronizador
   ?
2. Sistema detecta soldaduras pendientes
   ?
3. Sistema genera: AutoChangeWeldsToPhase{N}.cs  ? Formato correcto de MACRO
   ?
4. Archivo se guarda en: common\macros\  ? Ubicación correcta de MACROS
   ?
5. Macro aparece en Tools > Macros...  ? Visible automáticamente
   ?
6. Usuario selecciona soldaduras y ejecuta macro
   ?
7. ? Soldaduras actualizadas
```

---

## ?? CÓMO VERIFICAR

### Verificar que la macro se generó correctamente:

**En PowerShell:**
```powershell
Get-ChildItem "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\" -Filter "SyncWeld*.cs"
```

**Esperado**: 
```
SyncWeldPhaseFromParts.cs
```

**En Tekla:**
1. Abre Tekla Structures
2. `Tools` > `Macros...`
3. ? Deberías ver `AutoChangeWeldsToPhase1`, `AutoChangeWeldsToPhase2`, etc. en la lista

---

## ?? DOCUMENTACIÓN ACTUALIZADA

Todos los documentos ahora reflejan correctamente:
- ? Extensión: `.cs` (para macros)
- ? Ubicación: `common\macros\` (para macros)
- ? Diferencia entre macros y componentes clarificada

---

## ?? CONCLUSIÓN

**CORRECCIÓN FINAL IMPLEMENTADA**: 

- ? **Macros** usan extensión `.cs`
- ? **Componentes** usan extensión `.uel`
- ? Nuestro sistema genera **MACROS**, por lo tanto usa `.cs`
- ? Ubicación correcta: `common\macros\`

**NO SE REQUIERE ACCIÓN ADICIONAL**: El sistema ahora genera archivos en el formato y ubicación correctos.

---

**Versión**: 1.0.2  
**Fecha**: 2024  
**Estado**: ? CORREGIDO Y VERIFICADO

