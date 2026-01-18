using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace SincronizadorAtributos
{
    /// <summary>
    /// Herramienta de diagnóstico completo para mapear un Assembly completo.
    /// Muestra Main Part, Secondary Parts, Bolts y todos sus atributos.
    /// </summary>
    public class AttributeDiagnostic
    {
        /// <summary>
        /// Mapea un Assembly completo mostrando todos los detalles.
        /// </summary>
        public static void ShowAssemblyMapping()
        {
            try
            {
                Model model = new Model();
                
                if (!model.GetConnectionStatus())
                {
                    MessageBox.Show(
                        "No hay conexión con Tekla Structures.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Obtener objeto seleccionado
                Picker picker = new Picker();
                ModelObject selectedObject = null;
                
                try
                {
                    selectedObject = picker.PickObject(
                        Picker.PickObjectEnum.PICK_ONE_OBJECT,
                        "Selecciona UNA parte del Assembly para mapear completamente"
                    );
                }
                catch (Exception pickEx)
                {
                    if (pickEx.Message.Contains("interrupt") || pickEx.Message.Contains("cancel"))
                    {
                        return; // Usuario canceló
                    }
                    throw;
                }

                if (selectedObject == null)
                {
                    MessageBox.Show(
                        "No se seleccionó ningún objeto.",
                        "Sin Selección",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Obtener Assembly
                Assembly assembly = null;
                if (selectedObject is Part)
                {
                    assembly = (selectedObject as Part).GetAssembly();
                }
                else
                {
                    MessageBox.Show(
                        "Por favor selecciona una Part (viga, columna, etc.)",
                        "Objeto No Válido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (assembly == null || assembly.Identifier.ID <= 0)
                {
                    MessageBox.Show(
                        "La parte seleccionada no pertenece a ningún Assembly.",
                        "Sin Assembly",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Generar reporte completo del Assembly
                StringBuilder report = new StringBuilder();
                
                report.AppendLine("???????????????????????????????????????????????????????");
                report.AppendLine("  MAPEO COMPLETO DE ASSEMBLY");
                report.AppendLine("???????????????????????????????????????????????????????");
                report.AppendLine();
                report.AppendLine($"Assembly ID: {assembly.Identifier.ID}");
                report.AppendLine($"Parte seleccionada: {selectedObject.GetType().Name} (ID {selectedObject.Identifier.ID})");
                report.AppendLine();

                // MAIN PART
                Part mainPart = assembly.GetMainPart() as Part;
                if (mainPart != null)
                {
                    report.AppendLine("???????????????????????????????????????????????????????");
                    report.AppendLine("  MAIN PART");
                    report.AppendLine("???????????????????????????????????????????????????????");
                    report.AppendLine($"ID: {mainPart.Identifier.ID}");
                    report.AppendLine($"Tipo: {mainPart.GetType().Name}");
                    
                    // Leer atributos de Main Part
                    string estatusMainPart = ReadAttribute(mainPart, "ESTATUS_PIEZA");
                    string prioridadMainPart = ReadAttribute(mainPart, "PRIORIDAD");
                    
                    report.AppendLine();
                    report.AppendLine("ATRIBUTOS:");
                    report.AppendLine($"  ESTATUS_PIEZA: '{estatusMainPart}' {(string.IsNullOrEmpty(estatusMainPart) ? "[VACÍO]" : "[OK]")}");
                    report.AppendLine($"  PRIORIDAD:     '{prioridadMainPart}' {(string.IsNullOrEmpty(prioridadMainPart) ? "[VACÍO]" : "[OK]")}");
                    report.AppendLine();
                }
                else
                {
                    report.AppendLine("? Assembly sin Main Part");
                    report.AppendLine();
                }

                // SECONDARY PARTS
                ArrayList secondaries = assembly.GetSecondaries();
                if (secondaries != null && secondaries.Count > 0)
                {
                    report.AppendLine("???????????????????????????????????????????????????????");
                    report.AppendLine($"  SECONDARY PARTS ({secondaries.Count} piezas)");
                    report.AppendLine("???????????????????????????????????????????????????????");
                    
                    int index = 1;
                    foreach (Part part in secondaries)
                    {
                        if (part == null) continue;

                        string estatus = ReadAttribute(part, "ESTATUS_PIEZA");
                        string prioridad = ReadAttribute(part, "PRIORIDAD");

                        report.AppendLine($"[{index}] Part ID: {part.Identifier.ID}");
                        report.AppendLine($"    ESTATUS_PIEZA: '{estatus}' {(string.IsNullOrEmpty(estatus) ? "[VACÍO]" : "[OK]")}");
                        report.AppendLine($"    PRIORIDAD:     '{prioridad}' {(string.IsNullOrEmpty(prioridad) ? "[VACÍO]" : "[OK]")}");
                        report.AppendLine();
                        
                        index++;
                    }
                }
                else
                {
                    report.AppendLine("???????????????????????????????????????????????????????");
                    report.AppendLine("  SECONDARY PARTS");
                    report.AppendLine("???????????????????????????????????????????????????????");
                    report.AppendLine("(Sin Secondary Parts)");
                    report.AppendLine();
                }

                // BOLTS
                if (mainPart != null)
                {
                    ModelObjectEnumerator bolts = mainPart.GetBolts();
                    int boltCount = 0;
                    if (bolts != null)
                    {
                        while (bolts.MoveNext()) boltCount++;
                        bolts.SelectInstances = false;
                    }

                    report.AppendLine("???????????????????????????????????????????????????????");
                    report.AppendLine($"  BOLTS ({boltCount} grupos)");
                    report.AppendLine("???????????????????????????????????????????????????????");
                    
                    if (boltCount > 0)
                    {
                        bolts = mainPart.GetBolts();
                        int index = 1;
                        while (bolts.MoveNext())
                        {
                            BoltGroup bolt = bolts.Current as BoltGroup;
                            if (bolt == null) continue;

                            string estatus = ReadAttribute(bolt, "ESTATUS_PIEZA");
                            string prioridad = ReadAttribute(bolt, "PRIORIDAD");

                            report.AppendLine($"[{index}] Bolt ID: {bolt.Identifier.ID}");
                            report.AppendLine($"    ESTATUS_PIEZA: '{estatus}' {(string.IsNullOrEmpty(estatus) ? "[VACÍO]" : "[OK]")}");
                            report.AppendLine($"    PRIORIDAD:     '{prioridad}' {(string.IsNullOrEmpty(prioridad) ? "[VACÍO]" : "[OK]")}");
                            report.AppendLine();
                            
                            index++;
                        }
                    }
                    else
                    {
                        report.AppendLine("(Sin Bolts)");
                        report.AppendLine();
                    }
                }

                report.AppendLine("???????????????????????????????????????????????????????");
                report.AppendLine("  INSTRUCCIONES");
                report.AppendLine("???????????????????????????????????????????????????????");
                report.AppendLine();
                report.AppendLine("1. Verifica los valores mostrados arriba");
                report.AppendLine("2. Compara con lo que ves en Tekla");
                report.AppendLine("3. Si los valores NO coinciden, avísame");
                report.AppendLine("4. Si coinciden, ejecuta el sincronizador");
                report.AppendLine();
                report.AppendLine("???????????????????????????????????????????????????????");

                // Mostrar reporte
                ShowReportWindow(report.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}\n\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lee un atributo usando múltiples métodos.
        /// </summary>
        private static string ReadAttribute(ModelObject obj, string attributeName)
        {
            string value = "";

            // Método 1: GetReportProperty (más confiable en Tekla 2021)
            try
            {
                if (obj.GetReportProperty(attributeName, ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            // Método 2: GetUserProperty
            try
            {
                value = "";
                if (obj.GetUserProperty(attributeName, ref value) && !string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            catch { }

            return "";
        }

        /// <summary>
        /// Muestra la ventana de reporte.
        /// </summary>
        private static void ShowReportWindow(string reportText)
        {
            Form reportForm = new Form();
            reportForm.Text = "Mapeo Completo de Assembly";
            reportForm.Width = 900;
            reportForm.Height = 700;
            reportForm.StartPosition = FormStartPosition.CenterScreen;

            TextBox txtReport = new TextBox();
            txtReport.Multiline = true;
            txtReport.ScrollBars = ScrollBars.Both;
            txtReport.ReadOnly = true;
            txtReport.Font = new System.Drawing.Font("Consolas", 9);
            txtReport.Dock = DockStyle.Fill;
            txtReport.Text = reportText;
            txtReport.WordWrap = false;

            Panel pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 50;

            Button btnCopy = new Button();
            btnCopy.Text = "Copiar Reporte";
            btnCopy.Width = 150;
            btnCopy.Height = 30;
            btnCopy.Left = 10;
            btnCopy.Top = 10;
            btnCopy.Click += delegate
            {
                try
                {
                    Clipboard.SetText(txtReport.Text);
                    MessageBox.Show("Reporte copiado!", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { }
            };

            Button btnClose = new Button();
            btnClose.Text = "Cerrar";
            btnClose.Width = 100;
            btnClose.Height = 30;
            btnClose.Left = 170;
            btnClose.Top = 10;
            btnClose.Click += delegate { reportForm.Close(); };

            pnlButtons.Controls.Add(btnCopy);
            pnlButtons.Controls.Add(btnClose);

            reportForm.Controls.Add(txtReport);
            reportForm.Controls.Add(pnlButtons);

            reportForm.ShowDialog();
        }

        /// <summary>
        /// Método legacy para diagnóstico de un solo objeto.
        /// </summary>
        public static void ShowAllAttributes()
        {
            try
            {
                Model model = new Model();
                
                if (!model.GetConnectionStatus())
                {
                    MessageBox.Show(
                        "No hay conexión con Tekla Structures.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Obtener objeto seleccionado
                Picker picker = new Picker();
                ModelObject selectedObject = picker.PickObject(
                    Picker.PickObjectEnum.PICK_ONE_OBJECT,
                    "Selecciona una Part para ver todos sus atributos"
                );

                if (selectedObject == null)
                {
                    MessageBox.Show(
                        "No se seleccionó ningún objeto.",
                        "Sin Selección",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Obtener todos los atributos
                StringBuilder report = new StringBuilder();
                report.AppendLine("???????????????????????????????????????????");
                report.AppendLine("  DIAGNÓSTICO DE ATRIBUTOS");
                report.AppendLine("???????????????????????????????????????????");
                report.AppendLine();
                report.AppendLine($"Objeto: {selectedObject.GetType().Name}");
                report.AppendLine($"ID: {selectedObject.Identifier.ID}");
                report.AppendLine();
                report.AppendLine("???????????????????????????????????????????");
                report.AppendLine("  USER PROPERTIES");
                report.AppendLine("???????????????????????????????????????????");
                report.AppendLine();

                // Lista de nombres comunes a probar
                string[] commonNames = new string[]
                {
                    // ESTATUS_PIEZA variantes
                    "ESTATUS_PIEZA",
                    "Estatus de Pieza:",
                    "Estatus de Pieza",
                    "ESTATUS DE PIEZA",
                    "STATUS_PIEZA",
                    "Status de Pieza:",
                    "Status de Pieza",
                    "ESTADO_PIEZA",
                    "Estado de Pieza:",
                    "Estado de Pieza",
                    "ESTATUS",
                    "STATUS",
                    "ESTADO",
                    
                    // PRIORIDAD variantes
                    "PRIORIDAD",
                    "PRIORIDAD DETALLADO:",
                    "PRIORIDAD DETALLADO",
                    "Prioridad Detallado:",
                    "Prioridad Detallado",
                    "PRIORITY",
                    "Priority:",
                    "Priority"
                };

                int found = 0;
                foreach (string name in commonNames)
                {
                    string value = "";
                    try
                    {
                        if (selectedObject.GetUserProperty(name, ref value))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                report.AppendLine($"? [{name}] = '{value}'");
                                found++;
                            }
                        }
                    }
                    catch { }
                }

                if (found == 0)
                {
                    report.AppendLine("[!] No se encontraron atributos con nombres comunes");
                    report.AppendLine();
                    report.AppendLine("NOTA: Esto significa que el nombre del atributo es diferente.");
                    report.AppendLine("Por favor, abre las propiedades del objeto en Tekla y");
                    report.AppendLine("copia el nombre EXACTO del campo que contiene los valores.");
                }
                else
                {
                    report.AppendLine();
                    report.AppendLine($"Total encontrados: {found}");
                }

                report.AppendLine();
                report.AppendLine("???????????????????????????????????????????");
                report.AppendLine("  REPORT PROPERTIES");
                report.AppendLine("???????????????????????????????????????????");
                report.AppendLine();

                // Probar con ReportProperty
                int foundReport = 0;
                foreach (string name in commonNames)
                {
                    string value = "";
                    try
                    {
                        if (selectedObject.GetReportProperty(name, ref value))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                report.AppendLine($"? [{name}] = '{value}'");
                                foundReport++;
                            }
                        }
                    }
                    catch { }
                }

                if (foundReport == 0)
                {
                    report.AppendLine("[!] No se encontraron report properties");
                }
                else
                {
                    report.AppendLine();
                    report.AppendLine($"Total encontrados: {foundReport}");
                }

                report.AppendLine();
                report.AppendLine("???????????????????????????????????????????");

                // Mostrar reporte
                Form reportForm = new Form();
                reportForm.Text = "Diagnóstico de Atributos";
                reportForm.Width = 800;
                reportForm.Height = 600;
                reportForm.StartPosition = FormStartPosition.CenterScreen;

                TextBox txtReport = new TextBox();
                txtReport.Multiline = true;
                txtReport.ScrollBars = ScrollBars.Both;
                txtReport.ReadOnly = true;
                txtReport.Font = new System.Drawing.Font("Consolas", 9);
                txtReport.Dock = DockStyle.Fill;
                txtReport.Text = report.ToString();
                txtReport.WordWrap = false;

                Panel pnlButtons = new Panel();
                pnlButtons.Dock = DockStyle.Bottom;
                pnlButtons.Height = 50;

                Button btnCopy = new Button();
                btnCopy.Text = "Copiar Reporte";
                btnCopy.Width = 150;
                btnCopy.Height = 30;
                btnCopy.Left = 10;
                btnCopy.Top = 10;
                btnCopy.Click += delegate
                {
                    try
                    {
                        Clipboard.SetText(txtReport.Text);
                        MessageBox.Show("Reporte copiado!", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                };

                Button btnClose = new Button();
                btnClose.Text = "Cerrar";
                btnClose.Width = 100;
                btnClose.Height = 30;
                btnClose.Left = 170;
                btnClose.Top = 10;
                btnClose.Click += delegate { reportForm.Close(); };

                pnlButtons.Controls.Add(btnCopy);
                pnlButtons.Controls.Add(btnClose);

                reportForm.Controls.Add(txtReport);
                reportForm.Controls.Add(pnlButtons);

                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}\n\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
