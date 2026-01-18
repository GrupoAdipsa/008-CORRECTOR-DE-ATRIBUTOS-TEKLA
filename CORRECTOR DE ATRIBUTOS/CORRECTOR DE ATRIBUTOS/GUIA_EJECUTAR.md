# ?? GUÍA: Cómo Ejecutar la Aplicación de Sincronización de Phase

## ? **OPCIÓN 1: Ejecutar desde Visual Studio (RECOMENDADO para pruebas)**

### **Pasos:**

1. **Abre Tekla Structures 2021**
   - Abre cualquier modelo o crea uno nuevo
   - Asegúrate de que el modelo esté activo

2. **En Visual Studio 2022:**
   - Verifica que el proyecto **CORRECTOR DE ATRIBUTOS** esté seleccionado como proyecto de inicio
     - En el **Explorador de soluciones**, haz clic derecho en el proyecto
     - Si no dice "Proyecto de inicio", selecciona **"Establecer como proyecto de inicio"**

3. **Presiona F5** (o haz clic en el botón ? **Iniciar**)
   - La aplicación se compilará
   - Se abrirá una ventana con el título **"Sincronización de Phase - Tekla"**

4. **En la ventana que aparece:**
   - Haz clic en el botón **"Ejecutar Sincronización"**
   - La ventana se minimizará y Tekla esperará tu selección

5. **En Tekla Structures:**
   - Selecciona una o varias piezas (puedes usar Ctrl + clic para múltiples)
   - También puedes seleccionar assemblies completos
   - Presiona el **botón central del mouse** (rueda) o **Enter** para confirmar

6. **Resultado:**
   - Verás el reporte completo en la ventana de la aplicación
   - Si hay errores o advertencias, aparecerán listados

---

## ? **OPCIÓN 2: Ejecutar el EXE compilado**

### **Ubicación del ejecutable:**

Después de compilar, el archivo `.exe` está en:

```
C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\Installer\BuildDrop\CORRECTOR_DE_ATRIBUTOS.exe
```

### **Pasos:**

1. **Abre Tekla Structures 2021** con un modelo activo

2. **Navega a la carpeta:**
   ```
   C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\Installer\BuildDrop\
   ```

3. **Haz doble clic en:**
   ```
   CORRECTOR_DE_ATRIBUTOS.exe
   ```

4. Sigue los mismos pasos que en la Opción 1 (desde el paso 4)

---

## ? **OPCIÓN 3: Instalar como Plugin de Tekla (RECOMENDADO para uso frecuente)**

### **Pasos de Instalación:**

1. **Compila el proyecto en modo Release:**
   - En Visual Studio, cambia de **Debug** a **Release** en la barra superior
   - Presiona **Ctrl + Shift + B** para compilar

2. **Copia la DLL generada:**
   - Origen: 
     ```
     C:\Users\Kevin Flores\Documents\003-COMPARAR-PIEZAS-TEKLA\CORRECTOR DE ATRIBUTOS\Installer\BuildDrop\CORRECTOR_DE_ATRIBUTOS.dll
     ```
   
   - Destino (elige uno):
     - **Para tu usuario:**
       ```
       C:\TeklaStructuresModels\2021\Applications\
       ```
     - **Para todos los usuarios (requiere permisos de admin):**
       ```
       C:\ProgramData\Trimble\Tekla Structures\2021\Applications\
       ```

3. **Reinicia Tekla Structures**

4. **Ejecutar desde Tekla:**
   - Ve a **Applications & Components** (Aplicaciones y Componentes)
   - Busca **"CORRECTOR_DE_ATRIBUTOS"** o **"Phase Sync"**
   - Haz doble clic para ejecutar

---

## ?? **Solución de Problemas Comunes**

### **"No hay conexión con Tekla Structures"**
- ? Asegúrate de que Tekla esté **abierto** antes de ejecutar la aplicación
- ? Verifica que un **modelo esté activo** en Tekla (no solo la ventana de inicio)

### **"No se seleccionaron objetos"**
- ? Después de hacer clic en "Ejecutar Sincronización", ve a Tekla y selecciona piezas
- ? Confirma la selección con el **botón central del mouse** (rueda) o **Enter**

### **La ventana no aparece**
- ? Revisa la barra de tareas (puede estar minimizada)
- ? Presiona **Alt + Tab** para cambiar entre ventanas

### **Error de compilación en Visual Studio**
- ? Limpia la solución: **Compilar > Limpiar solución**
- ? Reconstruye: **Compilar > Recompilar solución**
- ? Verifica que los paquetes NuGet estén instalados

---

## ?? **¿Qué hace la aplicación?**

1. **Lee la Phase** de la **Main Part** del Assembly
2. **Aplica esa Phase** a:
   - Todas las **Secondary Parts**
   - Todos los **Bolts** (pernos/tornillos)
   - Todas las **Welds** (soldaduras)
3. **Genera un reporte** con:
   - Cuántos objetos se evaluaron
   - Cuántos se cambiaron
   - Cuántos se omitieron (porque ya tenían la Phase correcta)
   - Lista de errores y advertencias

---

## ?? **Ejemplo de Reporte**

```
???????????????????????????????????????????????????
  REPORTE DE SINCRONIZACIÓN DE PHASE
???????????????????????????????????????????????????

Assemblies procesados: 2

???????????????????????????????????????????????????
SECONDARY PARTS:
  • Evaluadas:  25
  • Cambiadas:  23
  • Omitidas:   2

BOLTS:
  • Evaluados:  68
  • Cambiados:  68
  • Omitidos:   0

WELDS:
  • Evaluadas:  12
  • Cambiadas:  10
  • Omitidas:   2

???????????????????????????????????????????????????
TOTALES:
  • Total evaluados: 105
  • Total cambiados: 101
  • Total omitidos:  4
```

---

## ?? **Atajos de Teclado en Visual Studio**

| Atajo | Acción |
|-------|--------|
| **F5** | Iniciar con depuración |
| **Ctrl + F5** | Iniciar sin depuración (más rápido) |
| **Shift + F5** | Detener depuración |
| **Ctrl + Shift + B** | Compilar solución |
| **F9** | Establecer punto de interrupción |

---

## ?? **¿Necesitas más ayuda?**

- Revisa el archivo **README_PHASE_SYNC.md** para documentación completa
- Revisa los ejemplos en **PhaseSyncExamples.cs**
- Contacta al equipo de desarrollo

---

**¡Listo para probar! Presiona F5 en Visual Studio y comienza.** ??
