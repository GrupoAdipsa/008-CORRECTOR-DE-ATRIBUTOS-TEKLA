# ?? SOLUCIÓN: Cómo Asignar Phase a las Piezas en Tekla

## ?? **Tu Problema Actual**

```
ADVERTENCIAS (2):
  ? Assembly 3236896: Main Part no tiene Phase asignada
  ? Assembly 3337734: Main Part no tiene Phase asignada
```

**Traducción:** Las piezas principales de los assemblies seleccionados **no tienen Phase configurada**.

---

## ? **SOLUCIÓN PASO A PASO**

### **1. En Tekla Structures, selecciona una pieza (Main Part)**

Haz clic en la pieza principal del assembly que quieres sincronizar.

---

### **2. Abre las Propiedades de la Pieza**

**Opción A:** Haz **doble clic** en la pieza  
**Opción B:** Presiona **Alt + Enter**  
**Opción C:** Clic derecho ? **Properties** (Propiedades)

Se abrirá la ventana de **Part Properties** (Propiedades de la Pieza).

---

### **3. Ve a la Pestaña "Phase" (Fase)**

En la ventana de propiedades, busca y haz clic en la pestaña llamada:
- **"Phase"** (en inglés)
- **"Fase"** (en español)

---

### **4. Asigna el Phase Number**

En el campo **"Phase number"** (Número de fase), escribe un número:
- Por ejemplo: `1`, `2`, `3`, etc.
- Este número representa la fase de construcción

**Opcional:** También puedes llenar el campo **"Phase name"** (Nombre de fase):
- Por ejemplo: `"Estructura"`, `"Fase 1"`, `"Acabados"`, etc.

---

### **5. Guarda los Cambios**

Haz clic en el botón **"Modify"** (Modificar) en la parte inferior de la ventana.

---

### **6. Repite para Otros Assemblies**

Si tienes más assemblies, repite los pasos 1-5 para cada uno.

---

## ?? **Ahora Ejecuta la Aplicación de Nuevo**

1. **Ejecuta la aplicación** (doble clic en `ejecutar.bat` o presiona F5 en Visual Studio)
2. Haz clic en **"Ejecutar Sincronización"**
3. **Selecciona las piezas** (las que ya tienen Phase asignada)
4. Presiona el **botón central del mouse** (rueda) o **Enter**
5. **¡Listo!** Ahora debería sincronizar correctamente

---

## ?? **Resultado Esperado**

Después de asignar Phase y ejecutar de nuevo, deberías ver algo como:

```
???????????????????????????????????????????????????
  REPORTE DE SINCRONIZACIÓN DE PHASE
???????????????????????????????????????????????????

Assemblies procesados: 2

???????????????????????????????????????????????????
SECONDARY PARTS:
  • Evaluadas:  12
  • Cambiadas:  12
  • Omitidas:   0

BOLTS:
  • Evaluados:  24
  • Cambiados:  24
  • Omitidos:   0

WELDS:
  • Evaluadas:  6
  • Cambiadas:  6
  • Omitidas:   0
```

---

## ??? **Verificación Rápida**

Para verificar que una pieza tiene Phase asignada:

1. Selecciona la pieza en Tekla
2. Mira la barra de estado inferior
3. Debería mostrar algo como: `Phase: 1` o `Fase: 1`

Si no muestra nada, la pieza **NO tiene Phase** y necesitas asignarla.

---

## ? **¿Por Qué Necesito Asignar Phase?**

La herramienta **lee la Phase de la Main Part** y la **copia** a:
- Todas las **secondary parts** del assembly
- Todos los **bolts** (pernos/tornillos)
- Todas las **welds** (soldaduras)

Si la Main Part **no tiene Phase**, no hay nada que copiar, por eso la herramienta te advierte.

---

## ?? **Resumen Rápido**

1. ? Selecciona pieza en Tekla
2. ? Doble clic para abrir propiedades
3. ? Ve a pestaña "Phase"
4. ? Asigna Phase Number (ej: 1, 2, 3...)
5. ? Haz clic en "Modify"
6. ? Ejecuta la aplicación de nuevo

---

## ?? **¿Sigue sin funcionar?**

Si después de asignar Phase sigue dando error, verifica:

- ? Que estés editando la **Main Part** (no una secondary part)
- ? Que hayas presionado **"Modify"** para guardar
- ? Que el modelo esté guardado (Ctrl + S en Tekla)
- ? Que Tekla esté en modo de edición (no en vista de solo lectura)

---

**¡Pruébalo y me cuentas cómo te fue!** ??
