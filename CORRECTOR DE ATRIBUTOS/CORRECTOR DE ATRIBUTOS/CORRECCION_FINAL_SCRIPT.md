# Corrección Final: Script instalar_macro.bat

## Problema Identificado

El script `instalar_macro.bat` tenía un error de lógica en las condiciones `if-else`. Todos los mensajes "[OK]" se imprimían antes del "[ERROR]", lo que indicaba que la estructura condicional no funcionaba correctamente.

### Error Original

```batch
if exist "%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\..\ 
    echo [OK] Macro encontrada (dos niveles arriba)
) else if exist "%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\
    echo [OK] Macro encontrada (un nivel arriba)
) else if exist "%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%
    echo [OK] Macro encontrada (mismo directorio)
) else (
    echo [ERROR] No se puede encontrar el archivo
    exit /b 1
)
```

**Problema**: El batch ejecutaba todos los bloques `else if` incluso después de encontrar el archivo, mostrando todos los mensajes.

## Solución Implementada

Usar etiquetas (`labels`) y `goto` para salir del flujo de búsqueda una vez encontrado el archivo:

```batch
set SOURCE=
set BASE_DIR=

REM Probar dos niveles arriba
if exist "%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\..\ 
    set SOURCE=%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo [OK] Macro encontrada: dos niveles arriba - raiz del repositorio
    goto :FOUND
)

REM Probar un nivel arriba
if exist "%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\
    set SOURCE=%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo [OK] Macro encontrada: un nivel arriba
    goto :FOUND
)

REM Probar mismo directorio
if exist "%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%
    set SOURCE=%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    echo [OK] Macro encontrada: mismo directorio
    goto :FOUND
)

REM Si llegamos aqui, no se encontro
echo [ERROR] No se puede encontrar el archivo de la macro
pause
exit /b 1

:FOUND
echo Directorio base: %BASE_DIR%
echo Archivo fuente: %SOURCE%
REM ... continua con la instalacion
```

## Cambios Clave

### 1. Variables Inicializadas

```batch
set SOURCE=
set BASE_DIR=
```

Ahora se inicializan vacías al principio.

### 2. Goto para Flujo de Control

```batch
goto :FOUND
```

Sale inmediatamente del flujo de búsqueda cuando encuentra el archivo.

### 3. Etiqueta :FOUND

```batch
:FOUND
echo Directorio base: %BASE_DIR%
echo Archivo fuente: %SOURCE%
```

Punto de entrada único después de encontrar el archivo.

### 4. Separación de Condiciones

En lugar de `else if` encadenados, ahora cada `if` es independiente con su propio `goto`.

## Resultado

### Antes (No funcionaba):
```
[OK] Macro encontrada (dos niveles arriba - raiz del repositorio
[OK] Macro encontrada (un nivel arriba)
[OK] Macro encontrada (mismo directorio)
[ERROR] No se puede encontrar el archivo de la macro
```

### Ahora (Funciona correctamente):
```
[OK] Macro encontrada: dos niveles arriba - raiz del repositorio
Directorio base: C:\...\
Archivo fuente: C:\...\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs

[1/5] Verificando archivo fuente...
   [OK] Archivo fuente encontrado
   ...
```

## Flujo Corregido

```
1. Inicializar variables SOURCE y BASE_DIR vacías
2. Probar ruta: dos niveles arriba
   ?? Si existe ? Asignar variables ? goto :FOUND
3. Probar ruta: un nivel arriba
   ?? Si existe ? Asignar variables ? goto :FOUND
4. Probar ruta: mismo directorio
   ?? Si existe ? Asignar variables ? goto :FOUND
5. Si ninguna existe ? Mostrar ERROR ? exit /b 1
6. :FOUND ? Continuar con instalación
```

## Prueba Exitosa

```cmd
.\instalar_macro.bat

Resultado:
[OK] Macro encontrada: dos niveles arriba - raiz del repositorio
Directorio base: C:\...\008-CORRECTOR-DE-ATRIBUTOS-TEKLA\...\ 
Archivo fuente: C:\...\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs

[1/5] Verificando archivo fuente...
   [OK] Archivo fuente encontrado
   Nombre: SyncWeldPhaseFromParts_OLD.cs
   Tamano: 14357 bytes
   Fecha:  01/18/2026 12:42 p. m.

...

========================================
  INSTALACION EXITOSA
========================================

Ubicacion: C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\modeling\SyncWeldPhaseFromParts.cs
```

## Lecciones Aprendidas

### Problema con Batch Scripts

En archivos `.bat`, el comportamiento de `else if` puede ser impredecible, especialmente con:
- Variables de entorno complejas
- Rutas con espacios
- Evaluación de múltiples condiciones

### Solución: Usar goto

```batch
if condicion (
    acciones
    goto :siguiente_seccion
)
```

Es más confiable que:

```batch
if condicion (
    acciones
) else if otra_condicion (
    otras_acciones
)
```

### Buenas Prácticas

1. **Inicializar variables** al principio
2. **Usar goto** para flujo de control claro
3. **Una condición por bloque** if
4. **Mensajes descriptivos** para debugging

## Archivos Actualizados

- ? `instalar_macro.bat` (v5.2) - Lógica corregida con goto
- ? `verificar_macro.bat` (v2.0) - Ya funcionaba correctamente
- ? `CORRECCION_SCRIPTS_INSTALACION.md` - Documentación
- ? `RUTA_CORRECTA_MACRO.md` - Documentación de rutas

## Estado Final

| Componente | Estado | Versión |
|------------|--------|---------|
| instalar_macro.bat | ? Funcional | 5.2 |
| verificar_macro.bat | ? Funcional | 2.0 |
| Macro de soldaduras | ? Corregida | 2.1.2 |
| Documentación | ? Completa | - |

---

**Versión**: 5.2
**Fecha**: 2024
**Estado**: Probado y Funcional ?
