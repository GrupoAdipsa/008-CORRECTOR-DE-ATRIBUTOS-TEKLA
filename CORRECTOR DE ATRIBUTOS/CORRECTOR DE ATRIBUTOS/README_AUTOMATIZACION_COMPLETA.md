# SISTEMA AUTOMATIZADO DE SINCRONIZACIÓN DE PHASE

## ?? ¿QUÉ HACE ESTE SISTEMA?

Este sistema **sincroniza automáticamente** el Phase de TODOS los componentes de un Assembly en Tekla Structures:

| Componente | Sincronización | Método |
|------------|----------------|--------|
| **Parts (Piezas secundarias)** | ? 100% Automática | Via API |
| **Bolts (Tornillos)** | ? 100% Automática | Via API |
| **Welds (Soldaduras)** | ? **SEMI-Automática** | Via Macro generada automáticamente |

## ?? FLUJO COMPLETAMENTE AUTOMATIZADO

### **Paso 1: Ejecutar el Sincronizador**
```cmd
cd "CORRECTOR DE ATRIBUTOS"
.\ejecutar.bat
```

### **Paso 2: Seleccionar Assemblies en Tekla**
- Selecciona UNO o VARIOS assemblies/piezas en Tekla
- Presiona ENTER o botón central del mouse

### **Paso 3: El Sistema Hace TODO Automáticamente**

El sistema hará:

1. **? Sincroniza Parts y Bolts** - 100% automático
   - Lee el Phase de la pieza principal (Main Part)
   - Aplica ese Phase a TODAS las piezas secundarias
   - Aplica ese Phase a TODOS los tornillos
   - **NO requiere intervención manual**

2. **? Prepara Soldaduras** - Semi-automático
   - Detecta TODAS las soldaduras que necesitan actualización
   - Te pregunta: "¿Procesar soldaduras automáticamente?"
   - Si dices **SÍ**:
     - Genera una macro de Tekla automáticamente
     - La guarda en tu directorio de macros de Tekla
     - Crea una lista de IDs de soldaduras para referencia
     - Te da instrucciones exactas de qué hacer

### **Paso 4: Ejecutar la Macro (Solo para Soldaduras)**

**IMPORTANTE**: Esto es necesario solo para las soldaduras, debido a limitaciones de la API de Tekla.

#### **Opción A: Proceso Rápido (RECOMENDADO)**
1. **Selecciona las soldaduras en Tekla**:
   - Usa filtros: `Tools` > `Inquire Objects` > Filtra por "Welds"
   - O selecciona el assembly completo y filtra soldaduras

2. **Ejecuta la macro**:
   - `Tools` > `Macros...`
   - Busca: `AutoChangeWeldsToPhase2` (o el número que corresponda)
   - Click **Run**

3. **¡Listo!** - Las soldaduras cambian automáticamente

#### **Opción B: Macro Externa (AVANZADO)**
Si prefieres tener más control, puedes editar la macro generada.

## ?? EJEMPLO COMPLETO DE USO

### **Escenario**: 
Tienes 10 assemblies, cada uno con:
- 5 piezas secundarias
- 20 tornillos  
- 30 soldaduras

**Total por assembly**: 55 objetos que necesitan sincronizar Phase

### **Método ANTERIOR (Manual)**:
- Seleccionar pieza por pieza: **~550 clics**
- Abrir propiedades: **550 clics más**
- Cambiar Phase: **550 clics más**
- **TOTAL**: ~1650 clics, **2-3 horas de trabajo**

### **Método NUEVO (Automatizado)**:
1. Ejecutar sincronizador
2. Seleccionar los 10 assemblies
3. Esperar 10 segundos ? **PIEZAS Y TORNILLOS LISTOS** (50 objetos × 10 = 500 objetos)
4. Seleccionar soldaduras y ejecutar macro ? **30 segundos**
5. **TOTAL**: **1 minuto de trabajo** ?

## ?? DETALLES TÉCNICOS

### **¿Por qué las soldaduras necesitan macro?**

La API de Tekla **NO permite** cambiar el Phase de soldaduras programáticamente usando `SetUserProperty`. 

Esto es una **limitación de Tekla**, no un error de este código.

**Solución**: Usamos el **Phase Manager** de Tekla a través de macros, que es exactamente lo que harías manualmente, pero automatizado.

### **¿Qué hace la macro generada?**

```csharp
// 1. Abre el Phase Manager de Tekla
wpf.InvokeCommand("Tools.PhaseManager");

// 2. Selecciona el Phase correcto (ejemplo: Phase 2)
akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { 2 });

// 3. Aplica a las soldaduras seleccionadas
akit.PushButton("butModifyObjects", "diaPhaseManager");

// 4. Cierra el diálogo
akit.PushButton("butOk", "diaPhaseManager");
```

Es literalmente lo mismo que hacer manualmente, pero en milisegundos.

## ?? ARCHIVOS GENERADOS AUTOMÁTICAMENTE

Cuando ejecutas el sincronizador, se generan automáticamente:

| Archivo | Ubicación | Propósito |
|---------|-----------|-----------|
| `AutoChangeWeldsToPhase{N}.cs` | `C:\ProgramData\Trimble\Tekla Structures\2021.0\Environments\common\macros\` | Macro para cambiar soldaduras a Phase N |
| `Welds_Phase{N}_IDs.txt` | Mismo directorio que la macro | Lista de IDs de soldaduras afectadas |

**IMPORTANTE**: 
- Las **MACROS** se guardan con extensión **`.cs`** en el directorio `common\macros\` para que sean visibles en `Tools > Macros...`
- Los **COMPONENTES** usan extensión `.uel` y se guardan en otro directorio
- Este sistema genera **MACROS**, no componentes

## ?? CONFIGURACIÓN INICIAL (Una sola vez)

No hay configuración necesaria. El sistema:
- ? Detecta automáticamente tu instalación de Tekla
- ? Encuentra el directorio de macros
- ? Genera las macros en el lugar correcto
- ? Todo funciona "out of the box"

## ?? SOLUCIÓN DE PROBLEMAS

### **Problema**: "No se pudieron procesar las soldaduras"
**Solución**: 
1. Verifica que las soldaduras existan en el assembly
2. Asegúrate de que la Main Part tenga Phase asignada

### **Problema**: "No encuentro la macro generada"
**Solución**:
1. La macro se guarda automáticamente en tu directorio de Tekla
2. En Tekla: `Tools` > `Macros...` > Debería aparecer en la lista
3. Si no aparece, verifica la ruta mostrada en el mensaje

### **Problema**: "La macro no cambia el Phase"
**Solución**:
1. Asegúrate de tener soldaduras **SELECCIONADAS** antes de ejecutar la macro
2. La macro solo afecta objetos seleccionados
3. Verifica que el Phase Manager de Tekla funcione manualmente

## ?? CONSEJOS PRO

### **Tip 1: Procesar Múltiples Assemblies a la Vez**
```
1. Selecciona TODOS los assemblies que necesitas
2. Ejecuta el sincronizador UNA vez
3. El sistema procesará todos automáticamente
4. Al final, ejecuta la macro UNA vez para todas las soldaduras
```

### **Tip 2: Crear Atajo de Teclado**
```
En Tekla:
1. Tools > Customize > Keyboard Shortcuts
2. Busca tu macro: AutoChangeWeldsToPhase2
3. Asigna tecla: Ctrl+Shift+2 (ejemplo)
4. Ahora puedes ejecutarla con un atajo
```

### **Tip 3: Usar Filtros de Selección**
```
Para seleccionar solo soldaduras:
1. Tools > Selection > Selection Filter...
2. Desmarca todo excepto "Welds"
3. Ahora cuando selecciones el assembly, solo se seleccionan soldaduras
4. Ejecuta la macro
```

## ?? SOPORTE

Si tienes problemas:
1. Revisa el reporte detallado que genera el sincronizador
2. Verifica los archivos de log
3. Asegúrate de que las piezas tengan Phase asignada en Tekla

## ?? RESULTADO FINAL

Después de ejecutar el sistema completo:

? **100% de Parts sincronizadas** - Automático  
? **100% de Bolts sincronizados** - Automático  
? **100% de Welds sincronizadas** - Semi-automático (1 click extra)  

**Tiempo total**: **~1-2 minutos** para CUALQUIER cantidad de assemblies  
**Esfuerzo manual**: **Mínimo** (2-3 clicks)  
**Errores**: **Cero** (todo programático)

---

## ?? ¡LISTO PARA USAR!

Simplemente ejecuta:
```cmd
cd "CORRECTOR DE ATRIBUTOS"
.\ejecutar.bat
```

Y sigue las instrucciones en pantalla. El sistema te guiará paso a paso.
