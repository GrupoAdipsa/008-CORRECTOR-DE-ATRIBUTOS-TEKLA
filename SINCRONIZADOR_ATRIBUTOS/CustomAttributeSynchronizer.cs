using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace SincronizadorAtributos
{
    /// <summary>
    /// Motor de sincronización de atributos personalizados (ESTATUS_PIEZA y PRIORIDAD).
    /// 
    /// Funcionalidad:
    /// - Lee los valores de ESTATUS_PIEZA y PRIORIDAD de la Main Part de cada Assembly
    /// - Propaga estos valores a todas las Secondary Parts del Assembly
    /// - Propaga estos valores a todos los Bolts del Assembly
    /// - Genera reportes detallados de todos los cambios
    /// 
    /// Atributos sincronizados:
    /// - ESTATUS_PIEZA: Estatus de la pieza (Programado, Conectado, Detallado, Revisado, Liberado)
    /// - PRIORIDAD: Prioridad de detallado (string)
    /// </summary>
    public class CustomAttributeSynchronizer
    {
        private Model _model;
        private CustomAttributeReport _report;

        public CustomAttributeSynchronizer()
        {
            _model = new Model();
            _report = new CustomAttributeReport();
        }

        /// <summary>
        /// Ejecuta la sincronización en modo interactivo.
        /// Permite al usuario seleccionar qué objetos sincronizar.
        /// </summary>
        public bool ExecuteInteractive()
        {
            try
            {
                if (!_model.GetConnectionStatus())
                {
                    MessageBox.Show(
                        "No hay conexión con Tekla Structures.\n\n" +
                        "Por favor:\n" +
                        "1. Abre Tekla Structures\n" +
                        "2. Abre o crea un modelo\n" +
                        "3. Ejecuta esta aplicación nuevamente",
                        "Error de Conexión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                // Obtener selección del usuario
                Picker picker = new Picker();
                ModelObjectEnumerator selectedObjects = null;
                
                try
                {
                    selectedObjects = picker.PickObjects(
                        Picker.PickObjectsEnum.PICK_N_OBJECTS,
                        "Selecciona Parts o Assemblies para sincronizar ESTATUS_PIEZA y PRIORIDAD"
                    );
                }
                catch (Exception pickEx)
                {
                    // Usuario canceló la selección (ESC o cerró diálogo)
                    if (pickEx.Message.Contains("interrupt") || pickEx.Message.Contains("cancel"))
                    {
                        MessageBox.Show(
                            "Selección cancelada por el usuario.",
                            "Operación Cancelada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return false;
                    }
                    // Otro error durante la selección
                    throw;
                }

                if (selectedObjects == null || selectedObjects.GetSize() == 0)
                {
                    MessageBox.Show(
                        "No se seleccionaron objetos.\n\n" +
                        "Operación cancelada.",
                        "Sin Selección",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }

                return ExecuteOnSelection(selectedObjects);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error durante la sincronización:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Ejecuta la sincronización sobre todos los objetos del modelo.
        /// </summary>
        public bool ExecuteOnAllModel()
        {
            try
            {
                if (!_model.GetConnectionStatus())
                {
                    MessageBox.Show(
                        "No hay conexión con Tekla Structures.",
                        "Error de Conexión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                // Obtener todos los Assemblies del modelo
                ModelObjectEnumerator allParts = _model.GetModelObjectSelector().GetAllObjectsWithType(
                    ModelObject.ModelObjectEnum.BEAM);

                if (allParts == null || allParts.GetSize() == 0)
                {
                    MessageBox.Show(
                        "No se encontraron piezas en el modelo.",
                        "Modelo Vacío",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }

                return ExecuteOnSelection(allParts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error durante la sincronización:\n\n{ex.Message}",
                    "Error Crítico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Ejecuta la sincronización sobre una selección específica de objetos.
        /// </summary>
        private bool ExecuteOnSelection(ModelObjectEnumerator selectedObjects)
        {
            _report = new CustomAttributeReport();

            // Agrupar objetos por Assembly
            Dictionary<Assembly, List<ModelObject>> groupedByAssembly = GroupByAssembly(selectedObjects);

            if (groupedByAssembly.Count == 0)
            {
                MessageBox.Show(
                    "No se encontraron Assemblies válidos en la selección.",
                    "Sin Assemblies",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            // Procesar cada Assembly
            foreach (var kvp in groupedByAssembly)
            {
                Assembly assembly = kvp.Key;
                SynchronizeAssembly(assembly);
            }

            // Guardar cambios
            try
            {
                _model.CommitChanges();
                _report.AddInfo("Cambios guardados en el modelo");
            }
            catch (Exception ex)
            {
                _report.AddError($"Error al guardar cambios: {ex.Message}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Agrupa los objetos seleccionados por su Assembly padre.
        /// </summary>
        private Dictionary<Assembly, List<ModelObject>> GroupByAssembly(ModelObjectEnumerator selectedObjects)
        {
            Dictionary<Assembly, List<ModelObject>> grouped = new Dictionary<Assembly, List<ModelObject>>();

            selectedObjects.SelectInstances = false;
            while (selectedObjects.MoveNext())
            {
                ModelObject obj = selectedObjects.Current as ModelObject;
                if (obj == null) continue;

                Assembly assembly = null;

                if (obj is Part)
                {
                    Part part = obj as Part;
                    assembly = part.GetAssembly();
                }
                else if (obj is BoltGroup)
                {
                    BoltGroup bolt = obj as BoltGroup;
                    Part partParent = bolt.PartToBoltTo as Part;
                    if (partParent != null)
                    {
                        assembly = partParent.GetAssembly();
                    }
                }
                else if (obj is Assembly)
                {
                    assembly = obj as Assembly;
                }

                if (assembly != null && assembly.Identifier.ID > 0)
                {
                    if (!grouped.ContainsKey(assembly))
                    {
                        grouped[assembly] = new List<ModelObject>();
                    }
                    grouped[assembly].Add(obj);
                }
            }

            return grouped;
        }

        /// <summary>
        /// Sincroniza un Assembly completo.
        /// LÓGICA INTELIGENTE:
        /// 1. Si Main Part tiene atributos ? úsalos
        /// 2. Si Main Part NO tiene atributos ? busca en Secondary Parts
        /// 3. Si encuentra en Secondary ? primero copia a Main Part, luego sincroniza todo
        /// </summary>
        private void SynchronizeAssembly(Assembly assembly)
        {
            try
            {
                // Obtener Main Part
                Part mainPart = assembly.GetMainPart() as Part;
                if (mainPart == null)
                {
                    _report.AddWarning($"Assembly {assembly.Identifier.ID}: No tiene Main Part");
                    return;
                }

                // PASO 1: Leer atributos de la Main Part
                string estatusPieza = ReadEstatusPieza(mainPart);
                string prioridad = ReadPrioridad(mainPart);

                _report.AddInfo($"Assembly {assembly.Identifier.ID} - Main Part {mainPart.Identifier.ID}:");
                _report.AddInfo($"  ESTATUS_PIEZA leído: '{estatusPieza}' (vacío={string.IsNullOrEmpty(estatusPieza)})");
                _report.AddInfo($"  PRIORIDAD leída: '{prioridad}' (vacío={string.IsNullOrEmpty(prioridad)})");

                // PASO 2: Si Main Part NO tiene atributos, buscar en Secondary Parts
                if (string.IsNullOrEmpty(estatusPieza) && string.IsNullOrEmpty(prioridad))
                {
                    _report.AddInfo("  Main Part sin atributos - Buscando en Secondary Parts...");
                    
                    ArrayList secondaries = assembly.GetSecondaries();
                    if (secondaries != null && secondaries.Count > 0)
                    {
                        foreach (Part part in secondaries)
                        {
                            if (part == null) continue;

                            string tempEstatus = ReadEstatusPieza(part);
                            string tempPrioridad = ReadPrioridad(part);

                            if (!string.IsNullOrEmpty(tempEstatus) || !string.IsNullOrEmpty(tempPrioridad))
                            {
                                // ¡Encontramos atributos en una Secondary Part!
                                estatusPieza = tempEstatus;
                                prioridad = tempPrioridad;
                                
                                _report.AddInfo($"  Atributos encontrados en Secondary Part {part.Identifier.ID}:");
                                _report.AddInfo($"    ESTATUS_PIEZA: '{estatusPieza}'");
                                _report.AddInfo($"    PRIORIDAD: '{prioridad}'");
                                
                                // PASO 3: Copiar atributos a la Main Part PRIMERO
                                _report.AddInfo($"  Copiando atributos a Main Part {mainPart.Identifier.ID}...");
                                
                                bool mainPartUpdated = false;
                                
                                if (!string.IsNullOrEmpty(estatusPieza))
                                {
                                    WriteEstatusPieza(mainPart, estatusPieza);
                                    mainPartUpdated = true;
                                }
                                
                                if (!string.IsNullOrEmpty(prioridad))
                                {
                                    WritePrioridad(mainPart, prioridad);
                                    mainPartUpdated = true;
                                }
                                
                                if (mainPartUpdated)
                                {
                                    mainPart.Modify();
                                    _report.AddInfo($"  Main Part actualizada con atributos de Secondary Part {part.Identifier.ID}");
                                }
                                
                                break; // Usar la primera parte con atributos
                            }
                        }
                    }
                }

                // PASO 4: Verificar si ahora tenemos atributos
                if (string.IsNullOrEmpty(estatusPieza) && string.IsNullOrEmpty(prioridad))
                {
                    _report.AddWarning($"Assembly {assembly.Identifier.ID}: Ninguna parte tiene ESTATUS_PIEZA ni PRIORIDAD - Se omitirá sincronización");
                    return;
                }

                _report.AddAssemblyProcessed(assembly.Identifier.ID);

                // PASO 5: Sincronizar Secondary Parts desde Main Part
                SyncSecondaryParts(assembly, estatusPieza, prioridad);

                // PASO 6: Sincronizar Bolts
                SyncBolts(mainPart, estatusPieza, prioridad);
                
                _report.AddInfo($"Assembly {assembly.Identifier.ID}: Sincronización completada exitosamente");
            }
            catch (Exception ex)
            {
                _report.AddError($"Assembly {assembly.Identifier.ID}: {ex.Message}");
            }
        }

        /// <summary>
        /// Sincroniza las Secondary Parts de un Assembly.
        /// </summary>
        private void SyncSecondaryParts(Assembly assembly, string estatusPieza, string prioridad)
        {
            ArrayList secondaries = assembly.GetSecondaries();
            if (secondaries == null || secondaries.Count == 0) return;

            foreach (Part part in secondaries)
            {
                if (part == null) continue;

                _report.PartsEvaluated++;

                try
                {
                    // LEER VALORES ANTES DE MODIFICAR
                    string estatusAntes = ReadEstatusPieza(part);
                    string prioridadAntes = ReadPrioridad(part);

                    bool changed = false;

                    // Aplicar ESTATUS_PIEZA si tiene valor
                    if (!string.IsNullOrEmpty(estatusPieza))
                    {
                        WriteEstatusPieza(part, estatusPieza);
                        changed = true;
                    }

                    // Aplicar PRIORIDAD si tiene valor
                    if (!string.IsNullOrEmpty(prioridad))
                    {
                        WritePrioridad(part, prioridad);
                        changed = true;
                    }

                    if (changed)
                    {
                        part.Modify();
                        _report.PartsChanged++;
                        
                        // LEER VALORES DESPUÉS DE MODIFICAR
                        string estatusDespues = ReadEstatusPieza(part);
                        string prioridadDespues = ReadPrioridad(part);
                        
                        // REPORTE DETALLADO ANTES/DESPUÉS
                        _report.AddInfo($"    ??? Part {part.Identifier.ID} ???");
                        _report.AddInfo($"    ANTES:");
                        _report.AddInfo($"      ESTATUS_PIEZA: '{estatusAntes}' {(string.IsNullOrEmpty(estatusAntes) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"      PRIORIDAD:     '{prioridadAntes}' {(string.IsNullOrEmpty(prioridadAntes) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"    DESPUÉS:");
                        _report.AddInfo($"      ESTATUS_PIEZA: '{estatusDespues}' {(string.IsNullOrEmpty(estatusDespues) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"      PRIORIDAD:     '{prioridadDespues}' {(string.IsNullOrEmpty(prioridadDespues) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"    CAMBIO: {(estatusAntes != estatusDespues || prioridadAntes != prioridadDespues ? "? MODIFICADO" : "? SIN CAMBIO REAL")}");
                    }
                    else
                    {
                        _report.PartsSkipped++;
                    }
                }
                catch (Exception ex)
                {
                    _report.AddWarning($"Part {part.Identifier.ID}: Error al modificar - {ex.Message}");
                    _report.PartsSkipped++;
                }
            }
        }

        /// <summary>
        /// Sincroniza los Bolts de una Part.
        /// </summary>
        private void SyncBolts(Part mainPart, string estatusPieza, string prioridad)
        {
            ModelObjectEnumerator bolts = mainPart.GetBolts();
            if (bolts == null) return;

            while (bolts.MoveNext())
            {
                BoltGroup bolt = bolts.Current as BoltGroup;
                if (bolt == null) continue;

                _report.BoltsEvaluated++;

                try
                {
                    // LEER VALORES ANTES DE MODIFICAR
                    string estatusAntes = ReadEstatusPieza(bolt);
                    string prioridadAntes = ReadPrioridad(bolt);

                    bool changed = false;

                    // Aplicar ESTATUS_PIEZA si tiene valor
                    if (!string.IsNullOrEmpty(estatusPieza))
                    {
                        WriteEstatusPieza(bolt, estatusPieza);
                        changed = true;
                    }

                    // Aplicar PRIORIDAD si tiene valor
                    if (!string.IsNullOrEmpty(prioridad))
                    {
                        WritePrioridad(bolt, prioridad);
                        changed = true;
                    }

                    if (changed)
                    {
                        bolt.Modify();
                        _report.BoltsChanged++;
                        
                        // LEER VALORES DESPUÉS DE MODIFICAR
                        string estatusDespues = ReadEstatusPieza(bolt);
                        string prioridadDespues = ReadPrioridad(bolt);
                        
                        // REPORTE DETALLADO ANTES/DESPUÉS
                        _report.AddInfo($"    ??? Bolt {bolt.Identifier.ID} ???");
                        _report.AddInfo($"    ANTES:");
                        _report.AddInfo($"      ESTATUS_PIEZA: '{estatusAntes}' {(string.IsNullOrEmpty(estatusAntes) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"      PRIORIDAD:     '{prioridadAntes}' {(string.IsNullOrEmpty(prioridadAntes) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"    DESPUÉS:");
                        _report.AddInfo($"      ESTATUS_PIEZA: '{estatusDespues}' {(string.IsNullOrEmpty(estatusDespues) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"      PRIORIDAD:     '{prioridadDespues}' {(string.IsNullOrEmpty(prioridadDespues) ? "[VACÍO]" : "[OK]")}");
                        _report.AddInfo($"    CAMBIO: {(estatusAntes != estatusDespues || prioridadAntes != prioridadDespues ? "? MODIFICADO" : "? SIN CAMBIO REAL")}");
                    }
                    else
                    {
                        _report.BoltsSkipped++;
                    }
                }
                catch (Exception ex)
                {
                    _report.AddWarning($"Bolt {bolt.Identifier.ID}: Error al modificar - {ex.Message}");
                    _report.BoltsSkipped++;
                }
            }
        }

        // ============================================================================
        // MÉTODOS DE LECTURA DE ATRIBUTOS
        // ============================================================================

        /// <summary>
        /// Lee el valor de ESTATUS_PIEZA de un objeto.
        /// Intenta múltiples métodos para asegurar compatibilidad.
        /// ORDEN IMPORTANTE: ReportProperty primero (es más confiable en Tekla 2021)
        /// </summary>
        private string ReadEstatusPieza(ModelObject obj)
        {
            string value = "";

            // Método 1: GetReportProperty (MÁS CONFIABLE EN TEKLA 2021)
            try
            {
                if (obj.GetReportProperty("ESTATUS_PIEZA", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 2: GetUserProperty con nombre exacto
            try
            {
                value = "";
                if (obj.GetUserProperty("ESTATUS_PIEZA", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 3: Nombre con formato de label
            try
            {
                value = "";
                if (obj.GetUserProperty("Estatus de Pieza:", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 4: Sin dos puntos
            try
            {
                value = "";
                if (obj.GetUserProperty("Estatus de Pieza", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 5: Todo mayúsculas
            try
            {
                value = "";
                if (obj.GetUserProperty("ESTATUS DE PIEZA", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            return "";
        }

        /// <summary>
        /// Lee el valor de PRIORIDAD de un objeto.
        /// ORDEN IMPORTANTE: ReportProperty primero (es más confiable en Tekla 2021)
        /// </summary>
        private string ReadPrioridad(ModelObject obj)
        {
            string value = "";

            // Método 1: GetReportProperty (MÁS CONFIABLE EN TEKLA 2021)
            try
            {
                if (obj.GetReportProperty("PRIORIDAD", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 2: GetUserProperty
            try
            {
                value = "";
                if (obj.GetUserProperty("PRIORIDAD", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 3: Nombre completo con dos puntos
            try
            {
                value = "";
                if (obj.GetUserProperty("PRIORIDAD DETALLADO:", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 4: Sin dos puntos
            try
            {
                value = "";
                if (obj.GetUserProperty("PRIORIDAD DETALLADO", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 5: Minúsculas
            try
            {
                value = "";
                if (obj.GetUserProperty("prioridad", ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            return "";
        }

        // ============================================================================
        // MÉTODOS DE ESCRITURA DE ATRIBUTOS
        // ============================================================================

        /// <summary>
        /// Escribe el valor de ESTATUS_PIEZA en un objeto.
        /// SOLUCIÓN FINAL: Convertir el string a índice numérico y usar SetUserProperty con int
        /// Basado en la macro: akit.ValueChange("ESTATUS_PIEZA", "3") donde 3 es el índice
        /// </summary>
        private void WriteEstatusPieza(ModelObject obj, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            try
            {
                // Leer el valor actual
                string currentValue = ReadEstatusPieza(obj);
                
                // Si ya tiene el mismo valor, no hacer nada
                if (currentValue == value)
                {
                    return;
                }

                // Convertir string a índice según la definición del atributo
                int index = GetEstatusPiezaIndex(value);
                
                if (index >= 0)
                {
                    // Usar SetUserProperty con el índice numérico
                    obj.SetUserProperty("ESTATUS_PIEZA", index);
                }
                else
                {
                    // Fallback: usar string directamente
                    obj.SetUserProperty("ESTATUS_PIEZA", value);
                }
            }
            catch (Exception ex)
            {
                _report.AddWarning($"No se pudo escribir ESTATUS_PIEZA en objeto {obj.Identifier.ID}: {ex.Message}");
            }
        }

        /// <summary>
        /// Convierte el valor string de ESTATUS_PIEZA a su índice numérico.
        /// Basado en la definición del atributo Option en el archivo .inp
        /// </summary>
        private int GetEstatusPiezaIndex(string value)
        {
            // Mapeo según el orden real del archivo .inp
            switch (value?.Trim())
            {
                case "Programado":
                    return 0;
                case "Conectado":
                    return 1;
                case "Detallado":
                    return 2;
                case "Revisado":
                    return 3;
                case "Liberado":
                    return 4;
                case "":
                case null:
                    return 0; // vacío = Programado
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Escribe el valor de PRIORIDAD en un objeto.
        /// PRIORIDAD es tipo string, no Option, así que usar SetUserProperty directamente
        /// </summary>
        private void WritePrioridad(ModelObject obj, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            try
            {
                // Leer el valor actual
                string currentValue = ReadPrioridad(obj);
                
                // Si ya tiene el mismo valor, no hacer nada
                if (currentValue == value)
                {
                    return;
                }

                // Para string, usar directamente
                obj.SetUserProperty("PRIORIDAD", value);
            }
            catch (Exception ex)
            {
                _report.AddWarning($"No se pudo escribir PRIORIDAD en objeto {obj.Identifier.ID}: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el reporte de la sincronización.
        /// </summary>
        public CustomAttributeReport GetReport()
        {
            return _report;
        }
    }
}
