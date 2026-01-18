# ?? GUÍA DE PRUEBA EN VIVO: MACRO UNIVERSAL

## ? ESTADO ACTUAL

**Sincronizador ejecutándose** ?

---

## ?? CHECKLIST DE PRUEBA

### **Paso 1: Verificar Sincronizador**
- [ ] Ventana del sincronizador abierta
- [ ] Botón "Ejecutar Sincronización" visible
- [ ] Tekla Structures abierto con modelo

### **Paso 2: Ejecutar Sincronización**
```
1. Click en "Ejecutar Sincronización"
2. Ve a Tekla
3. Selecciona 1 o más assemblies
4. Presiona ENTER o botón central del mouse
```

### **Paso 3: Verificar Procesamiento**
Deberías ver:
- [ ] ? Parts sincronizadas
- [ ] ? Bolts sincronizados
- [ ] ? Welds detectadas (mensaje aparece)

### **Paso 4: Generar Macro Universal**
Cuando aparezca el mensaje:
```
"Se encontraron X soldaduras en Y Phase(s)...
¿Deseas generar la MACRO UNIVERSAL INTELIGENTE?"
```

- [ ] Click en "SÍ"
- [ ] Mensaje muestra ubicación del archivo
- [ ] Click en "SÍ" para abrir directorio (opcional)

### **Paso 5: Verificar Archivo Generado**
```powershell
Get-ChildItem "C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros" -Filter "SyncWeld*.cs"
```

Esperado:
- [ ] SyncWeldPhaseFromParts.cs existe
- [ ] INSTRUCCIONES_SyncWeldPhaseFromParts.txt existe

### **Paso 6: Reiniciar Tekla**
- [ ] Cerrar Tekla Structures
- [ ] Volver a abrir Tekla
- [ ] Abrir el mismo modelo

### **Paso 7: Verificar Macro en Tekla**
```
Tools > Macros...
```
- [ ] SyncWeldPhaseFromParts aparece en la lista

### **Paso 8: Usar Macro Universal**

#### 8.1: Seleccionar Soldaduras
```
Opción recomendada para prueba:
1. Selecciona UN assembly completo
2. Click derecho
3. Select in Selected Assembly > Welds
```

#### 8.2: Ejecutar Macro
```
1. Tools > Macros...
2. Seleccionar: SyncWeldPhaseFromParts
3. Click "Run"
4. Esperar 2-3 segundos (compilación primera vez)
```

#### 8.3: Revisar Análisis
La macro mostrará:
```
???????????????????????????????????????
  ANÁLISIS DE SOLDADURAS
???????????????????????????????????????

Total procesadas: X
Soldaduras omitidas: Y

SOLDADURAS AGRUPADAS POR PHASE:

  • Phase 1: 10 soldaduras
  • Phase 2: 5 soldaduras
  • Phase 3: 8 soldaduras

Total a actualizar: 23

¿Deseas procesar estas soldaduras con el Phase Manager?
```

- [ ] Análisis correcto mostrado
- [ ] Click en "Yes"

#### 8.4: Verificar Procesamiento
La macro procesará automáticamente:
- [ ] Mensaje de procesamiento aparece
- [ ] Reporte final muestra estadísticas

#### 8.5: Reporte Final
```
???????????????????????????????????????
  SINCRONIZACIÓN COMPLETADA
???????????????????????????????????????

Soldaduras procesadas: 23
Soldaduras actualizadas: 23
Soldaduras omitidas: 0
```

- [ ] Reporte final correcto

### **Paso 9: Verificar Resultados**
```
1. Seleccionar una soldadura actualizada
2. Doble click ? Propiedades
3. Pestaña "Phase"
4. Verificar Phase correcto
```
- [ ] Phase coincide con pieza conectada

---

## ?? PUNTOS CRÍTICOS A VERIFICAR

### ? Funcionalidad Core:
1. [ ] **Una sola macro generada** (no múltiples)
2. [ ] **Nombre correcto**: SyncWeldPhaseFromParts.cs
3. [ ] **Detección automática** de Phase
4. [ ] **Agrupación por Phase** funciona
5. [ ] **Procesamiento múltiple** en una ejecución

### ? Experiencia de Usuario:
1. [ ] **Mensajes claros** en cada paso
2. [ ] **Instrucciones precisas** en diálogos
3. [ ] **Archivo de instrucciones** generado
4. [ ] **Reporte detallado** al final
5. [ ] **Sin errores** durante ejecución

---

## ?? PROBLEMAS COMUNES

### Problema 1: "Macro no aparece"
**Causa**: Tekla no reiniciado  
**Solución**: Reiniciar Tekla completamente

### Problema 2: "Error al ejecutar macro"
**Causa**: No hay soldaduras seleccionadas  
**Solución**: Seleccionar soldaduras antes de ejecutar

### Problema 3: "Soldaduras omitidas"
**Causa**: Piezas sin Phase asignada  
**Solución**: Asignar Phase a las piezas principales

### Problema 4: "No se encontró directorio de macros"
**Causa**: Instalación de Tekla no estándar  
**Solución**: Verificar ruta en variables de entorno

---

## ?? MÉTRICAS ESPERADAS

### Tiempo de Ejecución:
- Setup inicial: ~5 minutos
- Generar macro: ~1 minuto
- Reiniciar Tekla: ~30 segundos
- Usar macro: ~30 segundos
- **Total**: ~7 minutos

### Resultados:
- Archivos generados: **1** (macro universal)
- Reinicios necesarios: **1**
- Soldaduras procesadas: **Todas en una ejecución**
- Ahorro vs manual: **~95%**

---

## ? CRITERIOS DE ÉXITO

La prueba es exitosa si:

1. ? Se genera **UNA sola macro** (no múltiples)
2. ? Macro se llama `SyncWeldPhaseFromParts.cs`
3. ? Macro detecta Phase automáticamente
4. ? Agrupa soldaduras correctamente
5. ? Procesa múltiples Phases en una ejecución
6. ? Soldaduras terminan con Phase correcto
7. ? Sin errores críticos

---

## ?? NOTAS DE PRUEBA

### Observaciones:
```
(Anota aquí cualquier observación durante la prueba)
```

### Errores Encontrados:
```
(Describe cualquier error)
```

### Tiempos Medidos:
- Sincronización: _____ segundos
- Generación macro: _____ segundos
- Ejecución macro: _____ segundos
- Total: _____ minutos

### Resultado Final:
- [ ] ? EXITOSO
- [ ] ?? PARCIAL (con observaciones)
- [ ] ? FALLIDO (requiere correcciones)

---

**Estado**: ?? EN PRUEBA  
**Iniciado**: AHORA  
**Esperado**: ? EXITOSO
