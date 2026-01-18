# Correccion: Scripts de Instalacion de Macro

## Problema Identificado

Los scripts `instalar_macro.bat` y `verificar_macro.bat` tenian varios problemas:

### Problema 1: Nombre de Archivo Incorrecto
**Error**: Buscaban `SyncWeldPhaseFromParts.cs` pero el archivo real es `SyncWeldPhaseFromParts_OLD.cs`

### Problema 2: Ruta Hardcodeada Incorrecta
**Error**: Usaban una ruta absoluta hardcodeada a un proyecto diferente:
```batch
set BASE_DIR=C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\
```

**Deberia ser**: Ruta relativa desde el directorio del script.

### Problema 3: Estructura de Directorios Incorrecta
**Error**: No buscaba correctamente en la estructura real del proyecto.

**Estructura real**:
```
008-CORRECTOR-DE-ATRIBUTOS-TEKLA\           (Raiz del repositorio)
    |
    +-- MacroPlantilla\                      (Aqui esta la macro)
    |     +-- SyncWeldPhaseFromParts_OLD.cs
    |
    +-- CORRECTOR DE ATRIBUTOS\
          +-- CORRECTOR DE ATRIBUTOS\        (Aqui estan los scripts)
                +-- instalar_macro.bat
                +-- verificar_macro.bat
```

---

## Solucion Implementada

### 1. Correccion del Nombre de Archivo

**Antes:**
```batch
set SOURCE=%BASE_DIR%MacroPlantilla\SyncWeldPhaseFromParts.cs
```

**Ahora:**
```batch
set SOURCE=%BASE_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
```

### 2. Busqueda Inteligente de Rutas

**Ahora se busca en multiples ubicaciones**:
```batch
REM Probar dos niveles arriba (desde CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\)
if exist "%SCRIPT_DIR%..\..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\..\ 
    echo [OK] Macro encontrada (dos niveles arriba - raiz del repositorio)
) else if exist "%SCRIPT_DIR%..\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%..\
    echo [OK] Macro encontrada (un nivel arriba)
) else if exist "%SCRIPT_DIR%MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs" (
    set BASE_DIR=%SCRIPT_DIR%
    echo [OK] Macro encontrada (mismo directorio)
) else (
    echo [ERROR] No se puede encontrar el archivo de la macro
    echo ...mensajes de ayuda...
    exit /b 1
)
```

### 3. Mensajes de Error Mejorados

**Ahora muestra**:
- Rutas donde busco el archivo
- Directorio actual y del script
- Instrucciones claras de donde ejecutar

**Ejemplo**:
```
[ERROR] No se puede encontrar el archivo de la macro

Buscando en:
  - C:\...\008-CORRECTOR-DE-ATRIBUTOS-TEKLA\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
  - C:\...\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
  - C:\...\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs

Directorio actual: C:\...\
Directorio del script: C:\...\

NOTA: Asegurate de estar ejecutando el script desde:
  CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\instalar_macro.bat
```

### 4. Copia con Renombrado

**Ahora:**
- El archivo fuente `SyncWeldPhaseFromParts_OLD.cs` se copia como `SyncWeldPhaseFromParts.cs` en Tekla
- Tekla ve el archivo sin el sufijo `_OLD`

```batch
echo NOTA: El archivo _OLD.cs se copia como SyncWeldPhaseFromParts.cs
copy /Y "%SOURCE%" "%TARGET%"
```

---

## Archivos Corregidos

### 1. instalar_macro.bat

**Cambios principales**:
- [x] Busca `SyncWeldPhaseFromParts_OLD.cs` en lugar de `SyncWeldPhaseFromParts.cs`
- [x] Busqueda en 3 ubicaciones relativas (no hardcoded)
- [x] Mensajes [OK], [ERROR], [WARN], [INFO] claros
- [x] Instrucciones de uso mejoradas
- [x] Validacion robusta de rutas

**Nueva estructura del script**:
```batch
1. Determinar directorio del script
2. Buscar archivo fuente en 3 ubicaciones
3. Verificar archivo fuente existe
4. Verificar/crear directorio destino en Tekla
5. Copiar archivo (renombrado sin _OLD)
6. Verificar copia exitosa
7. Mostrar instrucciones de reinicio de Tekla
```

### 2. verificar_macro.bat

**Cambios principales**:
- [x] Busca `SyncWeldPhaseFromParts_OLD.cs` en lugar de `SyncWeldPhaseFromParts.cs`
- [x] Busqueda en 3 ubicaciones relativas
- [x] Comparacion por tamano de archivo (mas rapido que FC)
- [x] Verificacion del directorio de macros de Tekla
- [x] Lista de macros instaladas
- [x] Resumen claro del estado
- [x] Acciones recomendadas contextuales

**Nueva estructura del script**:
```batch
1. Buscar archivo fuente en 3 ubicaciones
2. Verificar archivo destino en Tekla
3. Comparar tamanos de archivos
4. Verificar directorio de macros
5. Listar todas las macros .cs instaladas
6. Mostrar resumen de estado
7. Mostrar acciones recomendadas
```

---

## Como Usar los Scripts Corregidos

### Instalacion de la Macro

**Paso 1: Navegar al directorio correcto**
```cmd
cd "008-CORRECTOR-DE-ATRIBUTOS-TEKLA\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
```

**Paso 2: Ejecutar instalador**
```cmd
instalar_macro.bat
```

**Resultado esperado**:
```
========================================
  INSTALAR MACRO: SyncWeldPhaseFromParts
========================================

Directorio del script: C:\...\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS\
[OK] Macro encontrada (dos niveles arriba - raiz del repositorio)
Directorio base: C:\...\008-CORRECTOR-DE-ATRIBUTOS-TEKLA\

[1/5] Verificando archivo fuente...
    [OK] Archivo fuente encontrado
    Nombre: SyncWeldPhaseFromParts_OLD.cs
    Tamano: 12345 bytes
    Fecha:  ...

[2/5] Verificando directorio destino de Tekla...
    [INFO] Ya existe una version de la macro
    ...

[3/5] Preparando directorio destino...
    [OK] Directorio existe

[4/5] Copiando archivo...
    De: C:\...\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    A:  C:\...\modeling\SyncWeldPhaseFromParts.cs
    NOTA: El archivo _OLD.cs se copia como SyncWeldPhaseFromParts.cs
        1 archivo(s) copiado(s).

[5/5] Verificando instalacion...
    [OK] Archivo copiado correctamente
    [OK] Tamanos coinciden: 12345 bytes

========================================
  INSTALACION EXITOSA
========================================

Ubicacion: C:\...\modeling\SyncWeldPhaseFromParts.cs

========================================
  IMPORTANTE: REINICIAR TEKLA
========================================

Tekla solo detecta macros nuevas al iniciar.

PASOS SIGUIENTES:
1. Cerrar Tekla Structures completamente
2. Volver a abrir Tekla
3. Tools > Macros... > SyncWeldPhaseFromParts
4. Seleccionar la macro y hacer clic en Run

========================================

Presione una tecla para continuar . . .
```

### Verificacion de la Instalacion

**Ejecutar verificador**:
```cmd
verificar_macro.bat
```

**Resultado esperado (si todo esta OK)**:
```
========================================
  VERIFICACION DE MACRO
========================================

[1] Verificando archivo fuente...
    [OK] Archivo fuente existe
    Ubicacion: C:\...\MacroPlantilla\SyncWeldPhaseFromParts_OLD.cs
    Nombre: SyncWeldPhaseFromParts_OLD.cs
    Tamano: 12345 bytes
    ...

[2] Verificando archivo destino...
    [OK] Macro instalada en Tekla
    Ubicacion: C:\...\modeling\SyncWeldPhaseFromParts.cs
    ...

[3] Comparando archivos...
    [OK] Los archivos tienen el mismo tamano: 12345 bytes
    La macro parece estar actualizada

[4] Verificando directorio de macros de Tekla...
    [OK] Directorio de macros existe
    Ubicacion: C:\...\macros\modeling
    
    Macros encontradas en modeling\:
    SyncWeldPhaseFromParts.cs

========================================
  RESUMEN
========================================

[OK] Archivo fuente: SI
[OK] Macro instalada: SI
[OK] Archivos coinciden: SI

========================================
  ACCIONES RECOMENDADAS
========================================

[OK] La macro esta instalada y actualizada

Para usar la macro:
[1] Abrir Tekla Structures
[2] Tools > Macros...
[3] Buscar: SyncWeldPhaseFromParts
[4] Seleccionar y hacer clic en Run

========================================

Presione una tecla para continuar . . .
```

---

## Casos de Error y Soluciones

### Error 1: "Archivo fuente NO encontrado"

**Causa**: Ejecutando el script desde el directorio incorrecto.

**Solucion**:
```cmd
cd "008-CORRECTOR-DE-ATRIBUTOS-TEKLA\CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
instalar_macro.bat
```

### Error 2: "No se pudo copiar el archivo"

**Causa**: Permisos insuficientes.

**Solucion**:
1. Cerrar Tekla completamente
2. Click derecho en `instalar_macro.bat`
3. Seleccionar "Ejecutar como Administrador"

### Error 3: "Archivos tienen diferente tamano"

**Causa**: Version desactualizada instalada.

**Solucion**:
1. Ejecutar `instalar_macro.bat` para actualizar
2. Reiniciar Tekla

### Error 4: "Directorio de macros NO existe"

**Causa**: Tekla no instalado o version diferente.

**Solucion**:
1. Verificar que Tekla 2021.0 este instalado
2. Si usas otra version, editar la ruta en el script:
```batch
set TARGETDIR=C:\ProgramData\Trimble\Tekla Structures\[TU_VERSION]\Environments\common\macros\modeling
```

---

## Estructura de Archivos Correcta

```
008-CORRECTOR-DE-ATRIBUTOS-TEKLA\
    |
    +-- MacroPlantilla\
    |     +-- SyncWeldPhaseFromParts_OLD.cs  <-- ARCHIVO FUENTE
    |
    +-- CORRECTOR DE ATRIBUTOS\
          +-- CORRECTOR DE ATRIBUTOS\
                +-- instalar_macro.bat        <-- SCRIPT INSTALADOR
                +-- verificar_macro.bat       <-- SCRIPT VERIFICADOR
                +-- ejecutar.bat
                +-- PhaseSyncForm.cs
                +-- PhaseSynchronizer.cs
                +-- ...
```

**Destino en Tekla**:
```
C:\ProgramData\Trimble\Tekla Structures\2021.0\
    +-- Environments\
          +-- common\
                +-- macros\
                      +-- modeling\
                            +-- SyncWeldPhaseFromParts.cs  <-- MACRO INSTALADA
```

---

## Testing

### Checklist de Verificacion:

- [x] Script encuentra archivo fuente
- [x] Script crea directorio si no existe
- [x] Script copia archivo correctamente
- [x] Script renombra archivo (sin _OLD)
- [x] Script verifica tamano coincide
- [x] Script muestra mensajes claros
- [x] verificar_macro.bat funciona
- [x] verificar_macro.bat compara archivos
- [x] verificar_macro.bat lista macros

---

## Conclusiones

### Problemas Resueltos:

1. ? Nombre de archivo corregido (_OLD.cs)
2. ? Rutas relativas en lugar de hardcoded
3. ? Busqueda en multiples ubicaciones
4. ? Mensajes de error claros y utiles
5. ? Validacion robusta
6. ? Instrucciones de uso mejoradas

### Mejoras Adicionales:

1. ? Formato de mensajes consistente [OK], [ERROR], [WARN], [INFO]
2. ? Mejor manejo de errores
3. ? Instrucciones contextuales
4. ? Verificacion de tamano en lugar de comparacion binaria (mas rapido)
5. ? Lista de macros instaladas en Tekla

---

**Version**: 2.0
**Fecha**: 2024
**Estado**: Corregido y Probado
