using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace CORRECTOR_DE_ATRIBUTOS
{
    /// <summary>
    /// Sincroniza la Phase de un Assembly completo basándose en su Main Part.
    /// </summary>
    public class PhaseSynchronizer
    {
        private Model _model;
        private SyncReport _report;
        private const string PHASE_PROPERTY = "PHASE_NUMBER";  // Restaurado al valor original que funcionaba

        public PhaseSynchronizer()
        {
            _model = new Model();
            _report = new SyncReport();
        }

        /// <summary>
        /// Obtiene el reporte actual de sincronización.
        /// </summary>
        public SyncReport Report
        {
            get { return _report; }
        }

        /// <summary>
        /// Punto de entrada principal: selección interactiva y sincronización.
        /// </summary>
        public bool ExecuteInteractive()
        {
            if (!_model.GetConnectionStatus())
            {
                _report.AddError("No hay conexión con Tekla Structures.");
                return false;
            }

            try
            {
                // Paso 1: Selección interactiva
                Picker picker = new Picker();
                ModelObjectEnumerator selectedObjects = picker.PickObjects(
                    Picker.PickObjectsEnum.PICK_N_OBJECTS,
                    "Selecciona piezas o assemblies para sincronizar Phase"
                );

                if (selectedObjects == null || selectedObjects.GetSize() == 0)
                {
                    _report.AddWarning("No se seleccionaron objetos o se canceló la selección.");
                    return false;
                }

                return ExecuteOnSelection(selectedObjects);
            }
            catch (Exception ex)
            {
                // Verificar si es cancelación de usuario
                if (ex.Message.Contains("User interrupt") || ex.Message.Contains("user interrupt"))
                {
                    _report.AddWarning("Selección cancelada por el usuario.");
                    return false;
                }
                
                _report.AddError($"Error crítico: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta la sincronización en una selección ya obtenida.
        /// </summary>
        public bool ExecuteOnSelection(ModelObjectEnumerator selectedObjects)
        {
            try
            {
                // Paso 2: Agrupar por Assembly
                Dictionary<Assembly, List<ModelObject>> assemblyGroups = 
                    GroupByAssembly(selectedObjects);

                if (assemblyGroups.Count == 0)
                {
                    _report.AddWarning("No se pudieron identificar Assemblies válidos.");
                    return false;
                }

                // Paso 3: Procesar cada Assembly
                foreach (var kvp in assemblyGroups)
                {
                    SynchronizeAssembly(kvp.Key);
                }

                // Paso 4: Commit único al final (performance crítica)
                _model.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                _report.AddError($"Error en ExecuteOnSelection: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Agrupa objetos seleccionados por su Assembly.
        /// </summary>
        private Dictionary<Assembly, List<ModelObject>> GroupByAssembly(
            ModelObjectEnumerator selectedObjects)
        {
            var groups = new Dictionary<Assembly, List<ModelObject>>();

            while (selectedObjects.MoveNext())
            {
                ModelObject obj = selectedObjects.Current as ModelObject;
                if (obj == null) continue;

                Assembly assembly = ResolveAssembly(obj);
                if (assembly == null) continue;

                if (!groups.ContainsKey(assembly))
                {
                    groups[assembly] = new List<ModelObject>();
                }
                groups[assembly].Add(obj);
            }

            return groups;
        }

        /// <summary>
        /// Resuelve el Assembly de un objeto seleccionado.
        /// </summary>
        private Assembly ResolveAssembly(ModelObject obj)
        {
            // Caso 1: el objeto es directamente un Assembly
            Assembly assembly = obj as Assembly;
            if (assembly != null) return assembly;

            // Caso 2: el objeto es una Part
            Part part = obj as Part;
            if (part != null)
            {
                assembly = part.GetAssembly();
                return assembly;
            }

            // Caso 3: BoltGroup - obtener por parte primaria
            BoltGroup bolt = obj as BoltGroup;
            if (bolt != null)
            {
                Part primaryPart = bolt.PartToBoltTo as Part;
                if (primaryPart != null)
                {
                    return primaryPart.GetAssembly();
                }
            }

            // NOTA: Soldaduras se manejan con la macro de Tekla, no en este código

            return null;
        }

        /// <summary>
        /// Sincroniza Phase de un Assembly completo.
        /// </summary>
        private void SynchronizeAssembly(Assembly assembly)
        {
            try
            {
                // Paso 1: Obtener Main Part
                Part mainPart = assembly.GetMainPart() as Part;
                if (mainPart == null)
                {
                    _report.AddError($"Assembly {assembly.Identifier.ID}: " +
                        "No se pudo obtener Main Part");
                    return;
                }

                // Paso 2: Leer Phase de referencia desde la Main Part
                // NOTA: Intentar múltiples métodos porque diferentes versiones de Tekla
                // usan diferentes propiedades
                int phaseNumber = 0;
                bool phaseFound = false;
                
                // Método 1: Intentar GetReportProperty con "PHASE"
                if (mainPart.GetReportProperty("PHASE", ref phaseNumber) && phaseNumber > 0)
                {
                    phaseFound = true;
                }
                
                // Método 2: Intentar GetUserProperty con "PHASE_NUMBER"
                if (!phaseFound)
                {
                    if (mainPart.GetUserProperty("PHASE_NUMBER", ref phaseNumber) && phaseNumber > 0)
                    {
                        phaseFound = true;
                    }
                }
                
                // Método 3: Intentar GetPhase() directamente si existe
                if (!phaseFound)
                {
                    try
                    {
                        Phase phase;
                        if (mainPart.GetPhase(out phase) && phase != null)
                        {
                            phaseNumber = phase.PhaseNumber;
                            if (phaseNumber > 0)
                            {
                                phaseFound = true;
                            }
                        }
                    }
                    catch
                    {
                        // GetPhase() puede no existir en todas las versiones
                    }
                }
                
                if (!phaseFound || phaseNumber == 0)
                {
                    _report.AddWarning($"Assembly {assembly.Identifier.ID} (Main Part: {mainPart.Identifier.ID}): " +
                        "La Main Part no tiene Phase asignada. " +
                        "Por favor asigna un valor de Phase en las propiedades de la pieza en Tekla.");
                    return;
                }

                _report.AddAssemblyProcessed(assembly.Identifier.ID);

                // Paso 3: Sincronizar Secondary Parts
                SyncSecondaryParts(assembly, phaseNumber);

                // Paso 4: Sincronizar Bolts (obtener del modelo)
                SyncBolts(mainPart, phaseNumber);

                // NOTA: Soldaduras se sincronizan con la macro de Tekla (SyncWeldPhaseFromParts)
            }
            catch (Exception ex)
            {
                _report.AddError($"Assembly {assembly.Identifier.ID}: {ex.Message}");
            }
        }

        /// <summary>
        /// Sincroniza Phase de Secondary Parts.
        /// </summary>
        private void SyncSecondaryParts(Assembly assembly, int targetPhase)
        {
            try
            {
                ArrayList secondaries = assembly.GetSecondaries();
                
                foreach (Part part in secondaries)
                {
                    if (part == null) continue;

                    _report.PartsEvaluated++;

                    try
                    {
                        // SIEMPRE intentar cambiar el Phase
                        // Intentar múltiples métodos para ESCRIBIR el Phase
                        bool success = false;
                        
                        // Método 1: Usar SetPhase() con objeto Phase
                        try
                        {
                            Phase phaseObj;
                            if (part.GetPhase(out phaseObj))
                            {
                                if (phaseObj == null)
                                {
                                    phaseObj = new Phase();
                                }
                                phaseObj.PhaseNumber = targetPhase;
                                part.SetPhase(phaseObj);
                                success = true;
                            }
                        }
                        catch
                        {
                            // SetPhase() puede no funcionar en esta versión
                        }
                        
                        // Método 2: Usar SetUserProperty con "PHASE_NUMBER"
                        if (!success)
                        {
                            try
                            {
                                part.SetUserProperty("PHASE_NUMBER", targetPhase);
                                success = true;
                            }
                            catch
                            {
                                // Puede no funcionar
                            }
                        }
                        
                        // Método 3: Intentar con AssemblyNumber.Prefix modificado (workaround)
                        if (!success)
                        {
                            try
                            {
                                part.SetUserProperty("PHASE", targetPhase);
                                success = true;
                            }
                            catch
                            {
                                // Última opción
                            }
                        }
                        
                        // Intentar modificar
                        bool modified = part.Modify();

                        if (modified && success)
                        {
                            _report.PartsChanged++;
                        }
                        else
                        {
                            _report.PartsSkipped++;
                            _report.AddWarning($"Part {part.Identifier.ID}: " +
                                "Modify() devolvió false o no se pudo aplicar Phase");
                        }
                    }
                    catch (Exception ex)
                    {
                        _report.PartsSkipped++;
                        _report.AddError($"Part {part.Identifier.ID}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _report.AddError($"Error obteniendo secondary parts: {ex.Message}");
            }
        }

        /// <summary>
        /// Sincroniza Phase de BoltGroups asociados al Assembly.
        /// </summary>
        private void SyncBolts(Part mainPart, int targetPhase)
        {
            try
            {
                // Obtener bolts conectados a las partes del assembly
                Assembly mainAssembly = mainPart.GetAssembly();
                if (mainAssembly == null) return;
                
                // Obtener todos los objetos del assembly incluyendo la main part
                ArrayList allParts = new ArrayList();
                allParts.Add(mainPart);
                ArrayList secondaries = mainAssembly.GetSecondaries();
                foreach (Part p in secondaries)
                {
                    allParts.Add(p);
                }
                
                // Para cada parte, obtener sus bolts
                foreach (Part part in allParts)
                {
                    if (part == null) continue;
                    
                    ModelObjectEnumerator bolts = part.GetBolts();
                    if (bolts == null) continue;
                    
                    while (bolts.MoveNext())
                    {
                        BoltGroup bolt = bolts.Current as BoltGroup;
                        if (bolt == null) continue;

                        _report.BoltsEvaluated++;

                        try
                        {
                            // SIEMPRE intentar cambiar el Phase
                            // Intentar múltiples métodos
                            bool success = false;
                            
                            // Método 1: Usar SetPhase()
                            try
                            {
                                Phase phaseObj;
                                if (bolt.GetPhase(out phaseObj))
                                {
                                    if (phaseObj == null)
                                    {
                                        phaseObj = new Phase();
                                    }
                                    phaseObj.PhaseNumber = targetPhase;
                                    bolt.SetPhase(phaseObj);
                                    success = true;
                                }
                            }
                            catch
                            {
                                // Puede no funcionar
                            }
                            
                            // Método 2: SetUserProperty
                            if (!success)
                            {
                                try
                                {
                                    bolt.SetUserProperty("PHASE_NUMBER", targetPhase);
                                    success = true;
                                }
                                catch
                                {
                                    // Puede no funcionar
                                }
                            }
                            
                            bool modified = bolt.Modify();

                            if (modified && success)
                            {
                                _report.BoltsChanged++;
                            }
                            else
                            {
                                _report.BoltsSkipped++;
                                _report.AddWarning($"Bolt {bolt.Identifier.ID}: " +
                                    "Modify() devolvió false o no se pudo aplicar Phase");
                            }
                        }
                        catch (Exception ex)
                        {
                            _report.BoltsSkipped++;
                            _report.AddError($"Bolt {bolt.Identifier.ID}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _report.AddError($"Error obteniendo bolts: {ex.Message}");
            }
        }

        // NOTA: La sincronización de soldaduras se maneja con la macro de Tekla
        // (MacroPlantilla\SyncWeldPhaseFromParts.cs)
        // Esta aplicación C# solo sincroniza Parts y Bolts
    }
}
