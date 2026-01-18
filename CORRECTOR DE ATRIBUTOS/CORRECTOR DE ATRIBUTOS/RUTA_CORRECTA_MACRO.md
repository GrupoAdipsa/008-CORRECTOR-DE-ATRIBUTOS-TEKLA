# Ruta Correcta de Instalacion de Macro

## Ubicacion Correcta

La macro **DEBE** instalarse en la siguiente ruta exacta:

```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs
```

## Estructura de Directorios de Tekla

```
C:\ProgramData\Trimble\Tekla Structures\2021.0\
    |
    +-- Environments\
          +-- common\
                +-- macros\
                      +-- modeling\              <-- AQUI va la macro
                            +-- SyncWeldPhaseFromParts.cs
```

## Por que la carpeta "modeling"?

La carpeta `modeling` es donde Tekla busca las macros relacionadas con el modelado. Las macros en esta ubicacion:

1. **Aparecen automaticamente** en el menu `Tools > Macros...`
2. **Se compilan automaticamente** al iniciar Tekla
3. **Estan disponibles** para todos los modelos

## Rutas Incorrectas (NO usar)

? **Incorrecto**: `C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\`
- Falta la carpeta `modeling\`
- La macro NO aparecera en Tekla

? **Incorrecto**: `C:\ProgramData\Trimble\Tekla Structures\2021.0\macros\`
- Ruta inexistente
- Tekla no busca aqui

? **Incorrecto**: Carpeta del usuario
- Las macros de usuario estan en otra ubicacion
- No se comparten entre usuarios

## Scripts de Instalacion

Los scripts `instalar_macro.bat` y `verificar_macro.bat` ya usan la ruta correcta:

```batch
set TARGETDIR=C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling
set TARGET=%TARGETDIR%\SyncWeldPhaseFromParts.cs
```

## Verificar Instalacion

### Metodo 1: Usando el Script

```cmd
cd "CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
verificar_macro.bat
```

Deberia mostrar:
```
[OK] Macro instalada en Tekla
Ubicacion: C:\...\macros\modeling\SyncWeldPhaseFromParts.cs
```

### Metodo 2: Manualmente

1. Abrir Explorador de Windows
2. Navegar a:
   ```
   C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling
   ```
3. Verificar que existe: `SyncWeldPhaseFromParts.cs`

### Metodo 3: Desde Tekla

1. Abrir Tekla Structures
2. `Tools > Macros...`
3. Buscar en la lista: `SyncWeldPhaseFromParts`
4. Si aparece ? Instalacion correcta

## Instalacion Manual

Si los scripts no funcionan, puedes copiar manualmente:

**Paso 1: Ubicar archivo fuente**
```
008-CORRECTOR-DE-ATRIBUTOS-TEKLA\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
```

**Paso 2: Crear directorio destino (si no existe)**
```cmd
mkdir "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling"
```

**Paso 3: Copiar archivo**
```cmd
copy "MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs"
```

**Nota**: El archivo se copia SIN el sufijo `_OLD`.

## Otras Versiones de Tekla

Si usas una version diferente de Tekla, ajusta la ruta:

### Tekla 2019:
```
C:\ProgramData\Trimble\Tekla Structures\2019.0\Environments\common\macros\modeling\
```

### Tekla 2020:
```
C:\ProgramData\Trimble\Tekla Structures\2020.0\Environments\common\macros\modeling\
```

### Tekla 2022:
```
C:\ProgramData\Trimble\Tekla Structures\2022.0\Environments\common\macros\modeling\
```

### Tekla 2023:
```
C:\ProgramData\Trimble\Tekla Structures\2023.0\Environments\common\macros\modeling\
```

## Permisos

La carpeta `C:\ProgramData\` requiere permisos de administrador:

- **Lectura**: Todos los usuarios
- **Escritura**: Requiere elevacion (Administrador)

### Si tienes error de permisos:

1. Click derecho en `instalar_macro.bat`
2. Seleccionar "Ejecutar como Administrador"
3. Aceptar el UAC (Control de Cuentas de Usuario)

## Macros en Otros Idiomas

Tekla detecta macros en estas carpetas segun el idioma:

```
macros\
    +-- modeling\           (Ingles - default)
    +-- modellazione\       (Italiano)
    +-- modellering\        (Holandes)
    +-- modellering_no\     (Noruego)
    +-- modelowanie\        (Polaco)
    +-- etc...
```

**Nota**: Para español, usa `modeling\` (carpeta en ingles).

## Troubleshooting

### Problema: "Macro no aparece en Tekla"

**Solucion**:
1. Verificar que el archivo esta en `modeling\` (no en `macros\` directamente)
2. Verificar que el archivo se llama `SyncWeldPhaseFromParts.cs` (sin `_OLD`)
3. Reiniciar Tekla completamente
4. Verificar que no hay errores de compilacion en el Output de Tekla

### Problema: "Error al copiar archivo"

**Solucion**:
1. Cerrar Tekla (el archivo puede estar en uso)
2. Ejecutar script como Administrador
3. Verificar que la ruta de Tekla existe

### Problema: "Carpeta modeling no existe"

**Solucion**:
```cmd
mkdir "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling"
```

Luego ejecutar `instalar_macro.bat` nuevamente.

## Resumen

? **Ruta correcta**:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs
```

? **Ruta incorrecta**:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\SyncWeldPhaseFromParts.cs
```

?? **Diferencia clave**: La carpeta `modeling\` es **necesaria**.

---

**Actualizacion**: 2024
**Version**: 1.0
