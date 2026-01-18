# Correcciones al Sincronizador de Atributos

## ?? Problema Identificado

El sincronizador no estaba procesando las Secondary Parts porque:

1. **Salía prematuramente** cuando los atributos estaban vacíos
2. **No intentaba suficientes variantes** de nombres de atributos
3. **No había suficiente logging** para debug

## ? Correcciones Aplicadas

### 1. Lógica de Sincronización Mejorada

**ANTES** (salía si ambos vacíos sin procesar):
```csharp
if (string.IsNullOrEmpty(estatusPieza) && string.IsNullOrEmpty(prioridad))
{
    _report.AddWarning("Main Part sin ESTATUS_PIEZA ni PRIORIDAD");
    return; // ? SALÍA SIN PROCESAR
}
```

**AHORA** (continúa procesando):
```csharp
// Logging detallado para debug
_report.AddInfo($"Assembly {assembly.Identifier.ID} - Main Part {mainPart.Identifier.ID}:");
_report.AddInfo($"  ESTATUS_PIEZA leído: '{estatusPieza}'");
_report.AddInfo($"  PRIORIDAD leída: '{prioridad}'");

// Solo advertir, pero CONTINUAR procesando
if (string.IsNullOrEmpty(estatusPieza) && string.IsNullOrEmpty(prioridad))
{
    _report.AddWarning("Main Part sin atributos - Se omitirá");
    return; // Solo si AMBOS están vacíos
}

// Procesar Secondary Parts y Bolts
SyncSecondaryParts(assembly, estatusPieza, prioridad);
SyncBolts(mainPart, estatusPieza, prioridad);
```

### 2. Métodos de Lectura Mejorados

**ANTES** (3 métodos):
```csharp
private string ReadEstatusPieza(ModelObject obj)
{
    string value = "";
    
    if (obj.GetUserProperty("ESTATUS_PIEZA", ref value))
        return value;
    
    if (obj.GetReportProperty("ESTATUS_PIEZA", ref value))
        return value;
    
    return "";
}
```

**AHORA** (5 métodos con try-catch y trim):
```csharp
private string ReadEstatusPieza(ModelObject obj)
{
    string value = "";
    
    // Método 1: Nombre exacto
    try {
        if (obj.GetUserProperty("ESTATUS_PIEZA", ref value) && !string.IsNullOrEmpty(value))
            return value.Trim();
    } catch { }
    
    // Método 2: GetReportProperty
    try {
        value = "";
        if (obj.GetReportProperty("ESTATUS_PIEZA", ref value) && !string.IsNullOrEmpty(value))
            return value.Trim();
    } catch { }
    
    // Método 3: Con formato de label
    try {
        value = "";
        if (obj.GetUserProperty("Estatus de Pieza:", ref value) && !string.IsNullOrEmpty(value))
            return value.Trim();
    } catch { }
    
    // Método 4: Sin dos puntos
    try {
        value = "";
        if (obj.GetUserProperty("Estatus de Pieza", ref value) && !string.IsNullOrEmpty(value))
            return value.Trim();
    } catch { }
    
    // Método 5: Todo mayúsculas
    try {
        value = "";
        if (obj.GetUserProperty("ESTATUS DE PIEZA", ref value) && !string.IsNullOrEmpty(value))
            return value.Trim();
    } catch { }
    
    return "";
}
```

### 3. Métodos de Escritura Mejorados

**ANTES** (sin validación de éxito):
```csharp
private void WriteEstatusPieza(ModelObject obj, string value)
{
    try {
        obj.SetUserProperty("ESTATUS_PIEZA", value);
    } catch { }
    
    try {
        obj.SetUserProperty("Estatus de Pieza:", value);
    } catch { }
}
```

**AHORA** (con validación y advertencias):
```csharp
private void WriteEstatusPieza(ModelObject obj, string value)
{
    if (string.IsNullOrEmpty(value)) return;
    
    bool success = false;
    
    // Método 1: Nombre exacto
    try {
        obj.SetUserProperty("ESTATUS_PIEZA", value);
        success = true;
    } catch { }
    
    // Método 2: Con formato de label
    try {
        obj.SetUserProperty("Estatus de Pieza:", value);
        if (!success) success = true;
    } catch { }
    
    // Método 3: Sin dos puntos
    try {
        obj.SetUserProperty("Estatus de Pieza", value);
        if (!success) success = true;
    } catch { }
    
    // Advertir si ningún método funcionó
    if (!success) {
        _report.AddWarning($"No se pudo escribir ESTATUS_PIEZA en objeto {obj.Identifier.ID}");
    }
}
```

### 4. Logging Detallado

Ahora el reporte incluye:

```
Assembly 5984533 - Main Part 5984534:
  ESTATUS_PIEZA leído: '' (vacío=True)
  PRIORIDAD leída: '' (vacío=True)

[WARN] Assembly 5984533: Main Part sin ESTATUS_PIEZA ni PRIORIDAD - Se omitirá
```

Esto te ayuda a ver exactamente:
- Qué Assembly está procesando
- Qué Main Part está leyendo
- Qué valores encontró (o no encontró)
- Por qué decidió omitir o procesar

## ?? Cómo Usar para Debugging

### Paso 1: Verifica que el atributo existe en Tekla

1. Abre Tekla
2. Selecciona la Main Part
3. Doble clic para abrir propiedades
4. Busca el atributo (puede estar en diferentes pestañas)
5. Anota el **nombre exacto** que aparece

### Paso 2: Ejecuta el sincronizador

```cmd
cd SINCRONIZADOR_ATRIBUTOS
ejecutar.bat
```

### Paso 3: Revisa el reporte detallado

El reporte ahora muestra:
- Qué valor leyó (o `''` si está vacío)
- Si el atributo está vacío (`True/False`)
- Qué piezas intentó modificar
- Qué métodos de lectura/escritura fallaron

### Paso 4: Si el atributo no se lee

Posibles causas:

1. **El nombre del atributo es diferente**
   - Solución: Añadir el nombre exacto a los métodos de lectura

2. **El atributo es de solo lectura**
   - Solución: Verificar que se puede modificar manualmente en Tekla

3. **El atributo no es User Property**
   - Solución: Puede ser un atributo del sistema (no sincronizable)

## ?? Nombres de Atributos Soportados

### ESTATUS_PIEZA

El sincronizador ahora busca en este orden:
1. `ESTATUS_PIEZA` (exacto)
2. `ESTATUS_PIEZA` (ReportProperty)
3. `Estatus de Pieza:` (con dos puntos)
4. `Estatus de Pieza` (sin dos puntos)
5. `ESTATUS DE PIEZA` (todo mayúsculas)

### PRIORIDAD

El sincronizador ahora busca en este orden:
1. `PRIORIDAD` (exacto)
2. `PRIORIDAD` (ReportProperty)
3. `PRIORIDAD DETALLADO:` (con dos puntos)
4. `PRIORIDAD DETALLADO` (sin dos puntos)
5. `prioridad` (minúsculas)

## ?? Próximos Pasos

1. **Recompilado**: ? Ya hecho
2. **Prueba**: Ejecuta en Tekla con el logging detallado
3. **Revisa el reporte**: Analiza qué valores lee
4. **Ajusta si es necesario**: Si el nombre del atributo es diferente, puedo añadirlo

## ?? Tip Importante

Si en Tekla el atributo aparece como:

```
attribute("ESTATUS_PIEZA", "Estatus de Pieza:", option, "%s", No, none, "0.0", "0.0")
```

Entonces:
- **Nombre interno**: `ESTATUS_PIEZA`
- **Nombre visible**: `Estatus de Pieza:`

El sincronizador ahora prueba **ambos nombres** automáticamente.

---

**Estado**: ? Compilado y listo para probar  
**Ejecutable**: `SINCRONIZADOR_ATRIBUTOS\bin\Release\net48\SINCRONIZADOR_ATRIBUTOS.exe`  
**Fecha**: 2024-01-18
