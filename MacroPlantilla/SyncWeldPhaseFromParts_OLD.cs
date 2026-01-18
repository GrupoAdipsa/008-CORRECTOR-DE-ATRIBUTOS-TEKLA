#pragma warning disable 1633
#pragma reference "Tekla.Macros.Wpf.Runtime"
#pragma reference "Tekla.Macros.Akit"
#pragma reference "Tekla.Macros.Runtime"
#pragma warning restore 1633

using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace UserMacros
{
    public sealed class Macro
    {
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime)
        {
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            
            Model model = new Model();
            
            if (!model.GetConnectionStatus())
            {
                System.Windows.Forms.MessageBox.Show("No hay conexión con Tekla Structures.",
                    "Error", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Preguntar al usuario qué desea procesar
                var choice = System.Windows.Forms.MessageBox.Show(
                    "¿Qué deseas procesar?\n\n" +
                    "SÍ = Solo soldaduras SELECCIONADAS\n" +
                    "NO = TODAS las soldaduras del modelo\n\n" +
                    "Cancelar = Salir",
                    "Sincronizar Phase de Soldaduras",
                    System.Windows.Forms.MessageBoxButtons.YesNoCancel,
                    System.Windows.Forms.MessageBoxIcon.Question);

                if (choice == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }

                bool processOnlySelected = (choice == System.Windows.Forms.DialogResult.Yes);
                
                // Obtener soldaduras a procesar
                ModelObjectEnumerator welds = null;
                
                if (processOnlySelected)
                {
                    ModelObjectSelector selector = new ModelObjectSelector();
                    welds = selector.GetSelectedObjects();
                    
                    if (welds == null || welds.GetSize() == 0)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            "No hay soldaduras seleccionadas.\n\n" +
                            "Por favor selecciona las soldaduras antes de ejecutar la macro.",
                            "Advertencia",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    // Obtener todas las soldaduras del modelo
                    welds = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.WELD);
                }

                // Procesar soldaduras
                int weldsProcessed = 0;
                int weldsChanged = 0;
                int weldsSkipped = 0;
                int weldsNoPhase = 0;
                System.Text.StringBuilder log = new System.Text.StringBuilder();

                while (welds.MoveNext())
                {
                    BaseWeld weld = welds.Current as BaseWeld;
                    if (weld == null) continue;
                    
                    // Si estamos procesando seleccionadas, verificar que sea una soldadura
                    if (processOnlySelected)
                    {
                        ModelObject obj = welds.Current as ModelObject;
                        if (!(obj is BaseWeld)) continue;
                    }

                    weldsProcessed++;

                    try
                    {
                        // Obtener piezas conectadas
                        Part mainPart = weld.MainObject as Part;
                        Part secondaryPart = weld.SecondaryObject as Part;

                        if (mainPart == null && secondaryPart == null)
                        {
                            weldsSkipped++;
                            log.AppendLine($"Weld {weld.Identifier.ID}: No se pudieron obtener las piezas conectadas");
                            continue;
                        }

                        // Determinar el Phase (prioridad: MainPart, luego SecondaryPart)
                        int targetPhase = 0;
                        string partSource = "";
                        
                        if (mainPart != null)
                        {
                            mainPart.GetReportProperty("PHASE", ref targetPhase);
                            partSource = $"MainPart {mainPart.Identifier.ID}";
                        }
                        
                        if (targetPhase == 0 && secondaryPart != null)
                        {
                            secondaryPart.GetReportProperty("PHASE", ref targetPhase);
                            partSource = $"SecondaryPart {secondaryPart.Identifier.ID}";
                        }

                        if (targetPhase == 0)
                        {
                            weldsNoPhase++;
                            log.AppendLine($"Weld {weld.Identifier.ID}: Las piezas conectadas no tienen Phase asignada");
                            continue;
                        }

                        // Leer Phase actual de la soldadura
                        int currentPhase = 0;
                        weld.GetReportProperty("PHASE", ref currentPhase);

                        // Si ya tiene el Phase correcto, omitir
                        if (currentPhase == targetPhase)
                        {
                            weldsSkipped++;
                            continue;
                        }

                        // Asignar nuevo Phase
                        weld.SetUserProperty("PHASE", targetPhase);
                        
                        bool modified = weld.Modify();

                        if (modified)
                        {
                            weldsChanged++;
                            log.AppendLine($"? Weld {weld.Identifier.ID}: Phase {currentPhase} ? {targetPhase} (de {partSource})");
                        }
                        else
                        {
                            weldsSkipped++;
                            log.AppendLine($"? Weld {weld.Identifier.ID}: Modify() devolvió false");
                        }
                    }
                    catch (Exception ex)
                    {
                        weldsSkipped++;
                        log.AppendLine($"? Weld {weld.Identifier.ID}: Error - {ex.Message}");
                    }
                }

                // Commit de cambios
                if (weldsChanged > 0)
                {
                    model.CommitChanges();
                }

                // Reporte final
                string scope = processOnlySelected ? "SELECCIONADAS" : "TODO EL MODELO";
                
                string finalReport = $"???????????????????????????????????????\n" +
                                   $"  SINCRONIZACIÓN DE PHASE - SOLDADURAS\n" +
                                   $"  Alcance: {scope}\n" +
                                   $"???????????????????????????????????????\n\n" +
                                   $"Soldaduras procesadas: {weldsProcessed}\n" +
                                   $"Soldaduras actualizadas: {weldsChanged}\n" +
                                   $"Soldaduras omitidas (ya correctas): {weldsSkipped}\n" +
                                   $"Soldaduras sin Phase en piezas: {weldsNoPhase}\n\n";

                if (weldsChanged > 0)
                {
                    finalReport += "? Cambios guardados en el modelo.\n\n";
                }

                if (log.Length > 0 && weldsChanged > 0)
                {
                    finalReport += $"???????????????????????????????????????\n" +
                                 $"DETALLES DE CAMBIOS:\n" +
                                 $"???????????????????????????????????????\n" +
                                 log.ToString();
                }

                System.Windows.Forms.MessageBox.Show(finalReport,
                    "Reporte de Sincronización",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    weldsChanged > 0 ? System.Windows.Forms.MessageBoxIcon.Information : System.Windows.Forms.MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Error crítico: {ex.Message}\n\n{ex.StackTrace}",
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
