# GUIA RAPIDA - Nueva Version 2.1

## Que hay de nuevo?

La nueva version del Corrector de Fases incluye 3 mejoras importantes:

1. **Ventana siempre visible**: Se sincroniza con Tekla (maximiza/minimiza automaticamente)
2. **Dos botones**: Uno para objetos seleccionados, otro para todo el modelo
3. **Procesamiento por lotes**: Para modelos grandes sin problemas de memoria

---

## Como Ejecutar

```batch
cd "CORRECTOR DE ATRIBUTOS\CORRECTOR DE ATRIBUTOS"
ejecutar.bat
```

O directamente:
```batch
cd "CORRECTOR DE ATRIBUTOS\Installer\BuildDrop\net48"
CORRECTOR_DE_ATRIBUTOS.exe
```

---

## Interfaz Nueva

```
+----------------------------------------------------------+
| Corrector de Atributos - v2.1                            |
+----------------------------------------------------------+
|                                                          |
| MODO 1 - OBJETOS SELECCIONADOS:                         |
|   Ideal para correcciones puntuales                     |
|                                                          |
| MODO 2 - TODO EL MODELO:                                |
|   Procesamiento por lotes (alto rendimiento)            |
|                                                          |
+----------------------------------------------------------+
| [ Sincronizar Seleccionados ]  [ Sincronizar Todo ]     |
|        (Boton Azul)                 (Boton Verde)       |
+----------------------------------------------------------+
| Reporte:                                                 |
| [                                                      ] |
| [                                                      ] |
| [                                                      ] |
+----------------------------------------------------------+
|                                             [ Cerrar ]   |
+----------------------------------------------------------+
```

---

## Opcion 1: Sincronizar Seleccionados (Boton Azul)

**Cuando usarlo:**
- Correcciones rapidas
- Actualizar algunos assemblies especificos
- Trabajar en zonas especificas del modelo

**Como usarlo:**
1. Clic en boton AZUL "Sincronizar Seleccionados"
2. Selecciona assemblies, parts o bolts en Tekla
3. Presiona ENTER o boton central del mouse
4. Ver reporte inmediato

**Ejemplo de uso:**
```
Situacion: 10 assemblies necesitan correccion

Pasos:
1. Clic en boton azul
2. Seleccionar los 10 assemblies en Tekla
3. ENTER
4. Listo! (5-10 segundos)
```

---

## Opcion 2: Sincronizar Todo el Modelo (Boton Verde)

**Cuando usarlo:**
- Sincronizacion inicial de proyecto nuevo
- Verificacion completa del modelo
- Actualizacion masiva de todas las phases

**Como usarlo:**
1. Clic en boton VERDE "Sincronizar Todo el Modelo"
2. Confirmar mensaje de advertencia (SI/NO)
3. Observar progreso en tiempo real
4. Esperar a que termine (puede tardar varios minutos)
5. Ver reporte completo

**Ejemplo de uso:**
```
Situacion: Modelo con 500 assemblies

Pasos:
1. Clic en boton verde
2. Confirmar: SI
3. Observar progreso:
   - LOTE 1 de 7
   - Progreso: 10/75 (13%)
   - Progreso: 20/75 (27%)
   - ...
   - Guardando cambios del lote 1...
   - Lote 1 completado
4. Esperar 2-3 minutos
5. Listo! Ver reporte completo
```

---

## Progreso en Tiempo Real

Cuando usas "Sincronizar Todo el Modelo", veras:

```
Obteniendo lista de assemblies del modelo...
Total de assemblies encontrados: 350

Procesamiento en 5 lotes de hasta 75 assemblies cada uno
Esto mantiene el rendimiento y estabilidad del sistema

----------------------------------------
LOTE 1 de 5
Procesando assemblies 1 a 75 (75 assemblies)
----------------------------------------
  Progreso del lote: 10/75 (13%)
  Progreso del lote: 20/75 (27%)
  Progreso del lote: 30/75 (40%)
  Progreso del lote: 40/75 (53%)
  Progreso del lote: 50/75 (67%)
  Progreso del lote: 60/75 (80%)
  Progreso del lote: 70/75 (93%)
  Progreso del lote: 75/75 (100%)
Guardando cambios del lote 1...
Lote 1 completado exitosamente

----------------------------------------
LOTE 2 de 5
...
```

---

## Sincronizacion de Ventana con Tekla

**Funcionalidad automatica:**

La ventana del corrector ahora se sincroniza con Tekla:

- Si MAXIMIZAS Tekla ? La ventana del corrector se MAXIMIZA
- Si MINIMIZAS Tekla ? La ventana del corrector se MINIMIZA
- Si RESTAURAS Tekla ? La ventana del corrector se RESTAURA

**Ademas:**
- La ventana siempre permanece AL FRENTE
- No la pierdes de vista mientras trabajas
- Se comporta como parte de Tekla

---

## Comparacion de Modos

| Caracteristica | Seleccionados (Azul) | Todo el Modelo (Verde) |
|----------------|----------------------|------------------------|
| Seleccion manual | Si | No |
| Confirmacion | No | Si |
| Velocidad | Rapida (segundos) | Variable (minutos) |
| Progreso visible | No | Si |
| Ideal para | Correcciones | Sincronizacion completa |
| Memoria usada | Baja | Media |

---

## Consejos y Recomendaciones

### Para Modelos Pequenos (< 100 assemblies):
- Usa cualquier modo
- Ambos son rapidos

### Para Modelos Medianos (100-500 assemblies):
- **Modo seleccionados**: Para zonas especificas
- **Modo completo**: Para sincronizacion inicial (1-3 minutos)

### Para Modelos Grandes (> 500 assemblies):
- **Modo seleccionados**: Para mantenimiento diario
- **Modo completo**: Para sincronizacion mensual (5-10 minutos)

### Si tienes problemas de memoria:
```
Editar: PhaseSynchronizer.cs
Linea ~488: const int BATCH_SIZE = 75;
Cambiar a: const int BATCH_SIZE = 50;
Recompilar
```

---

## Reporte de Ejemplo

### Modo Seleccionados:
```
=== REPORTE DE SINCRONIZACION ===
Assemblies procesados: 10
Parts evaluadas: 85
  - Modificadas: 85
  - Sin cambios: 0
Bolts evaluados: 42
  - Modificados: 42
  - Sin cambios: 0

[OK] Sincronizacion completada exitosamente
```

### Modo Todo el Modelo:
```
=== REPORTE DE SINCRONIZACION ===
Assemblies procesados: 350
Parts evaluadas: 2,840
  - Modificadas: 2,835
  - Sin cambios: 5
Bolts evaluados: 1,456
  - Modificados: 1,450
  - Sin cambios: 6

Tiempo total: 3 minutos 15 segundos
Lotes procesados: 5

[OK] Sincronizacion completa exitosa
```

---

## Soluciones Rapidas

### Problema: La ventana no se sincroniza con Tekla
**Solucion**: 
- Espera 1-2 segundos (busca automaticamente)
- Si no funciona, la ventana seguira al frente de todas formas

### Problema: El modo completo es muy lento
**Solucion**: 
- Normal para modelos grandes
- Ver progreso en tiempo real para confirmar que avanza
- Alternativa: Usar modo seleccionados por zonas

### Problema: Se queda sin memoria
**Solucion**: 
- Reducir BATCH_SIZE de 75 a 50 (ver arriba)
- Cerrar otras aplicaciones
- Usar modo seleccionados por zonas

### Problema: No puedo seleccionar objetos en Tekla
**Solucion**: 
- Verificar que no haya filtros activos en Tekla
- Usar seleccion por area
- Confirmar con ENTER o boton central del mouse

---

## Atajos de Teclado (Tekla)

Durante la seleccion:
- **ENTER**: Confirmar seleccion
- **ESC**: Cancelar seleccion
- **Boton Central Mouse**: Confirmar seleccion
- **Ctrl+A**: Seleccionar todo (cuidado!)

---

## Checklist de Uso

**Antes de sincronizar:**
- [ ] Tekla esta abierto
- [ ] Modelo esta cargado
- [ ] Main Parts tienen Phase asignada

**Durante la sincronizacion:**
- [ ] No cerrar Tekla
- [ ] No cerrar el corrector
- [ ] Observar progreso (modo completo)

**Despues de sincronizar:**
- [ ] Revisar reporte
- [ ] Verificar algunas piezas en Tekla
- [ ] Guardar modelo en Tekla

---

## Proximos Pasos

1. **Probar Modo Seleccionados**
   - Selecciona 5-10 assemblies
   - Verifica que funciona correctamente

2. **Probar Modo Completo**
   - Usa en modelo de prueba primero
   - Observa el progreso por lotes

3. **Verificar Sincronizacion de Ventana**
   - Maximiza/minimiza Tekla
   - Confirma que la ventana se sincroniza

4. **Sincronizar Soldaduras**
   - Recuerda: Las soldaduras se manejan con la macro de Tekla
   - Tools > Macros > SyncWeldPhaseFromParts

---

## Documentacion Completa

- `MEJORAS_IMPLEMENTADAS.md`: Detalles tecnicos
- `RESUMEN_MEJORAS_v2.1.md`: Resumen ejecutivo
- `README.md`: Documentacion general del proyecto

---

**Version**: 2.1
**Estado**: Listo para usar
**Compilacion**: Exitosa

**Listo para sincronizar!** ?
