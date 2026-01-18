# ? ACTUALIZACIÓN: RUTA DE MACROS CAMBIADA

## ?? CAMBIO DE UBICACIÓN

### **ANTES**:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\
```

### **AHORA**:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\
```

---

## ?? RAZÓN DEL CAMBIO

La nueva ubicación `modeling` es más específica y organizada para macros de modelado en Tekla.

---

## ? ARCHIVOS ACTUALIZADOS

### 1. **instalar_macro.bat**
```batch
set TARGET=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs
```

### 2. **WeldPhaseMacroGenerator.cs**
```csharp
string commonMacroDir = @"C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling";
```

### 3. **Macro ya movida**
La macro `SyncWeldPhaseFromParts.cs` ya está en la nueva ubicación.

---

## ?? VERIFICACIÓN

### Verificar nueva ubicación:
```powershell
Test-Path "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"
```

**Resultado**: ? True

### Listar macros en modeling:
```powershell
Get-ChildItem "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling" -Filter "*.cs"
```

---

## ?? ESTADO ACTUAL

| Componente | Ubicación | Estado |
|-----------|-----------|--------|
| **Macro instalada** | `modeling\SyncWeldPhaseFromParts.cs` | ? Movida |
| **Script instalación** | `instalar_macro.bat` | ? Actualizado |
| **Generador C#** | `WeldPhaseMacroGenerator.cs` | ? Actualizado |
| **Proyecto** | Compilación | ? Sin errores |

---

## ?? PRÓXIMOS PASOS

### 1. Reiniciar Tekla (Necesario)
```
- Cerrar Tekla Structures
- Volver a abrir
- Abrir modelo
```

### 2. Verificar Macro en Tekla
```
Tools > Macros...
Buscar: SyncWeldPhaseFromParts
```

**Esperado**: Macro aparece en la lista

### 3. Usar la Macro
```
1. Seleccionar soldaduras (o no, si quieres todas)
2. Tools > Macros > SyncWeldPhaseFromParts
3. Run
4. Elegir: Seleccionadas (SÍ) o Todas (NO)
```

---

## ?? COMANDOS DE VERIFICACIÓN

### PowerShell completo:
```powershell
# Verificar directorio modeling existe
Test-Path "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling"

# Verificar macro existe
Test-Path "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"

# Listar todas las macros
Get-ChildItem "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling" -Filter "*.cs"
```

---

## ?? SI NECESITAS REINSTALAR

```cmd
cd "CORRECTOR DE ATRIBUTOS"
.\instalar_macro.bat
```

El script ahora usará la nueva ubicación automáticamente.

---

## ? CONFIRMACIÓN FINAL

```
? Ruta actualizada en código C#
? Ruta actualizada en script .bat
? Macro movida a nueva ubicación
? Proyecto compilado sin errores
? Sistema listo para usar
```

---

## ?? DOCUMENTOS A ACTUALIZAR

Los siguientes documentos mencionan la ruta antigua y deberían actualizarse:
- `FORMATO_MACROS_UL.md`
- `MACRO_INDEPENDIENTE_WELDS.md`
- `MACRO_INSTALADA_EXITO.md`
- `RESUMEN_FINAL_COMPLETO.md`

---

**Nueva ubicación**: `common\macros\modeling\`  
**Estado**: ? ACTUALIZADO Y FUNCIONAL  
**Próximo paso**: REINICIAR TEKLA ??
