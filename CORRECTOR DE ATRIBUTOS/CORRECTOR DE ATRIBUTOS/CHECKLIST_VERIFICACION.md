# ? CHECKLIST DE VERIFICACIÓN DEL SISTEMA

## ?? ANTES DE EJECUTAR POR PRIMERA VEZ

### 1. ? Verificar Instalación de Tekla
- [ ] Tekla Structures 2021.0 instalado
- [ ] Ambiente USA configurado
- [ ] Modelo de prueba abierto en Tekla

### 2. ? Verificar Directorios
Ejecuta este comando en PowerShell para verificar que exista el directorio de macros:
```powershell
Test-Path "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros"
```
**Resultado esperado**: `True`

Si el resultado es `False`, crea el directorio:
```powershell
New-Item -ItemType Directory -Path "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros" -Force
```

### 3. ? Verificar Compilación
```cmd
cd "CORRECTOR DE ATRIBUTOS"
dotnet build "CORRECTOR DE ATRIBUTOS.csproj" --configuration Debug
```
**Resultado esperado**: `Compilación correcta`

### 4. ? Verificar Modelo de Tekla
Antes de ejecutar el sincronizador, asegúrate de que tu modelo tenga:
- [ ] Al menos un Assembly con piezas
- [ ] La pieza principal (Main Part) tiene Phase asignada
- [ ] Hay piezas secundarias, tornillos o soldaduras

## ?? PRUEBA INICIAL PASO A PASO

### Paso 1: Preparar Modelo de Tekla
1. Abre un modelo en Tekla
2. Selecciona UNA pieza principal de un assembly
3. Abre sus propiedades (doble clic)
4. Ve a la pestaña **Phase**
5. Asigna un número de fase (ejemplo: **2**)
6. Click en **Modify**
7. Cierra las propiedades

### Paso 2: Ejecutar Sincronizador
```cmd
cd "CORRECTOR DE ATRIBUTOS"
.\ejecutar.bat
```

### Paso 3: Seleccionar Assembly
1. En la ventana del sincronizador, click en **"Ejecutar Sincronización"**
2. En Tekla, selecciona el assembly que acabas de configurar
3. Presiona **ENTER** o **botón central del mouse**

### Paso 4: Verificar Resultados
El sincronizador debería mostrar algo como:
```
???????????????????????????????????????
  REPORTE DE SINCRONIZACIÓN DE PHASE
???????????????????????????????????????

Assemblies procesados: 1

???????????????????????????????????????
SECONDARY PARTS:
  • Evaluadas:  5
  • Cambiadas:  5
  • Omitidas:   0

BOLTS:
  • Evaluados:  10
  • Cambiados:  10
  • Omitidos:   0

WELDS:
  • Evaluadas:  15
  • Cambiadas:  0
  • Omitidas:   15
```

? Si ves este resultado, **Parts y Bolts funcionan correctamente**

### Paso 5: Procesar Soldaduras (Si hay)
Si el sistema encuentra soldaduras:

1. Aparecerá un mensaje: **"¿Deseas procesarlas AUTOMÁTICAMENTE?"**
2. Click en **SÍ**
3. El sistema generará la macro automáticamente
4. Mensaje de confirmación mostrará la ubicación:
   ```
   ? Macro creada: C:\ProgramData\Trimble\...\common\macros\AutoChangeWeldsToPhase2.cs
   ? Lista de soldaduras guardada: ...\Welds_Phase2_IDs.txt
   ```
   **NOTA**: Las macros usan extensión `.cs` (no `.uel` que es para componentes)

5. En Tekla:
   - Selecciona las soldaduras del assembly (usa filtros si es necesario)
   - `Tools` > `Macros...`
   - Busca: `AutoChangeWeldsToPhase2`
   - Click **Run**

6. ? Las soldaduras deberían cambiar a Phase 2

## ?? VERIFICACIÓN DE RESULTADOS

### Verificar Parts (Piezas)
1. En Tekla, selecciona una pieza secundaria del assembly
2. Abre propiedades (doble clic)
3. Ve a la pestaña **Phase**
4. ? Debería mostrar el mismo Phase que la pieza principal

### Verificar Bolts (Tornillos)
1. Selecciona un tornillo del assembly
2. Abre propiedades
3. Ve a la pestaña **Phase**
4. ? Debería mostrar el mismo Phase que la pieza principal

### Verificar Welds (Soldaduras)
1. Selecciona una soldadura del assembly
2. Abre propiedades
3. Ve a la pestaña **Phase**
4. ? Debería mostrar el mismo Phase que la pieza principal

## ?? DIAGNÓSTICO DE PROBLEMAS

### Problema 1: "No se encontró el directorio de macros"
**Solución**:
```powershell
New-Item -ItemType Directory -Path "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros" -Force
```

### Problema 2: "No se pudo guardar la macro"
**Causa posible**: Permisos insuficientes

**Solución**:
1. Ejecuta el sincronizador como Administrador:
   - Click derecho en `ejecutar.bat`
   - "Ejecutar como administrador"

### Problema 3: "La macro no aparece en Tools > Macros..."
**Soluciones**:
1. Verifica que el archivo `.cs` esté en la ubicación correcta: `common\macros\`
2. Reinicia Tekla Structures
3. Verifica que el nombre del archivo sea exactamente: `AutoChangeWeldsToPhase{N}.cs`
4. Las macros de Tekla usan extensión `.cs`, los componentes usan `.uel`

### Problema 4: "La Main Part no tiene Phase asignada"
**Solución**:
1. En Tekla, selecciona la pieza principal
2. Doble clic para abrir propiedades
3. Pestaña **Phase**
4. Asigna un número (1, 2, 3, etc.)
5. Click **Modify**

### Problema 5: "Parts cambiaron pero Welds no"
**Esto es NORMAL**: Las soldaduras necesitan la macro adicional debido a limitaciones de la API de Tekla.

**Solución**: Sigue el Paso 5 arriba para ejecutar la macro.

## ?? PRUEBAS AVANZADAS

### Prueba 1: Multiple Assemblies
1. Selecciona 3-5 assemblies
2. Ejecuta el sincronizador
3. ? Todos deberían procesarse en una sola ejecución

### Prueba 2: Diferentes Phases
1. Assembly 1 ? Phase 1
2. Assembly 2 ? Phase 2
3. Assembly 3 ? Phase 3
4. Ejecuta sincronizador seleccionando los 3
5. ? Cada assembly debería mantener su Phase correspondiente
6. ? Deberían generarse 3 macros: `AutoChangeWeldsToPhase1`, `...Phase2`, `...Phase3`

### Prueba 3: Solo Soldaduras
1. Crea un assembly simple con solo soldaduras
2. Ejecuta el sincronizador
3. ? Solo debería procesar soldaduras
4. ? Parts y Bolts deberían mostrar 0

## ? SISTEMA COMPLETAMENTE FUNCIONAL

Si todas las pruebas pasaron:
- ? Parts se sincronizan automáticamente
- ? Bolts se sincronizan automáticamente
- ? Welds se pueden sincronizar con 1 click (macro generada)
- ? El sistema funciona con múltiples assemblies simultáneamente
- ? Las macros se generan y guardan automáticamente en la ubicación correcta

## ?? SIGUIENTE PASO

Ahora puedes usar el sistema en producción:

```cmd
cd "CORRECTOR DE ATRIBUTOS"
.\ejecutar.bat
```

**¡Listo para sincronizar miles de objetos con un solo click!** ??

---

## ?? NOTAS IMPORTANTES

1. **Backup**: Siempre haz backup del modelo antes de sincronizaciones masivas
2. **Pruebas**: Prueba primero con 1-2 assemblies antes de procesar todo el modelo
3. **Verificación**: Verifica siempre el reporte generado para confirmar los cambios
4. **Macros**: Las macros generadas son reutilizables - no necesitas regenerarlas cada vez

## ?? SOPORTE

Si algo no funciona después de seguir esta guía:
1. Revisa el reporte detallado del sincronizador
2. Ejecuta los diagnósticos:
   - `diagnosticar_phase.bat` - Para piezas
   - `diagnosticar_weld.bat` - Para soldaduras
3. Verifica que tu versión de Tekla sea 2021.0
4. Asegúrate de tener permisos de administrador si es necesario
