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
                System.Windows.Forms.MessageBox.Show("No hay conexion con Tekla Structures.",
                    "Error", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Preguntar al usuario que desea procesar
                var choice = System.Windows.Forms.MessageBox.Show(
                    "Que deseas procesar?\n\n" +
                    "SI = Solo soldaduras SELECCIONADAS\n" +
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
                    Tekla.Structures.Model.UI.ModelObjectSelector selector = new Tekla.Structures.Model.UI.ModelObjectSelector();
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
                    welds = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.WELD);
                }

                // Contadores
                int weldsProcessed = 0;
                int weldsChanged = 0;
                int weldsSkipped = 0;
                int weldsNoPhase = 0;
                System.Text.StringBuilder log = new System.Text.StringBuilder();

                // Diccionario: Phase objetivo -> Lista de soldaduras
                System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<BaseWeld>> weldsByPhase = 
                    new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<BaseWeld>>();

                // PASO 1: Identificar Phase objetivo de cada soldadura y agrupar
                while (welds.MoveNext())
                {
                    BaseWeld weld = welds.Current as BaseWeld;
                    if (weld == null) continue;
                    
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
                            log.AppendLine(string.Format("Weld {0}: No se encontraron piezas conectadas", weld.Identifier.ID));
                            continue;
                        }

                        // Determinar Phase objetivo (prioridad: MainPart, luego SecondaryPart)
                        int targetPhase = 0;
                        string partSource = "";
                        
                        if (mainPart != null)
                        {
                            int phase = 0;
                            if (mainPart.GetReportProperty("PHASE", ref phase) && phase > 0)
                            {
                                targetPhase = phase;
                                partSource = string.Format("MainPart {0}", mainPart.Identifier.ID);
                            }
                        }
                        
                        if (targetPhase == 0 && secondaryPart != null)
                        {
                            int phase = 0;
                            if (secondaryPart.GetReportProperty("PHASE", ref phase) && phase > 0)
                            {
                                targetPhase = phase;
                                partSource = string.Format("SecondaryPart {0}", secondaryPart.Identifier.ID);
                            }
                        }

                        if (targetPhase == 0)
                        {
                            weldsNoPhase++;
                            log.AppendLine(string.Format("Weld {0}: Piezas conectadas sin Phase asignada", weld.Identifier.ID));
                            continue;
                        }

                        // Leer Phase actual de la soldadura
                        int currentPhase = 0;
                        weld.GetReportProperty("PHASE", ref currentPhase);

                        // Si ya tiene el Phase correcto, omitir
                        if (currentPhase == targetPhase && currentPhase > 0)
                        {
                            weldsSkipped++;
                            log.AppendLine(string.Format("Weld {0}: Ya tiene Phase {1} (omitida)", weld.Identifier.ID, currentPhase));
                            continue;
                        }

                        // Agrupar por Phase objetivo
                        if (!weldsByPhase.ContainsKey(targetPhase))
                        {
                            weldsByPhase[targetPhase] = new System.Collections.Generic.List<BaseWeld>();
                        }
                        weldsByPhase[targetPhase].Add(weld);
                        
                        log.AppendLine(string.Format("Weld {0}: Reportado={1}, Target={2} (de {3})", 
                            weld.Identifier.ID, currentPhase, targetPhase, partSource));
                    }
                    catch (Exception ex)
                    {
                        weldsSkipped++;
                        log.AppendLine(string.Format("ERROR Weld {0}: {1}", weld.Identifier.ID, ex.Message));
                    }
                }

                // PASO 2: Aplicar Phase usando Phase Manager por grupos
                if (weldsByPhase.Count > 0)
                {
                    Tekla.Structures.Model.UI.ModelObjectSelector uiSelector = 
                        new Tekla.Structures.Model.UI.ModelObjectSelector();
                    
                    foreach (var kvp in weldsByPhase)
                    {
                        int targetPhase = kvp.Key;
                        System.Collections.Generic.List<BaseWeld> weldsToChange = kvp.Value;

                        try
                        {
                            // Crear ArrayList con los objetos a seleccionar
                            ArrayList objectsToSelect = new ArrayList();
                            foreach (BaseWeld w in weldsToChange)
                            {
                                objectsToSelect.Add(w);
                            }
                            
                            // Seleccionar los objetos
                            uiSelector.Select(objectsToSelect);
                            System.Threading.Thread.Sleep(200);

                            // Abrir Phase Manager
                            wpf.InvokeCommand("CommandRepository", "Tools.PhaseManager");
                            System.Threading.Thread.Sleep(500);
                            
                            // IMPORTANTE: En Phase Manager, el indice de tabla = numero de Phase
                            // Phase 0 = indice 0, Phase 1 = indice 1, Phase 2 = indice 2, etc.
                            int tableIndex = targetPhase;
                            log.AppendLine(string.Format("DEBUG: targetPhase={0}, tableIndex={1}", targetPhase, tableIndex));
                            
                            // Seleccionar Phase en la tabla
                            akit.TableSelect("diaPhaseManager", "tablePhases", new int[] { tableIndex });
                            System.Threading.Thread.Sleep(200);
                            
                            // Aplicar Phase a los objetos seleccionados
                            akit.PushButton("butModifyObjects", "diaPhaseManager");
                            System.Threading.Thread.Sleep(300);
                            
                            // Cerrar Phase Manager
                            akit.PushButton("butOk", "diaPhaseManager");
                            System.Threading.Thread.Sleep(300);

                            // Deseleccionar
                            uiSelector.Select(new ArrayList());

                            // Verificacion post-cambio
                            int successCount = 0;
                            int failCount = 0;
                            foreach (BaseWeld w in weldsToChange)
                            {
                                int verifyPhase = 0;
                                if (w.GetReportProperty("PHASE", ref verifyPhase))
                                {
                                    if (verifyPhase == targetPhase)
                                    {
                                        successCount++;
                                        log.AppendLine(string.Format("  OK: Weld {0} = Phase {1}", w.Identifier.ID, verifyPhase));
                                    }
                                    else
                                    {
                                        failCount++;
                                        log.AppendLine(string.Format("  WARN: Weld {0} = Phase {1}, esperaba {2}", 
                                            w.Identifier.ID, verifyPhase, targetPhase));
                                    }
                                }
                            }

                            weldsChanged += successCount;
                            weldsSkipped += failCount;
                            
                            log.AppendLine("");
                            log.AppendLine(string.Format("==> {0} soldaduras OK a Phase {1}", successCount, targetPhase));
                            if (failCount > 0)
                            {
                                log.AppendLine(string.Format("==> {0} soldaduras fallaron", failCount));
                            }
                            log.AppendLine("");
                        }
                        catch (Exception ex)
                        {
                            weldsSkipped += weldsToChange.Count;
                            log.AppendLine(string.Format("ERROR al aplicar Phase {0}: {1}", targetPhase, ex.Message));
                        }
                    }

                    // Commit de cambios
                    model.CommitChanges();
                    System.Threading.Thread.Sleep(500);
                    
                    // Refrescar la vista para actualizar la representacion visual
                    if (weldsChanged > 0)
                    {
                        try
                        {
                            // Metodo 1: Forzar refresco usando comando de Tekla
                            Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Actualizando representacion visual...");
                            System.Threading.Thread.Sleep(200);
                            Tekla.Structures.Model.Operations.Operation.DisplayPrompt("");
                            
                            // Metodo 2: Refrescar vista usando AKIT
                            try
                            {
                                akit.CommandStart("View.Refresh", "", "main_frame");
                            }
                            catch { }
                        }
                        catch (Exception refreshEx)
                        {
                            log.AppendLine(string.Format("WARN: No se pudo refrescar vista automaticamente: {0}", refreshEx.Message));
                        }
                    }
                }

                // PASO 3: Reporte final
                string scope = processOnlySelected ? "SELECCIONADAS" : "TODO EL MODELO";
                
                string finalReport = "=======================================\n" +
                                   "  SINCRONIZACION DE PHASE - SOLDADURAS\n" +
                                   string.Format("  Alcance: {0}\n", scope) +
                                   "=======================================\n\n" +
                                   string.Format("Soldaduras procesadas: {0}\n", weldsProcessed) +
                                   string.Format("Soldaduras actualizadas: {0}\n", weldsChanged) +
                                   string.Format("Soldaduras omitidas (ya correctas): {0}\n", weldsSkipped) +
                                   string.Format("Soldaduras sin Phase en piezas: {0}\n\n", weldsNoPhase);

                if (weldsChanged > 0)
                {
                    finalReport += "[OK] Cambios guardados en el modelo.\n\n";
                }
                else
                {
                    finalReport += "[INFO] No se realizaron cambios.\n\n";
                }

                if (log.Length > 0)
                {
                    finalReport += "=======================================\n" +
                                 "DETALLES:\n" +
                                 "=======================================\n" +
                                 log.ToString();
                }

                // Guardar log en archivo
                string logPath = "";
                try
                {
                    logPath = System.IO.Path.Combine(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
                        string.Format("SyncWeldPhase_{0}.txt", System.DateTime.Now.ToString("yyyyMMdd_HHmmss")));
                    
                    System.IO.File.WriteAllText(logPath, finalReport, System.Text.Encoding.UTF8);
                }
                catch
                {
                    logPath = "";
                }

                // Mostrar reporte con botones
                System.Windows.Forms.Form reportForm = new System.Windows.Forms.Form();
                reportForm.Text = "Reporte de Sincronizacion";
                reportForm.Width = 700;
                reportForm.Height = 600;
                reportForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                reportForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                reportForm.MaximizeBox = true;
                reportForm.MinimizeBox = false;

                System.Windows.Forms.TextBox txtReport = new System.Windows.Forms.TextBox();
                txtReport.Multiline = true;
                txtReport.ScrollBars = System.Windows.Forms.ScrollBars.Both;
                txtReport.ReadOnly = true;
                txtReport.Font = new System.Drawing.Font("Consolas", 9);
                txtReport.Dock = System.Windows.Forms.DockStyle.Fill;
                txtReport.Text = finalReport;
                txtReport.WordWrap = false;

                System.Windows.Forms.Panel pnlButtons = new System.Windows.Forms.Panel();
                pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
                pnlButtons.Height = 50;

                System.Windows.Forms.Button btnCopy = new System.Windows.Forms.Button();
                btnCopy.Text = "Copiar al Portapapeles";
                btnCopy.Width = 180;
                btnCopy.Height = 30;
                btnCopy.Left = 10;
                btnCopy.Top = 10;
                btnCopy.Click += delegate
                {
                    try
                    {
                        System.Windows.Forms.Clipboard.SetText(finalReport);
                        System.Windows.Forms.MessageBox.Show("Reporte copiado al portapapeles!", 
                            "Exito", System.Windows.Forms.MessageBoxButtons.OK, 
                            System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error al copiar: " + ex.Message, 
                            "Error", System.Windows.Forms.MessageBoxButtons.OK, 
                            System.Windows.Forms.MessageBoxIcon.Error);
                    }
                };

                System.Windows.Forms.Button btnOpenFile = new System.Windows.Forms.Button();
                btnOpenFile.Text = "Abrir Archivo";
                btnOpenFile.Width = 120;
                btnOpenFile.Height = 30;
                btnOpenFile.Left = 200;
                btnOpenFile.Top = 10;
                btnOpenFile.Enabled = (logPath != "");
                if (logPath != "")
                {
                    btnOpenFile.Click += delegate
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(logPath);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show("Error al abrir archivo: " + ex.Message, 
                                "Error", System.Windows.Forms.MessageBoxButtons.OK, 
                                System.Windows.Forms.MessageBoxIcon.Error);
                        }
                    };
                }

                System.Windows.Forms.Button btnClose = new System.Windows.Forms.Button();
                btnClose.Text = "Cerrar";
                btnClose.Width = 100;
                btnClose.Height = 30;
                btnClose.Left = 330;
                btnClose.Top = 10;
                btnClose.Click += delegate { reportForm.Close(); };

                pnlButtons.Controls.Add(btnCopy);
                pnlButtons.Controls.Add(btnOpenFile);
                pnlButtons.Controls.Add(btnClose);

                reportForm.Controls.Add(txtReport);
                reportForm.Controls.Add(pnlButtons);

                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    string.Format("Error critico: {0}\n\n{1}", ex.Message, ex.StackTrace),
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
