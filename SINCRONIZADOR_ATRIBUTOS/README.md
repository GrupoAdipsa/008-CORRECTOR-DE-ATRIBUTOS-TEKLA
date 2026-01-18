# Sincronizador de Atributos Personalizados

Sistema automatizado para sincronizar atributos personalizados (`ESTATUS_PIEZA` y `PRIORIDAD`) en Tekla Structures, diseñado para mantener la coherencia de estos atributos entre Main Parts, Secondary Parts y Bolts dentro de los Assemblies.

## ?? Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Atributos Sincronizados](#atributos-sincronizados)
- [Características](#características)
- [Requisitos](#requisitos)
- [Instalación](#instalación)
- [Uso](#uso)
- [Arquitectura](#arquitectura)
- [Comparación con Sincronizador de Phase](#comparación-con-sincronizador-de-phase)

---

## ?? Descripción General

**Sincronizador de Atributos Personalizados** es una aplicación standalone para Tekla Structures que automatiza la sincronización de atributos personalizados de usuario.

### ¿Qué problema resuelve?

En proyectos de Tekla Structures, mantener la coherencia de atributos personalizados como el estatus de pieza y la prioridad de detallado puede ser tedioso cuando tienes muchos Assemblies. Este sistema automatiza completamente el proceso:

- ? Detecta automáticamente los valores de ESTATUS_PIEZA y PRIORIDAD de la Main Part
- ? Propaga estos valores a todas las Secondary Parts del Assembly
- ? Actualiza todos los Bolts del Assembly
- ? Genera reportes detallados de todos los cambios
- ? Procesa por lotes para máximo rendimiento

---

## ??? Atributos Sincronizados

### 1. ESTATUS_PIEZA

**Nombre en Tekla**: `"Estatus de Pieza:"`  
**Nombre interno**: `ESTATUS_PIEZA`  
**Tipo**: Option (lista de valores)

**Valores posibles:**
- (vacío)
- Programado
- Conectado
- Detallado
- Revisado
- Liberado

**Uso**: Indica el estado de avance de la pieza en el proceso de fabricación/detallado.

### 2. PRIORIDAD

**Nombre en Tekla**: `"PRIORIDAD DETALLADO:"`  
**Nombre interno**: `PRIORIDAD`  
**Tipo**: String (texto libre)

**Uso**: Indica la prioridad de detallado de la pieza (ej: "Alta", "Media", "Baja", "Urgente", etc.)

---

## ? Características

### Sincronización Automatizada

- **Lectura inteligente**: Usa múltiples métodos para leer atributos (compatibilidad con diferentes versiones de Tekla)
- **Escritura robusta**: Intenta múltiples métodos para asegurar que los cambios se apliquen
- **Procesamiento por lotes**: Un solo `CommitChanges()` al final para máxima eficiencia
- **Manejo de errores**: Continúa procesando aunque algunas piezas fallen

### Dos Modos de Operación

1. **Procesar Objetos Seleccionados**
   - Ideal para correcciones puntuales
   - Selecciona Assemblies, Parts o Bolts específicos
   - Procesamiento rápido

2. **Sincronizar Todo el Modelo**
   - Ideal para sincronización inicial
   - Procesa todos los Assemblies del modelo
   - Confirmación antes de ejecutar

### Reportes Detallados

- Estadísticas completas de procesamiento
- Lista de errores y advertencias
- Información de cada Assembly procesado
- Posibilidad de copiar reporte al portapapeles

### Interfaz Amigable

- Ventana posicionada para no tapar Tekla
- Botones claros y descriptivos
- Información de atributos visible
- Diseño moderno y profesional

---

## ?? Requisitos

### Software Requerido

- **Tekla Structures 2021.0** o superior
- **.NET Framework 4.8**
- **Windows** (7, 8, 10, 11)

### Dependencias NuGet

```xml
<PackageReference Include="Tekla.Structures" Version="2021.0.0" />
<PackageReference Include="Tekla.Structures.Model" Version="2021.0.0" />
```

---

## ?? Instalación

### Paso 1: Clonar o Descargar

Si ya tienes el repositorio del Corrector de Atributos Tekla, esta aplicación está incluida en la carpeta `SINCRONIZADOR_ATRIBUTOS/`.

### Paso 2: Compilar la Aplicación

```cmd
cd SINCRONIZADOR_ATRIBUTOS
compilar.bat
```

El ejecutable se generará en: `bin\Release\net48\SINCRONIZADOR_ATRIBUTOS.exe`

### Paso 3: Verificar Instalación

Verifica que el ejecutable existe:
- ? `SINCRONIZADOR_ATRIBUTOS\bin\Release\net48\SINCRONIZADOR_ATRIBUTOS.exe`

---

## ?? Uso

### Ejecución

#### Opción A: Usando el Script

```cmd
cd SINCRONIZADOR_ATRIBUTOS
ejecutar.bat
```

#### Opción B: Ejecutando Directamente

```cmd
cd SINCRONIZADOR_ATRIBUTOS\bin\Release\net48
SINCRONIZADOR_ATRIBUTOS.exe
```

### Flujo de Trabajo Recomendado

#### Para Proyectos Nuevos

```
1. Asignar ESTATUS_PIEZA y PRIORIDAD a Main Parts en Tekla
   |
   v
2. Ejecutar Sincronizador: "Sincronizar Todo el Modelo"
   |
   v
3. Verificar reporte
   |
   v
4. ¡Proyecto completo sincronizado!
```

#### Para Correcciones Puntuales

```
1. Identificar Assembly con atributos incorrectos
   |
   v
2. Modificar Main Part en Tekla
   |
   v
3. Seleccionar el Assembly (o sus Parts)
   |
   v
4. Ejecutar Sincronizador: "Procesar Objetos Seleccionados"
   |
   v
5. Verificar reporte
```

#### Para Mantenimiento Continuo

```
1. Agregar nuevos Assemblies al proyecto
   |
   v
2. Asignar atributos a las Main Parts
   |
   v
3. Seleccionar los nuevos Assemblies
   |
   v
4. Ejecutar Sincronizador: "Procesar Objetos Seleccionados"
```

### Ejemplo de Reporte

```
===============================================
  REPORTE DE SINCRONIZACION DE ATRIBUTOS
  Atributos: ESTATUS_PIEZA y PRIORIDAD
===============================================

Assemblies procesados: 25

--- PARTS ---
Total evaluadas: 180
  Modificadas:   175
  Sin cambios:   5

--- BOLTS ---
Total evaluados: 120
  Modificados:   118
  Sin cambios:   2

[OK] Sincronización completada exitosamente

===============================================
```

---

## ??? Arquitectura

### Componentes Principales

```
CustomAttributeSyncLauncher.cs (Main Entry Point)
    |
    +-- Crea --> CustomAttributeSyncForm.cs (Interfaz Gráfica)
            |
            +-- Crea --> CustomAttributeSynchronizer.cs (Motor Core)
                    |
                    +-- Genera --> CustomAttributeReport.cs (Reportes)
```

### Archivos del Proyecto

| Archivo | Descripción |
|---------|-------------|
| `CustomAttributeSyncLauncher.cs` | Punto de entrada (Main) |
| `CustomAttributeSyncForm.cs` | Interfaz gráfica Windows Forms |
| `CustomAttributeSynchronizer.cs` | Motor de sincronización |
| `CustomAttributeReport.cs` | Sistema de reportes |
| `SINCRONIZADOR_ATRIBUTOS.csproj` | Archivo de proyecto |
| `compilar.bat` | Script de compilación |
| `ejecutar.bat` | Script de ejecución |

### Flujo de Sincronización

```
1. Usuario selecciona objetos (o todo el modelo)
   |
   v
2. Sistema agrupa objetos por Assembly
   |
   v
3. Para cada Assembly:
   |
   +-- Lee ESTATUS_PIEZA y PRIORIDAD de Main Part
   +-- Propaga a todas las Secondary Parts
   +-- Propaga a todos los Bolts
   |
   v
4. Commit de cambios (una sola vez)
   |
   v
5. Genera reporte detallado
```

### Métodos de Lectura/Escritura

El sistema usa múltiples métodos para asegurar compatibilidad:

**Lectura**:
1. `GetUserProperty("ESTATUS_PIEZA")` / `GetUserProperty("PRIORIDAD")`
2. `GetReportProperty("ESTATUS_PIEZA")` / `GetReportProperty("PRIORIDAD")`
3. Nombres alternativos: `"Estatus de Pieza:"`, `"PRIORIDAD DETALLADO:"`

**Escritura**:
1. `SetUserProperty("ESTATUS_PIEZA", value)` / `SetUserProperty("PRIORIDAD", value)`
2. `SetUserProperty("Estatus de Pieza:", value)` / `SetUserProperty("PRIORIDAD DETALLADO:", value)`

---

## ?? Comparación con Sincronizador de Phase

| Característica | Sincronizador de Phase | Sincronizador de Atributos |
|----------------|----------------------|---------------------------|
| **Atributos** | Phase (número) | ESTATUS_PIEZA y PRIORIDAD (strings) |
| **Objetos sincronizados** | Parts, Bolts | Parts, Bolts |
| **Soldaduras** | ? Macro separada | ? No aplica |
| **Dos modos** | ? Sí | ? Sí |
| **Reportes** | ? Sí | ? Sí |
| **Procesamiento por lotes** | ? Sí | ? Sí |
| **Ventana no tapa Tekla** | ? Sí | ? Sí |

### ¿Cuándo usar cada uno?

**Sincronizador de Phase**:
- Cuando necesites sincronizar fases de construcción
- Incluye sincronización de soldaduras (Welds)
- Atributo estándar de Tekla

**Sincronizador de Atributos Personalizados**:
- Cuando necesites sincronizar atributos de usuario
- Para mantener coherencia en estatus de detallado
- Para gestionar prioridades de fabricación
- No incluye soldaduras (no aplica para estos atributos)

**Ambos son independientes y pueden usarse juntos** en el mismo proyecto.

---

## ?? Solución de Problemas

### Problema: "No hay conexión con Tekla Structures"

**Solución**:
1. Abre Tekla Structures
2. Abre o crea un modelo
3. Ejecuta la aplicación nuevamente

### Problema: "Main Part sin ESTATUS_PIEZA ni PRIORIDAD"

**Solución**:
1. Selecciona la Main Part en Tekla
2. Abre propiedades (doble clic)
3. Busca los campos `ESTATUS_PIEZA` y `PRIORIDAD`
4. Asigna valores
5. Guarda y ejecuta el sincronizador nuevamente

### Problema: Error de compilación

**Solución**:
```cmd
# Restaurar dependencias
dotnet restore

# Limpiar
dotnet clean

# Recompilar
dotnet build --configuration Release
```

---

## ?? Licencia

Este proyecto utiliza las APIs de Tekla Structures, que están sujetas a los términos de licencia de Trimble Solutions Corporation.

---

## ?? Autores

**Grupo Adipsa**  
- Repository: [GrupoAdipsa/008-CORRECTOR-DE-ATRIBUTOS-TEKLA](https://github.com/GrupoAdipsa/008-CORRECTOR-DE-ATRIBUTOS-TEKLA)

---

## ?? Soporte

¿Necesitas ayuda?

1. Revisa este README
2. Consulta la documentación del Sincronizador de Phase (lógica similar)
3. Abre un issue en GitHub

---

<div align="center">

**¡Listo para sincronizar tus atributos personalizados!**

</div>

---

**Última actualización**: 2024  
**Versión**: 1.0  
**Estado**: Listo para producción
