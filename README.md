# Corrector de Atributos Tekla

Sistema automatizado para sincronizacion de atributos Phase en Tekla Structures, disenado para mantener la coherencia de fases entre Main Parts, Secondary Parts, Bolts y Welds dentro de los Assemblies.

## Tabla de Contenidos

- [Descripcion General](#descripcion-general)
- [Caracteristicas](#caracteristicas)
- [Requisitos del Sistema](#requisitos-del-sistema)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Instalacion](#instalacion)
- [Uso](#uso)
- [Arquitectura y Componentes](#arquitectura-y-componentes)
- [Documentacion Adicional](#documentacion-adicional)
- [Solucion de Problemas](#solucion-de-problemas)
- [Contribucion](#contribucion)

---

## Descripcion General

**Corrector de Atributos Tekla** es una solucion completa para sincronizar automaticamente el atributo `Phase` (fase) entre todos los componentes de un Assembly en Tekla Structures.

### Componentes Principales

1. **Aplicacion de Sincronizacion** (.NET): Sincroniza Parts y Bolts
2. **Macro de Tekla**: Sincroniza Welds (soldaduras)

### Que problema resuelve?

En proyectos de Tekla Structures, mantener la coherencia del atributo `Phase` entre todos los elementos de un Assembly puede ser tedioso y propenso a errores. Este sistema automatiza completamente el proceso:

- Detecta automaticamente la Phase de la Main Part
- Propaga el valor a todas las Secondary Parts
- Actualiza todos los Bolts del Assembly
- Sincroniza las Welds basandose en las piezas conectadas
- Genera reportes detallados de todos los cambios

---

## Caracteristicas

### Sincronizador de Assemblies

- **Sincronizacion automatica** de Parts y Bolts basada en Main Part
- **Reportes detallados** de cambios realizados
- **Seleccion flexible**: selecciona Assemblies, Parts o Bolts individuales
- **Validacion robusta** con multiples metodos de lectura/escritura
- **Alto rendimiento**: procesamiento en lote con commit unico
- **Deteccion de errores** con mensajes claros

### Macro de Soldaduras

- **Sincronizacion inteligente** basada en piezas conectadas
- **Alcance configurable**: seleccionadas o todas las soldaduras
- **Reporte completo** de soldaduras procesadas
- **Una sola macro** para todas las fases
- **Ejecucion rapida** directa desde Tekla

---

## Requisitos del Sistema

### Software Requerido

- **Tekla Structures 2021.0** o superior
- **.NET Framework 4.8**
- **Windows** (7, 8, 10, 11)
- **Visual Studio 2019/2022** (solo para desarrollo)

### Dependencias NuGet

```xml
<PackageReference Include="System.Resources.Extensions" Version="8.0.0" />
<PackageReference Include="Tekla.Structures" Version="2021.0.0" />
<PackageReference Include="Tekla.Structures.Datatype" Version="2021.0.0" />
<PackageReference Include="Tekla.Structures.Dialog" Version="2021.0.0" />
<PackageReference Include="Tekla.Structures.Model" Version="2021.0.0" />
<PackageReference Include="Tekla.Structures.Plugins" Version="2021.0.0" />
```

---

## Estructura del Proyecto

```
008-CORRECTOR-DE-ATRIBUTOS-TEKLA/
|
+-- README.md                                    (Este archivo)
+-- instructions.md                              (Instrucciones de codificacion)
|
+-- CORRECTOR DE ATRIBUTOS/                      (Proyecto principal)
    +-- CORRECTOR DE ATRIBUTOS.sln              (Solucion de Visual Studio)
    +-- CORRECTOR DE ATRIBUTOS.csproj           (Archivo de proyecto)
    |
    +-- CORRECTOR DE ATRIBUTOS/                 (Codigo fuente)
    |   +-- PhaseSynchronizer.cs                (Motor de sincronizacion - CORE)
    |   +-- PhaseSyncForm.cs                    (Interfaz grafica)
    |   +-- PhaseSyncLauncher.cs                (Punto de entrada)
    |   +-- SyncReport.cs                       (Sistema de reportes)
    |   +-- PhasePropertyDiagnostic.cs          (Herramienta de diagnostico)
    |   |
    |   +-- ejecutar.bat                        (Script de ejecucion rapida)
    |   +-- instalar_macro.bat                  (Instalador de macro)
    |   +-- verificar_macro.bat                 (Verificar instalacion)
    |   |
    |   +-- Documentacion/                      (40+ documentos tecnicos)
    |       +-- RESUMEN_FINAL_COMPLETO.md
    |       +-- README_AUTOMATIZACION_COMPLETA.md
    |       +-- GUIA_EJECUTAR.md
    |       +-- ... (mas documentos)
    |
    +-- Installer/                              (Archivos de instalacion)
    |   +-- BuildDrop/net48/
    |       +-- CORRECTOR_DE_ATRIBUTOS.exe      (Ejecutable compilado)
    |
    +-- MainForm.cs                             (Form del plugin - legacy)
    +-- MainForm.Designer.cs
    +-- ModelPlugin.cs                          (Plugin base - legacy)
    |
    +-- Properties/                             (Propiedades del proyecto)
        +-- AssemblyInfo.cs
|
+-- MacroPlantilla/                             (Macros de Tekla)
    +-- SyncWeldPhaseFromParts_OLD.cs           (Plantilla de macro - backup)
```

### Dependencias entre Archivos

La arquitectura del sistema sigue un patron modular con separacion clara de responsabilidades:

#### Flujo Principal de Ejecucion

```
PhaseSyncLauncher.cs (Main Entry Point)
    |
    +-- Crea --> PhaseSyncForm.cs (Interfaz Grafica)
            |
            +-- Crea --> PhaseSynchronizer.cs (Motor Core)
                    |
                    +-- Genera --> SyncReport.cs (Reportes)
```

#### Modulos y sus Responsabilidades

**1. PhaseSyncLauncher.cs**
- Punto de entrada de la aplicacion (Main())
- Inicializa el formulario principal
- No depende de otros modulos del proyecto

**2. PhaseSyncForm.cs**
- Interfaz grafica de usuario (Windows Forms)
- Maneja eventos de botones y controles
- Depende de:
  - PhaseSynchronizer.cs (para ejecutar sincronizacion)
  - SyncReport.cs (para mostrar resultados)

**3. PhaseSynchronizer.cs** **[NUCLEO DEL SISTEMA]**
- Logica principal de sincronizacion
- Interactua directamente con la API de Tekla
- Procesa Assemblies, Parts y Bolts
- Depende de:
  - Tekla.Structures.Model (API de Tekla)
  - SyncReport.cs (para registrar eventos)
- Utilizado por:
  - PhaseSyncForm.cs

**4. SyncReport.cs**
- Sistema de logging y reportes
- Acumula estadisticas de sincronizacion
- Genera reportes formateados
- No depende de otros modulos del proyecto
- Utilizado por:
  - PhaseSynchronizer.cs
  - PhaseSyncForm.cs

**5. PhasePropertyDiagnostic.cs**
- Herramienta de diagnostico
- Prueba diferentes metodos de lectura/escritura de Phase
- Independiente del flujo principal
- Usado para troubleshooting

#### Archivos Legacy (Plugin de Tekla)

Estos archivos son parte de la plantilla original del plugin de Tekla, pero **NO se usan** en la version actual:

- **ModelPlugin.cs**: Plugin base generado por Tekla (plantilla)
- **MainForm.cs**: Formulario del plugin (plantilla)
- **MainForm.Designer.cs**: Disenador del formulario

**Nota**: El proyecto actual funciona como aplicacion standalone, no como plugin de Tekla.

#### Macros de Tekla (Separadas)

- **MacroPlantilla/SyncWeldPhaseFromParts_OLD.cs**
  - Plantilla de macro para sincronizar soldaduras
  - Se instala en: `C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\`
  - **NO se compila con el proyecto C#** (se compila automaticamente por Tekla)
  - Funciona de manera completamente independiente

---

## Instalacion

### Paso 1: Clonar o Descargar el Repositorio

```bash
git clone https://github.com/GrupoAdipsa/008-CORRECTOR-DE-ATRIBUTOS-TEKLA.git
cd 008-CORRECTOR-DE-ATRIBUTOS-TEKLA
```

### Paso 2: Compilar la Aplicacion (Opcional)

Si necesitas recompilar el proyecto:

```bash
cd "CORRECTOR DE ATRIBUTOS"
dotnet build "CORRECTOR DE ATRIBUTOS.sln" --configuration Release
```

**Nota**: El ejecutable precompilado ya esta disponible en `Installer/BuildDrop/net48/CORRECTOR_DE_ATRIBUTOS.exe`

### Paso 3: Instalar la Macro de Soldaduras

```batch
cd "CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
instalar_macro.bat
```

Este script copia la macro a la ubicacion correcta de Tekla:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\
```

### Paso 4: Verificar Instalacion

```batch
verificar_macro.bat
```

**Checklist de Verificacion**:
- El ejecutable `CORRECTOR_DE_ATRIBUTOS.exe` existe
- La macro aparece en Tekla: `Tools > Macros... > SyncWeldPhaseFromParts`

---

## Uso

### 1. Sincronizar Assemblies (Parts y Bolts)

#### Opcion A: Usando el Script

```batch
cd "CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
ejecutar.bat
```

#### Opcion B: Ejecutando Directamente

```batch
cd "CORRECTOR DE ATRIBUTOS\Installer\BuildDrop\net48"
CORRECTOR_DE_ATRIBUTOS.exe
```

#### Proceso:

1. **Se abre una ventana de seleccion en Tekla**
2. **Selecciona** los Assemblies, Parts o Bolts que deseas sincronizar
3. **Presiona ENTER** o haz clic derecho y Confirmar
4. **Se procesa** la sincronizacion automaticamente
5. **Se muestra** un reporte con los resultados

#### Ejemplo de Reporte:

```
=== REPORTE DE SINCRONIZACION ===
Assemblies procesados: 15
Parts evaluadas: 120
  - Modificadas: 118
  - Sin cambios: 2
Bolts evaluados: 87
  - Modificados: 85
  - Sin cambios: 2

[OK] Sincronizacion completada exitosamente
```

---

### 2. Sincronizar Soldaduras (Welds)

#### Primera Ejecucion:

1. **Reinicia Tekla Structures** (necesario para detectar la macro)
2. Abre tu modelo en Tekla
3. Ve a `Tools > Macros...`
4. Busca `SyncWeldPhaseFromParts`
5. Haz clic en `Run`

#### Configuracion del Alcance:

Se te preguntara:

```
Procesar solo las soldaduras seleccionadas?
[S] = Si (solo seleccionadas)
[N] = No (todas las soldaduras del modelo)
```

- **Opcion S**: Ideal para correcciones puntuales
- **Opcion N**: Ideal para sincronizacion completa inicial

#### Ejemplo de Salida:

```
=== Iniciando Sincronizacion de Soldaduras ===
Procesando: Todas las soldaduras del modelo

Soldaduras procesadas: 245
  - Actualizadas: 238
  - Ya correctas: 7
  - Con errores: 0

[OK] Sincronizacion completada
```

---

### Flujo de Trabajo Recomendado

#### Para Proyectos Nuevos:

```
1. Asignar Phase a Main Parts en Tekla
   |
   v
2. Ejecutar Sincronizador
   |
   v
3. Reiniciar Tekla
   |
   v
4. Ejecutar Macro: Todas las soldaduras
   |
   v
5. Proyecto completo sincronizado
```

#### Para Mantenimiento Continuo:

```
1. Agregar nuevos Assemblies
   |
   v
2. Ejecutar Sincronizador
   |
   v
3. Ejecutar Macro: Solo seleccionadas
   |
   v
4. Verificar cambios
```

#### Para Correcciones Puntuales:

```
1. Identificar elementos con Phase incorrecta
   |
   v
2. Seleccionar elementos
   |
   v
3. Ejecutar herramienta correspondiente
   |
   v
4. Verificar reporte
```

---

## Arquitectura y Componentes

### Componente 1: Sincronizador de Assemblies (.NET)

#### Responsabilidades:
- Sincronizar **Secondary Parts** basandose en la **Main Part**
- Sincronizar **Bolts** del Assembly
- Detectar **Welds** que necesitan actualizacion
- Generar reportes detallados

#### Tecnologias:
- **Lenguaje**: C# (.NET Framework 4.8)
- **Interfaz**: Windows Forms
- **API**: Tekla Structures API 2021.0

#### Archivos Clave:

##### PhaseSynchronizer.cs (Nucleo del Sistema)

```csharp
// Estructura basica del sincronizador

// 1. Constructor - Inicializa modelo y reporte
public PhaseSynchronizer()
{
    _model = new Model();              // Conexion con Tekla
    _report = new SyncReport();        // Sistema de logging
}

// 2. Metodo principal - Ejecucion interactiva
public bool ExecuteInteractive()
{
    // Paso 1: Obtener seleccion del usuario en Tekla
    Picker picker = new Picker();
    ModelObjectEnumerator selectedObjects = picker.PickObjects(
        Picker.PickObjectsEnum.PICK_N_OBJECTS,
        "Selecciona piezas o assemblies para sincronizar Phase"
    );
    
    // Paso 2: Procesar seleccion
    return ExecuteOnSelection(selectedObjects);
}

// 3. Agrupar objetos por Assembly
private Dictionary<Assembly, List<ModelObject>> GroupByAssembly(
    ModelObjectEnumerator selectedObjects)
{
    // Agrupa todos los objetos seleccionados por su Assembly padre
    // Esto permite procesar todo el Assembly aunque solo se seleccione una Part
}

// 4. Sincronizar un Assembly completo
private void SynchronizeAssembly(Assembly assembly)
{
    // Paso 1: Obtener Main Part del Assembly
    Part mainPart = assembly.GetMainPart() as Part;
    
    // Paso 2: Leer Phase de la Main Part (multiples metodos)
    int phaseNumber = ReadPhaseFromPart(mainPart);
    
    // Paso 3: Sincronizar Secondary Parts
    SyncSecondaryParts(assembly, phaseNumber);
    
    // Paso 4: Sincronizar Bolts
    SyncBolts(mainPart, phaseNumber);
}

// 5. Sincronizar Parts secundarias
private void SyncSecondaryParts(Assembly assembly, int targetPhase)
{
    ArrayList secondaries = assembly.GetSecondaries();
    
    foreach (Part part in secondaries)
    {
        // Aplicar Phase usando multiples metodos de fallback
        WritePhaseToObject(part, targetPhase);
        part.Modify();
        
        _report.PartsChanged++;
    }
}

// 6. Sincronizar Bolts
private void SyncBolts(Part mainPart, int targetPhase)
{
    ModelObjectEnumerator bolts = mainPart.GetBolts();
    
    while (bolts.MoveNext())
    {
        BoltGroup bolt = bolts.Current as BoltGroup;
        
        // Aplicar Phase
        WritePhaseToObject(bolt, targetPhase);
        bolt.Modify();
        
        _report.BoltsChanged++;
    }
}
```

**Explicacion del Flujo:**

1. **Inicializacion**: Se crea una conexion con el modelo de Tekla activo
2. **Seleccion**: El usuario selecciona elementos en Tekla (Assemblies, Parts o Bolts)
3. **Agrupacion**: Todos los elementos se agrupan por su Assembly padre
4. **Lectura**: Se lee el valor de Phase de la Main Part de cada Assembly
5. **Escritura**: Se propaga ese valor a todas las Secondary Parts y Bolts
6. **Commit**: Se guardan todos los cambios en el modelo de Tekla con un solo commit
7. **Reporte**: Se genera un reporte detallado de todos los cambios

##### SyncReport.cs (Sistema de Reportes)

```csharp
// Estructura del sistema de reportes

public class SyncReport
{
    // Contadores de estadisticas
    public int PartsEvaluated { get; set; }
    public int PartsChanged { get; set; }
    public int PartsSkipped { get; set; }
    
    public int BoltsEvaluated { get; set; }
    public int BoltsChanged { get; set; }
    public int BoltsSkipped { get; set; }
    
    // Listas de eventos
    private List<string> _errors = new List<string>();
    private List<string> _warnings = new List<string>();
    private List<int> _assembliesProcessed = new List<int>();
    
    // Metodos para registrar eventos
    public void AddError(string message)
    {
        _errors.Add($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}");
    }
    
    public void AddWarning(string message)
    {
        _warnings.Add($"[WARNING] {DateTime.Now:HH:mm:ss} - {message}");
    }
    
    // Generar reporte formateado
    public string GenerateReport()
    {
        StringBuilder sb = new StringBuilder();
        
        sb.AppendLine("=== REPORTE DE SINCRONIZACION ===");
        sb.AppendLine($"Assemblies procesados: {_assembliesProcessed.Count}");
        sb.AppendLine($"Parts evaluadas: {PartsEvaluated}");
        sb.AppendLine($"  - Modificadas: {PartsChanged}");
        sb.AppendLine($"  - Sin cambios: {PartsSkipped}");
        // ... mas estadisticas
        
        return sb.ToString();
    }
}
```

**Proposito del Reporte:**
- Proporciona visibilidad completa de la operacion
- Ayuda a identificar problemas (errores y advertencias)
- Confirma que los cambios se aplicaron correctamente
- Util para auditoria y debugging

---

### Componente 2: Macro de Soldaduras (Tekla)

#### Responsabilidades:
- Sincronizar **Welds** basandose en piezas conectadas
- Operar directamente desde el entorno de Tekla
- Manejar seleccion flexible (todas o seleccionadas)

#### Tecnologias:
- **Lenguaje**: C# (compilado por Tekla)
- **Entorno**: Tekla Macro Engine
- **API**: Tekla Structures API

#### Archivo Clave:

##### SyncWeldPhaseFromParts.cs (Macro de Tekla)

```csharp
// Estructura basica de la macro (version simplificada)

public class SyncWeldPhaseFromParts
{
    public static void Run(Tekla.Technology.Akit.IScript akit)
    {
        Model model = new Model();
        
        // Paso 1: Preguntar alcance al usuario
        string input = akit.ValueQuery("Procesar solo soldaduras seleccionadas? (S/N)", "N");
        bool onlySelected = (input.ToUpper() == "S");
        
        // Paso 2: Obtener soldaduras
        ModelObjectEnumerator welds = GetWelds(model, onlySelected);
        
        int processed = 0;
        int updated = 0;
        
        // Paso 3: Procesar cada soldadura
        while (welds.MoveNext())
        {
            BaseWeld weld = welds.Current as BaseWeld;
            if (weld == null) continue;
            
            processed++;
            
            // Paso 4: Obtener Phase de las piezas conectadas
            int targetPhase = GetPhaseFromConnectedParts(weld);
            
            // Paso 5: Aplicar Phase a la soldadura
            if (ApplyPhaseToWeld(weld, targetPhase))
            {
                updated++;
            }
        }
        
        // Paso 6: Guardar cambios
        model.CommitChanges();
        
        // Paso 7: Mostrar reporte
        akit.Output($"Procesadas: {processed}, Actualizadas: {updated}");
    }
    
    // Obtener Phase de las piezas conectadas a una soldadura
    private static int GetPhaseFromConnectedParts(BaseWeld weld)
    {
        // Intentar obtener Phase de la pieza principal
        ModelObject mainObj = weld.MainObject;
        int phase = ReadPhaseFromObject(mainObj);
        
        if (phase == 0)
        {
            // Si no tiene, intentar con la pieza secundaria
            ModelObject secondaryObj = weld.SecondaryObject;
            phase = ReadPhaseFromObject(secondaryObj);
        }
        
        return phase;
    }
    
    // Leer Phase de un objeto (multiples metodos de fallback)
    private static int ReadPhaseFromObject(ModelObject obj)
    {
        int phase = 0;
        
        // Metodo 1: GetReportProperty
        if (obj.GetReportProperty("PHASE", ref phase) && phase > 0)
            return phase;
        
        // Metodo 2: GetUserProperty
        if (obj.GetUserProperty("PHASE_NUMBER", ref phase) && phase > 0)
            return phase;
        
        // Metodo 3: GetPhase() si existe
        try
        {
            Phase phaseObj;
            if (obj.GetPhase(out phaseObj) && phaseObj != null)
                return phaseObj.PhaseNumber;
        }
        catch { }
        
        return 0;
    }
}
```

**Explicacion del Flujo:**

1. **Entrada del Usuario**: Se pregunta si procesar todas o solo las seleccionadas
2. **Obtencion**: Se obtienen las soldaduras segun el alcance elegido
3. **Iteracion**: Se procesa cada soldadura individualmente
4. **Lectura**: Se lee el Phase de la pieza principal conectada
5. **Fallback**: Si no tiene, se lee de la pieza secundaria
6. **Escritura**: Se aplica el Phase a la soldadura
7. **Commit**: Se guardan todos los cambios
8. **Feedback**: Se muestra un reporte al usuario

---

### Interaccion entre Componentes

```
+--------------------------------------------------------+
|                   USUARIO EN TEKLA                     |
+--------------+---------------------+-------------------+
               |                     |
               v                     v
+---------------------------+ +---------------------------+
| SINCRONIZADOR (.NET)      | | MACRO (Tekla)             |
|                           | |                           |
| - PhaseSyncLauncher.cs    | | - SyncWeldPhaseFromParts  |
| - PhaseSyncForm.cs        | |                           |
| - PhaseSynchronizer.cs    | | Sincroniza:               |
| - SyncReport.cs           | | +-- Welds                 |
|                           | |                           |
| Sincroniza:               | | Ubicacion:                |
| +-- Parts                 | | +-- Macros de Tekla       |
| +-- Bolts                 | |                           |
+--------------+------------+ +-------------+-------------+
               |                            |
               +------------+---------------+
                            v
                 +----------------------+
                 |  TEKLA MODEL API     |
                 |                      |
                 | - Model              |
                 | - Assembly           |
                 | - Part               |
                 | - BoltGroup          |
                 | - BaseWeld           |
                 +----------------------+
```

**Nota Importante**: Ambos componentes son **independientes** pero **complementarios**:
- El sincronizador maneja Parts y Bolts
- La macro maneja Welds
- Ambos pueden ejecutarse por separado
- Juntos proporcionan sincronizacion completa

---

## Documentacion Adicional

El proyecto incluye mas de 40 documentos tecnicos en la carpeta `CORRECTOR DE ATRIBUTOS/CORRECTOR DE ATRIBUTOS/`:

### Guias de Usuario:

| Documento | Descripcion |
|-----------|-------------|
| `RESUMEN_FINAL_COMPLETO.md` | Resumen ejecutivo completo del sistema |
| `README_AUTOMATIZACION_COMPLETA.md` | Guia completa de automatizacion |
| `GUIA_EJECUTAR.md` | Como usar el sincronizador |
| `MACRO_INDEPENDIENTE_WELDS.md` | Como usar la macro de soldaduras |
| `SISTEMA_LISTO_USAR.md` | Checklist de verificacion |

### Documentacion Tecnica:

| Documento | Descripcion |
|-----------|-------------|
| `FORMATO_MACROS_UL.md` | Formatos de archivos de Tekla |
| `COMO_IMPORTAR_MACROS_TEKLA.md` | Sistema de macros de Tekla |
| `COMPILACION_AUTOMATICA_MACROS.md` | Como Tekla compila macros |
| `ACLARACION_CRITICA_MACROS_COMPONENTES.md` | Diferencias entre componentes |
| `DOS_APLICACIONES_SEPARADAS.md` | Arquitectura del sistema |

### Solucion de Problemas:

| Documento | Descripcion |
|-----------|-------------|
| `SOLUCION_PHASE_FALTANTE.md` | Si Phase no esta asignada |
| `SOLUCION_PARTS_NO_CAMBIAN.md` | Si Parts no se actualizan |
| `SOLUCION_SOLDADURAS_HEREDAN_PHASE.md` | Problemas con soldaduras |
| `CHECKLIST_VERIFICACION.md` | Lista de verificacion completa |
| `PRUEBA_EN_VIVO.md` | Checklist de prueba |

---

## Solucion de Problemas

### Problema 1: "No hay conexion con Tekla Structures"

**Causa**: Tekla no esta abierto o el modelo no esta cargado.

**Solucion**:
1. Abre Tekla Structures
2. Abre o crea un modelo
3. Ejecuta nuevamente la aplicacion

---

### Problema 2: "La Main Part no tiene Phase asignada"

**Causa**: La Main Part del Assembly no tiene un valor de Phase.

**Solucion**:
1. Selecciona la Main Part en Tekla
2. Abre las propiedades (doble clic)
3. Busca el campo `Phase` o `PHASE_NUMBER`
4. Asigna un valor numerico (ej: 1, 2, 3)
5. Guarda los cambios
6. Ejecuta nuevamente el sincronizador

---

### Problema 3: "Macro no aparece en Tekla"

**Causa**: Tekla no ha detectado la macro nueva.

**Solucion**:
1. Cierra completamente Tekla Structures
2. Verifica que la macro existe en:
   ```
   C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\SyncWeldPhaseFromParts.cs
   ```
3. Abre Tekla nuevamente
4. Ve a `Tools > Macros...`
5. La macro deberia aparecer ahora

---

### Problema 4: "Parts Skipped: X" en el reporte

**Causa**: Algunas Parts no pudieron modificarse (puede ser por permisos o estado).

**Solucion**:
1. Verifica que las Parts no esten bloqueadas en Tekla
2. Asegurate de que no haya filtros activos
3. Revisa el reporte completo para ver mensajes especificos
4. Intenta ejecutar el sincronizador nuevamente

---

### Problema 5: Errores de Compilacion

**Causa**: Dependencias faltantes o configuracion incorrecta.

**Solucion**:
```bash
# 1. Restaurar paquetes NuGet
cd "CORRECTOR DE ATRIBUTOS"
dotnet restore

# 2. Limpiar solucion
dotnet clean

# 3. Recompilar
dotnet build --configuration Release
```

---

### Problema 6: "System.DllNotFoundException: Tekla.Structures"

**Causa**: Variable de entorno `XSDATADIR` no configurada.

**Solucion**:
1. Abre "Variables de entorno" en Windows
2. Verifica que existe `XSDATADIR`
3. Deberia apuntar a algo como:
   ```
   C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\system\
   ```
4. Si no existe, creala con la ruta correcta de tu instalacion de Tekla

---

## Contribucion

### Estructura de Commits

Usa commits descriptivos siguiendo este formato:

```
<tipo>: <descripcion breve>

<descripcion detallada (opcional)>
```

**Tipos**:
- `feat`: Nueva caracteristica
- `fix`: Correccion de bug
- `docs`: Cambios en documentacion
- `refactor`: Refactorizacion de codigo
- `test`: Anadir o modificar tests
- `chore`: Tareas de mantenimiento

**Ejemplo**:
```
feat: Anadir soporte para Tekla 2022

- Actualizar referencias NuGet a version 2022.0.0
- Ajustar metodos de lectura/escritura de Phase
- Actualizar documentacion
```

---

### Guia de Estilo de Codigo

Este proyecto sigue las instrucciones definidas en `instructions.md`:

#### Comentarios en Espanol
```csharp
// [OK] CORRECTO
// Obtiene el Assembly padre de una Part
Assembly assembly = part.GetAssembly();

// [X] INCORRECTO
// Get parent assembly
Assembly assembly = part.GetAssembly();
```

#### Comentarios Detallados
```csharp
// [OK] CORRECTO
// Sincroniza el Phase de todas las Secondary Parts de un Assembly.
// 
// Que hace?
// - Obtiene todas las piezas secundarias del Assembly
// - Lee el Phase objetivo de la Main Part
// - Aplica ese Phase a cada pieza secundaria
// - Registra el resultado en el reporte
//
// Por que es necesario?
// Las Secondary Parts no heredan automaticamente el Phase de la Main Part,
// por lo que debemos sincronizarlas manualmente para mantener coherencia.
private void SyncSecondaryParts(Assembly assembly, int targetPhase)
{
    // Obtener lista de piezas secundarias
    ArrayList secondaries = assembly.GetSecondaries();
    
    // Procesar cada pieza
    foreach (Part part in secondaries)
    {
        // Aplicar el Phase y modificar
        WritePhaseToObject(part, targetPhase);
        part.Modify();
        
        // Registrar en el reporte
        _report.PartsChanged++;
    }
}

// [X] INCORRECTO
// Sync parts
private void SyncSecondaryParts(Assembly assembly, int targetPhase)
{
    ArrayList secondaries = assembly.GetSecondaries();
    foreach (Part part in secondaries)
    {
        WritePhaseToObject(part, targetPhase);
        part.Modify();
        _report.PartsChanged++;
    }
}
```

#### Documentacion de Decisiones
```csharp
// [OK] CORRECTO
// DECISION DE DISENO: Usar multiples metodos de lectura
//
// Por que?
// Diferentes versiones de Tekla usan diferentes propiedades para Phase:
// - Tekla 2019: "PHASE_NUMBER" (user property)
// - Tekla 2020: "PHASE" (report property)
// - Tekla 2021+: GetPhase() method
//
// Para garantizar compatibilidad, intentamos todos los metodos en secuencia
// hasta encontrar uno que funcione.
private int ReadPhaseFromObject(ModelObject obj)
{
    int phase = 0;
    
    // Metodo 1: ReportProperty
    if (obj.GetReportProperty("PHASE", ref phase) && phase > 0)
        return phase;
    
    // Metodo 2: UserProperty
    if (obj.GetUserProperty("PHASE_NUMBER", ref phase) && phase > 0)
        return phase;
    
    // Metodo 3: GetPhase()
    // ... resto del codigo
}
```

---

### Proceso de Desarrollo

#### 1. Fork y Clone
```bash
# Fork el repositorio en GitHub
# Luego clona tu fork
git clone https://github.com/TU_USUARIO/008-CORRECTOR-DE-ATRIBUTOS-TEKLA.git
cd 008-CORRECTOR-DE-ATRIBUTOS-TEKLA
```

#### 2. Crear Rama de Feature
```bash
git checkout -b feature/nueva-funcionalidad
```

#### 3. Desarrollar
- Escribe codigo siguiendo la guia de estilo
- Anade comentarios detallados en espanol
- Documenta decisiones de diseno
- Prueba exhaustivamente en Tekla

#### 4. Commit y Push
```bash
git add .
git commit -m "feat: Descripcion de la funcionalidad"
git push origin feature/nueva-funcionalidad
```

#### 5. Pull Request
- Crea un PR desde tu rama a `main`
- Describe los cambios realizados
- Menciona issues relacionados
- Espera revision del equipo

---

## Licencia

Copyright (c) 1992-2024 Trimble Solutions Corporation and its licensors. All rights reserved.

Este proyecto utiliza las APIs de Tekla Structures, que estan sujetas a los terminos de licencia de Trimble Solutions Corporation.

---

## Autores

**Grupo Adipsa**  
- Repository: [GrupoAdipsa/008-CORRECTOR-DE-ATRIBUTOS-TEKLA](https://github.com/GrupoAdipsa/008-CORRECTOR-DE-ATRIBUTOS-TEKLA)

---

## Soporte

Necesitas ayuda? Consulta estos recursos:

1. **Documentacion**: Revisa los 40+ documentos en `CORRECTOR DE ATRIBUTOS/CORRECTOR DE ATRIBUTOS/`
2. **Issues**: Abre un issue en GitHub para reportar bugs o solicitar features
3. **Tekla API**: [Tekla Structures API Documentation](https://developer.tekla.com/)

---

## Roadmap

### Version 2.1 (Futuro)
- [ ] Soporte para Tekla 2022/2023
- [ ] Interfaz grafica mejorada
- [ ] Exportar reportes a Excel
- [ ] Sincronizacion de atributos adicionales (Class, Material, etc.)
- [ ] Modo batch para procesamiento masivo

### Version 2.2 (Futuro)
- [ ] Integracion con bases de datos
- [ ] API REST para automatizacion
- [ ] Plugin de Tekla integrado
- [ ] Multi-idioma (ingles, espanol, etc.)

---

## Estado del Proyecto

| Componente | Estado | Version |
|------------|--------|---------|
| Sincronizador de Assemblies | Completo | 2.0 |
| Macro de Soldaduras | Completo | 2.0 |
| Documentacion | Completa | 2.0 |
| Tests | Manual | - |
| CI/CD | Pendiente | - |

---

## Agradecimientos

Agradecimientos especiales a:
- **Trimble Solutions Corporation** por proporcionar las APIs de Tekla Structures
- **Equipo de Grupo Adipsa** por el desarrollo y mantenimiento
- **Comunidad de Tekla** por el apoyo y feedback

---

<div align="center">

**Listo para sincronizar tus Assemblies de Tekla!**

Si este proyecto te ha sido util, por favor dale una estrella en GitHub

</div>

---

**Ultima actualizacion**: 2024  
**Version del README**: 1.0  
**Estado**: Produccion
