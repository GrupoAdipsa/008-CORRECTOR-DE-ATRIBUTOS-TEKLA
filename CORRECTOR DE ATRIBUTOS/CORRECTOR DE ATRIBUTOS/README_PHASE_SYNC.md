# Sincronización de Phase en Tekla Structures

## ?? Descripción

Módulo para sincronizar automáticamente la **Phase** de un **Assembly** completo en Tekla Structures. 
Toma la Phase de la **Main Part** y la propaga a todas las **secondary parts**, **bolts** y **welds** asociadas.

---

## ?? Funcionalidades

? **Selección flexible**: Puedes seleccionar assemblies directos o cualquier pieza (el sistema identifica su assembly)  
? **Sincronización completa**: Secondary parts + Bolts + Welds  
? **Optimización automática**: Solo modifica objetos que necesitan cambio (evita `Modify()` innecesarios)  
? **Reporte detallado**: Muestra totales evaluados, cambiados y omitidos por tipo  
? **Manejo de errores**: Captura errores por objeto sin abortar el proceso completo  
? **Performance**: Un solo `CommitChanges()` al final del proceso  

---

## ?? Cómo Usar

### **Opción 1: Ejecutar desde Visual Studio (Desarrollo/Pruebas)**

1. Abre el proyecto en **Visual Studio 2022**
2. Asegúrate de que **Tekla Structures 2021** esté abierto con un modelo activo
3. Establece `PhaseSyncLauncher` como proyecto de inicio o modifica el `Main()` principal
4. Presiona **F5** para ejecutar
5. En la ventana que aparece, haz clic en **"Ejecutar Sincronización"**
6. Selecciona las piezas/assemblies en Tekla (puedes seleccionar múltiples con Ctrl)
7. Presiona el botón central del mouse o Enter para confirmar selección
8. Revisa el reporte de resultados en la ventana

### **Opción 2: Compilar como DLL e instalar en Tekla**

1. Compila el proyecto en modo **Release**:
   ```
   dotnet build --configuration Release
   ```

2. Copia el archivo `.dll` generado desde:
   ```
   Installer/BuildDrop/CORRECTOR_DE_ATRIBUTOS.dll
   ```

3. Pega la DLL en la carpeta de aplicaciones de Tekla:
   ```
   C:\TeklaStructuresModels\<versión>\Applications\
   ```
   O en la carpeta global:
   ```
   C:\ProgramData\Trimble\Tekla Structures\<versión>\Applications\
   ```

4. Reinicia Tekla Structures

5. Ve a **Applications & Components** en Tekla

6. Busca **"CORRECTOR_DE_ATRIBUTOS"** o **"Phase Synchronizer"**

7. Haz doble clic para ejecutar

---

## ?? Ejemplo de Reporte

```
???????????????????????????????????????????????????
  REPORTE DE SINCRONIZACIÓN DE PHASE
???????????????????????????????????????????????????

Assemblies procesados: 3

???????????????????????????????????????????????????
SECONDARY PARTS:
  • Evaluadas:  45
  • Cambiadas:  38
  • Omitidas:   7

BOLTS:
  • Evaluados:  120
  • Cambiados:  115
  • Omitidos:   5

WELDS:
  • Evaluadas:  28
  • Cambiadas:  28
  • Omitidas:   0

???????????????????????????????????????????????????
TOTALES:
  • Total evaluados: 193
  • Total cambiados: 181
  • Total omitidos:  12
```

---

## ?? Configuración de Phase

El sistema usa las siguientes **User Properties** de Tekla para manejar Phase:

- **`PHASE_NUMBER`**: Número de fase (principal)
- **`PHASE_NAME`**: Nombre de fase (opcional)

Estos valores son **estándar en Tekla** y se manejan mediante:
- `GetUserProperty("PHASE_NUMBER", ref phaseNumber)`
- `SetUserProperty("PHASE_NUMBER", phaseNumber)`

---

## ?? Arquitectura del Código

### **Archivos Principales**

| Archivo | Descripción |
|---------|-------------|
| `PhaseSynchronizer.cs` | Lógica principal de sincronización |
| `SyncReport.cs` | Manejo de reportes y contadores |
| `PhaseSyncForm.cs` | Formulario de interfaz de usuario |
| `PhaseSyncLauncher.cs` | Punto de entrada para ejecutar la herramienta |

### **Flujo de Trabajo**

```
1. Usuario selecciona objetos en Tekla
   ?
2. Sistema agrupa por Assembly (evita duplicados)
   ?
3. Por cada Assembly:
   a. Obtener Main Part
   b. Leer Phase de Main Part (referencia)
   c. Sincronizar Secondary Parts
   d. Sincronizar Bolts (de todas las parts)
   e. Sincronizar Welds (de todas las parts)
   ?
4. CommitChanges() UNA VEZ
   ?
5. Mostrar reporte consolidado
```

---

## ??? Manejo de Casos Especiales

| Escenario | Comportamiento |
|-----------|----------------|
| **Phase ya correcta** | Se omite (no ejecuta `Modify()`) |
| **Main Part sin Phase** | Se registra advertencia y no se procesa el Assembly |
| **Modify() devuelve false** | Se registra advertencia, cuenta como omitido |
| **Objeto bloqueado/sin permisos** | Se captura excepción, se registra error, continúa con el siguiente |
| **Selección de múltiples assemblies** | Se agrupa y procesa cada uno independientemente |
| **Usuario selecciona Bolt/Weld directamente** | Se resuelve su Assembly a través de la parte primaria |

---

## ?? Requisitos del Sistema

- **Tekla Structures**: 2021 o superior
- **Framework**: .NET Framework 4.8
- **NuGet Packages**:
  - `Tekla.Structures` (2021.0.0)
  - `Tekla.Structures.Model` (2021.0.0)
  - `Tekla.Structures.Dialog` (2021.0.0)

---

## ?? Solución de Problemas

### **"No hay conexión con Tekla Structures"**
- Asegúrate de que Tekla esté abierto con un modelo activo antes de ejecutar la herramienta

### **"No se seleccionaron objetos"**
- Haz clic en el botón de ejecución y luego selecciona las piezas en Tekla
- Confirma la selección con el botón central del mouse o Enter

### **"No se pudieron identificar Assemblies válidos"**
- Verifica que las piezas seleccionadas pertenezcan a un Assembly válido
- Las piezas sueltas sin Assembly no pueden ser procesadas

### **Bolts/Welds no se sincronizan**
- Algunos bolts/welds pueden no tener la propiedad `PHASE_NUMBER` si fueron creados de forma especial
- Revisa el reporte de errores para detalles específicos

---

## ?? Notas para Desarrolladores

### **Optimizaciones de Performance**

1. **Commit único**: Se ejecuta `model.CommitChanges()` solo una vez al final
2. **Verificación previa**: Se lee la Phase actual antes de modificar
3. **Agrupación por Assembly**: Evita procesar el mismo Assembly múltiples veces

### **Extensiones Futuras Posibles**

- [ ] Modo batch desde archivo Excel/CSV
- [ ] Filtros adicionales (por perfil, material, etc.)
- [ ] Sincronización inversa (secondary ? main)
- [ ] Historial de cambios con undo/redo
- [ ] Interfaz de usuario mejorada con preview de cambios

---

## ?? Autor

Desarrollado para **Grupo Adipsa**  
Repositorio: https://github.com/GrupoAdipsa/003-COMPARAR-PIEZAS-TEKLA

---

## ?? Licencia

Copyright © 1992-2025 Trimble Solutions Corporation and its licensors. All rights reserved.

---

## ?? Soporte

Para dudas o problemas, contacta al equipo de desarrollo o revisa la documentación de Tekla Open API.
