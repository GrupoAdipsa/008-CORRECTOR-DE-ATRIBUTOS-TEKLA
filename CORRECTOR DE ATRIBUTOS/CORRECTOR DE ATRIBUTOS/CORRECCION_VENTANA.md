# Corrección: Ventana No Debe Tapar Tekla

## Problema Identificado

La ventana del sincronizador se maximizaba completamente cuando Tekla estaba maximizada, tapando completamente la ventana de Tekla y haciendo imposible trabajar con ambas aplicaciones simultáneamente.

## Solución Implementada

Se realizaron 2 cambios principales:

### 1. Desactivar Sincronización de Maximizado

**Antes:**
```csharp
// Si Tekla se maximiza -> Esta ventana se maximiza
if (IsZoomed(_teklaHandle))
{
    if (this.WindowState != FormWindowState.Maximized)
    {
        this.WindowState = FormWindowState.Maximized;
    }
}
```

**Ahora:**
```csharp
// Si Tekla NO está minimizada -> Solo restaurar si esta ventana está minimizada
// NO sincronizar maximizado para no tapar Tekla
if (this.WindowState == FormWindowState.Minimized)
{
    this.WindowState = FormWindowState.Normal;
}
```

### 2. Posicionamiento Inteligente

**Antes:**
```csharp
this.StartPosition = FormStartPosition.CenterScreen;
this.Size = new System.Drawing.Size(800, 600);
```

**Ahora:**
```csharp
// Posicionar en esquina superior derecha
this.StartPosition = FormStartPosition.Manual;
this.Size = new System.Drawing.Size(850, 650);

int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
this.Location = new Point(
    screenWidth - this.Width - 20,  // Esquina derecha
    20                               // Margen superior
);
```

## Comportamiento Actual

### ? Lo que SÍ hace:

1. **Siempre al frente**: La ventana permanece siempre visible
2. **Minimiza con Tekla**: Si minimizas Tekla, también se minimiza el sincronizador
3. **Restaura con Tekla**: Si restauras Tekla, también se restaura el sincronizador
4. **Posición fija**: Se abre en la esquina superior derecha

### ? Lo que NO hace:

1. **NO se maximiza automáticamente**: El usuario decide si quiere maximizar
2. **NO tapa a Tekla**: Deja espacio para ver y trabajar con Tekla
3. **NO cambia de tamaño solo**: Mantiene el tamaño que el usuario configure

## Layout de Pantalla

```
+----------------------------------------------------------+
| Pantalla Completa                                        |
|                                                          |
|  +---------------------------------+  +--------------+   |
|  |                                 |  | Sincronizador|   |
|  |                                 |  | (850x650)    |   |
|  |                                 |  |              |   |
|  |         TEKLA                   |  |  Siempre     |   |
|  |         (Área de trabajo)       |  |  Visible     |   |
|  |                                 |  |              |   |
|  |                                 |  |              |   |
|  |                                 |  |              |   |
|  |                                 |  +--------------+   |
|  +---------------------------------+                     |
|                                                          |
+----------------------------------------------------------+
```

## Ventajas del Nuevo Comportamiento

### 1. Productividad
- Puedes ver ambas aplicaciones simultáneamente
- No necesitas alternar entre ventanas
- Flujo de trabajo más eficiente

### 2. Flexibilidad
- El usuario puede maximizar manualmente si lo desea
- Se puede redimensionar y reposicionar libremente
- Se adapta a diferentes tamaños de pantalla

### 3. Consistencia
- Comportamiento predecible
- No sorprende al usuario con cambios de tamaño
- Se mantiene siempre visible pero no invasivo

## Casos de Uso

### Caso 1: Trabajo Normal
```
1. Abrir Tekla (ventana normal)
2. Ejecutar sincronizador
3. Aparece en esquina derecha
4. Ambas ventanas visibles
5. Trabajar con ambas simultáneamente
```

### Caso 2: Tekla Maximizada
```
1. Tekla maximizada (ocupa toda la pantalla)
2. Ejecutar sincronizador
3. Aparece en esquina derecha SOBRE Tekla
4. Sincronizador siempre al frente
5. Puedes minimizar Tekla para ver mejor el reporte
```

### Caso 3: Minimizar Todo
```
1. Minimizar Tekla
2. El sincronizador se minimiza automáticamente
3. Restaurar Tekla
4. El sincronizador se restaura automáticamente
```

## Configuración Recomendada

### Para Pantallas Pequeñas (<= 1920x1080):
- Dejar el tamaño por defecto (850x650)
- Posición: Esquina superior derecha
- Tekla en modo ventana (no maximizada)

### Para Pantallas Grandes (> 1920x1080):
- Puedes aumentar el tamaño del sincronizador
- O tener ambas ventanas lado a lado
- Aprovechar el espacio adicional

### Para Múltiples Monitores:
- Tekla en monitor principal
- Sincronizador en monitor secundario
- Mover manualmente según preferencia

## Código Relevante

### WindowSyncTimer_Tick() - Lógica de Sincronización

```csharp
private void WindowSyncTimer_Tick(object sender, EventArgs e)
{
    if (_teklaHandle == IntPtr.Zero)
    {
        FindTeklaWindow();
        return;
    }
    
    try
    {
        // Solo sincronizar minimizado
        if (IsIconic(_teklaHandle))
        {
            // Tekla minimizada -> Minimizar sincronizador
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }
        else
        {
            // Tekla visible -> Asegurar que sincronizador está visible
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            
            // NO cambiar a maximizado automáticamente
        }
        
        // Siempre al frente
        if (!this.TopMost)
        {
            this.TopMost = true;
        }
    }
    catch
    {
        _teklaHandle = IntPtr.Zero;
    }
}
```

### InitializeComponent() - Posicionamiento

```csharp
private void InitializeComponent()
{
    this.Text = "Sincronización de Phase - Piezas v2.1";
    this.Size = new Size(850, 650);
    this.StartPosition = FormStartPosition.Manual;
    
    // Calcular posición en esquina superior derecha
    int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
    int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
    
    this.Location = new Point(
        screenWidth - this.Width - 20,  // 20px desde borde derecho
        20                               // 20px desde borde superior
    );
    
    // Permitir redimensionar pero no forzar maximizado
    this.FormBorderStyle = FormBorderStyle.Sizable;
    this.MaximizeBox = true;  // Usuario puede maximizar si quiere
    this.MinimizeBox = true;
}
```

## Testing

### Pruebas Realizadas:
- [x] Ventana aparece en esquina superior derecha
- [x] NO se maximiza automáticamente
- [x] Tekla queda visible
- [x] Se minimiza cuando Tekla se minimiza
- [x] Se restaura cuando Tekla se restaura
- [x] Siempre permanece al frente
- [x] Usuario puede maximizar manualmente

### Pruebas Pendientes:
- [ ] Probar en pantallas de diferentes resoluciones
- [ ] Probar con múltiples monitores
- [ ] Verificar comportamiento con Tekla en diferentes tamaños

## Comparación de Comportamiento

| Acción | Antes (v2.1 original) | Ahora (v2.1 corregida) |
|--------|----------------------|------------------------|
| Abrir ventana | Centro de pantalla | Esquina superior derecha |
| Tekla maximizada | Se maximiza también | Permanece en esquina |
| Tekla normal | Se restaura | Permanece igual |
| Tekla minimizada | Se minimiza | Se minimiza |
| Usuario maximiza | Tapa Tekla | Tapa Tekla (decisión del usuario) |
| Visibilidad | Tapa completamente | Siempre visible parcialmente |

## Notas Importantes

### ¿Por qué NO sincronizar maximizado?

1. **Usabilidad**: Necesitas ver ambas aplicaciones
2. **Flujo de trabajo**: El reporte es de consulta, no de edición
3. **Ergonomía**: Es más cómodo tener el sincronizador en un lado

### ¿Cuándo maximizar?

El usuario puede maximizar manualmente cuando:
- Necesita leer un reporte largo con detalle
- Está revisando muchos errores o advertencias
- Prefiere foco completo en los resultados

### Alternativas Consideradas

**Opción 1: Ventana flotante pequeña (Descartada)**
- Muy pequeña para leer reportes
- Difícil de redimensionar

**Opción 2: Panel lateral en Tekla (No posible)**
- Tekla no permite plugins con UI integrada de esta manera
- Requeriría desarrollo de plugin nativo

**Opción 3: Sincronizar tamaño exacto (Compleja)**
- Difícil de implementar correctamente
- Comportamiento impredecible
- No aporta valor significativo

**Opción 4: Posición fija en esquina (ELEGIDA)**
- Simple y efectiva
- Predecible
- Flexible para el usuario

## Conclusión

La ventana ahora se comporta de manera más intuitiva y útil:
- ? Siempre visible (TopMost)
- ? NO tapa Tekla completamente
- ? Se minimiza/restaura con Tekla
- ? Posición fija y predecible
- ? Usuario controla el tamaño

Este cambio mejora significativamente la experiencia de usuario sin sacrificar funcionalidad.

---

**Versión**: 2.1.1
**Fecha**: 2024
**Estado**: Corregido y Compilado
