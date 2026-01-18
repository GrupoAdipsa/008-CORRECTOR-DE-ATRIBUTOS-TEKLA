# ?? CÓMO USAR MACROS EN TEKLA STRUCTURES

## ? ACLARACIÓN IMPORTANTE

Las macros **NO se importan** en Tekla - simplemente necesitan estar en el directorio correcto.

**Tekla las detecta automáticamente** cuando:
1. El archivo `.cs` está en el directorio de macros
2. Reinicias Tekla Structures (o abres el gestor de macros)

---

## ? SOLUCIÓN: USAR LA MACRO

### Paso a Paso:

#### 1. **Generar la Macro**
```
- Ejecuta el sincronizador
- Cuando detecte soldaduras, acepta generar la macro
- La macro se guarda automáticamente en el directorio correcto
```

Ubicación automática:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\AutoChangeWeldsToPhase2.cs
```

#### 2. **Reiniciar Tekla o Refrescar Macros**
```
OPCIÓN A: Reiniciar Tekla Structures (recomendado)
- Cierra Tekla
- Vuelve a abrir
- Las macros nuevas aparecerán automáticamente

OPCIÓN B: Refrescar el gestor de macros
- Abre: Tools > Macros...
- Cierra el diálogo
- Vuelve a abrir: Tools > Macros...
- La macro debería aparecer
```

#### 3. **Abrir el Gestor de Macros**
```
Tools > Macros...
```

#### 4. **Verificar que Aparece la Macro**
```
- La macro debería aparecer en la lista
- Nombre: AutoChangeWeldsToPhase2
- Si NO aparece, reinicia Tekla
```

#### 5. **Ejecutar la Macro**
```
1. Selecciona las soldaduras en el modelo
2. En el gestor de macros, selecciona: AutoChangeWeldsToPhase2
3. Click en "Run"
4. La macro se ejecuta automáticamente
```

---

## ?? FLUJO CORRECTO

```
???????????????????????????????????????
?  1. Ejecutar Sincronizador          ?
?     Genera: AutoChangeWeldsToPhase2.cs?
?     En: common\macros\              ?
???????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????
?  2. Reiniciar Tekla                 ?
?     (o refrescar gestor de macros)  ?
???????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????
?  3. Abrir: Tools > Macros...        ?
?     La macro aparece automáticamente?
???????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????
?  4. ? Macro Visible en la Lista    ?
?     AutoChangeWeldsToPhase2         ?
???????????????????????????????????????
                  ?
                  ?
???????????????????????????????????????
?  5. Seleccionar soldaduras + Run    ?
?     ? Phase actualizado             ?
???????????????????????????????????????
```

---

## ?? IMPORTANTE: MACROS vs COMPONENTES

### MACROS (`.cs`) - LO QUE USAMOS
- **NO se importan** con "Import..."
- **Se detectan automáticamente** del directorio
- **Ubicación**: `common\macros\`
- **Acceso**: `Tools > Macros...`
- **Requiere**: Reiniciar Tekla para detectar macros nuevas

### COMPONENTES (`.uel`) - NO LO USAMOS
- **SÍ se importan** con "Import..."
- **Ubicación**: `USA\Common\General\Macros\modeling\`
- **Acceso**: `Applications & components`
- Este sistema **NO genera componentes**

---

## ?? PROBLEMAS COMUNES

### Problema 1: "La macro no aparece en la lista"
**Solución**: 
- **REINICIA TEKLA** (esto es necesario para detectar macros nuevas)
- Verifica que el archivo esté en: `C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\`
- Verifica que el archivo termine en `.cs`

### Problema 2: "No veo el botón Import"
**Solución**:
- **NO necesitas "Import" para macros** - eso es solo para componentes (`.uel`)
- Las macros se detectan automáticamente del directorio
- Solo reinicia Tekla

### Problema 3: "La macro no se ejecuta"
**Solución**:
1. Verifica que seleccionaste soldaduras antes de ejecutar
2. La primera ejecución toma 2-3 segundos (Tekla compila la macro)
3. Verifica que el archivo `.cs` esté completo

### Problema 4: "Dice que el archivo no existe"
**Solución**:
- Regenera la macro desde el sincronizador
- Verifica permisos de escritura en el directorio
- Ejecuta el sincronizador como Administrador si es necesario

---

## ?? CONSEJOS PRO

### Tip 1: No necesitas importar
```
? Solo guarda el archivo .cs en el directorio correcto
? Reinicia Tekla
? La macro aparece automáticamente
```

### Tip 2: Macro para cada Phase
```
? Genera: AutoChangeWeldsToPhase1.cs ? Para Phase 1
? Genera: AutoChangeWeldsToPhase2.cs ? Para Phase 2
? Genera: AutoChangeWeldsToPhase3.cs ? Para Phase 3
? Reinicia Tekla ? Todas aparecen automáticamente
```

### Tip 3: Crear atajo de teclado
```
1. Tools > Customize > Keyboard Shortcuts
2. Busca tu macro: AutoChangeWeldsToPhase2
3. Asigna tecla (ej: Ctrl+Shift+2)
4. ¡Ejecución instantánea!
```

### Tip 4: Pre-generar macros comunes
```
1. Crea assemblies de prueba con Phase 1, 2, 3, 4, 5
2. Ejecuta el sincronizador en cada uno
3. Genera todas las macros que necesitas
4. Reinicia Tekla una vez
5. ? Todas las macros disponibles siempre
```

---

## ?? CHECKLIST CORRECTO

- [ ] ? Generar macro con sincronizador
- [ ] ? Verificar que el archivo .cs existe en common\macros\
- [ ] ? **REINICIAR TEKLA STRUCTURES**
- [ ] ? Abrir: Tools > Macros...
- [ ] ? Verificar que la macro aparece en la lista
- [ ] ? Seleccionar soldaduras en el modelo
- [ ] ? Seleccionar macro en la lista
- [ ] ? Click en "Run"
- [ ] ? Verificar que Phase cambió

---

## ?? RESUMEN RÁPIDO

```
1. Sincronizador genera: AutoChangeWeldsToPhase2.cs
   ? Guardado en: common\macros\
2. REINICIAR TEKLA
3. Tools > Macros... ? Macro aparece automáticamente
4. Seleccionar soldaduras ? Seleccionar macro ? Run
5. ? LISTO
```

---

## ?? VERIFICACIÓN DEL DIRECTORIO

Para verificar que la macro se guardó correctamente:

### En PowerShell:
```powershell
Get-ChildItem "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros" -Filter "AutoChange*.cs"
```

### Deberías ver:
```
AutoChangeWeldsToPhase1.cs
AutoChangeWeldsToPhase2.cs
AutoChangeWeldsToPhase3.cs
```

---

## ?? ALTERNATIVAS

### Opción A: Usar Phase Manager manual
```
1. Seleccionar soldaduras
2. Tools > Phase Manager
3. Seleccionar Phase
4. Modify Objects
```

### Opción B: Pre-generar todas las macros
```
1. Ejecuta el sincronizador varias veces con diferentes Phases
2. Genera macros para Phase 1-10
3. Reinicia Tekla una vez
4. Siempre tendrás todas las macros disponibles
```

---

## ?? DOCUMENTOS RELACIONADOS

- `README_AUTOMATIZACION_COMPLETA.md` - Guía completa del sistema
- `MACRO_WELD_PHASE.md` - Código de las macros
- `COMPILACION_AUTOMATICA_MACROS.md` - Cómo Tekla compila macros

---

**Versión**: 1.0.6 CORREGIDA  
**Fecha**: 2024  
**Estado**: ? DOCUMENTADO CORRECTAMENTE  
**Importación**: NO REQUERIDA - Detección automática después de reiniciar Tekla
