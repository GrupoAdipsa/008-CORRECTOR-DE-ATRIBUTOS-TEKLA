using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace CORRECTOR_DE_ATRIBUTOS
{
    /// <summary>
    /// Herramienta de diagnóstico para ver las propiedades reales de una pieza en Tekla.
    /// </summary>
    public class PhasePropertyDiagnostic
    {
        [STAThread]
        public static void Main()
        {
            var diagnostic = new PhasePropertyDiagnostic();
            diagnostic.Run();
        }

        public void Run()
        {
            Model model = new Model();

            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("No hay conexión con Tekla Structures.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Seleccionar una pieza
                Picker picker = new Picker();
                ModelObject obj = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART,
                    "Selecciona UNA pieza para diagnosticar sus propiedades de Phase");

                Part part = obj as Part;
                if (part == null)
                {
                    MessageBox.Show("El objeto seleccionado no es una pieza.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Diagnosticar propiedades
                StringBuilder report = new StringBuilder();
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine("  DIAGNÓSTICO DE PROPIEDADES DE PHASE");
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine();
                report.AppendLine($"Pieza ID: {part.Identifier.ID}");
                report.AppendLine($"Tipo: {part.GetType().Name}");
                report.AppendLine();

                // Intentar leer diferentes propiedades relacionadas con Phase
                string[] phaseProperties = new string[]
                {
                    "PHASE",
                    "PHASE_NUMBER",
                    "PHASE_NAME",
                    "Phase",
                    "PhaseNumber",
                    "PhaseName",
                    "ASSEMBLY_PHASE",
                    "ACT_PHASE_NUMBER",
                    "ACT_PHASE_NAME",
                    "PROJECT_PHASE"
                };

                report.AppendLine("???????????????????????????????????????");
                report.AppendLine("PROPIEDADES DE USUARIO ENCONTRADAS:");
                report.AppendLine("???????????????????????????????????????");

                bool foundAny = false;
                foreach (string propName in phaseProperties)
                {
                    string value = "";
                    int intValue = 0;
                    double doubleValue = 0.0;

                    // Intentar como string
                    if (part.GetUserProperty(propName, ref value))
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            report.AppendLine($"? {propName} = \"{value}\" (string)");
                            foundAny = true;
                        }
                    }

                    // Intentar como int
                    if (part.GetUserProperty(propName, ref intValue))
                    {
                        if (intValue != 0)
                        {
                            report.AppendLine($"? {propName} = {intValue} (int)");
                            foundAny = true;
                        }
                    }

                    // Intentar como double
                    if (part.GetUserProperty(propName, ref doubleValue))
                    {
                        if (doubleValue != 0.0)
                        {
                            report.AppendLine($"? {propName} = {doubleValue} (double)");
                            foundAny = true;
                        }
                    }
                }

                if (!foundAny)
                {
                    report.AppendLine("? No se encontraron propiedades de Phase en esta pieza.");
                }

                report.AppendLine();
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine("PROPIEDADES ESTÁNDAR DE TEKLA:");
                report.AppendLine("???????????????????????????????????????");

                // Propiedades estándar de Part
                report.AppendLine($"Name: {part.Name}");
                report.AppendLine($"Profile: {part.Profile.ProfileString}");
                report.AppendLine($"Material: {part.Material.MaterialString}");
                report.AppendLine($"Class: {part.Class}");
                report.AppendLine($"AssemblyNumber: {part.AssemblyNumber.Prefix}{part.AssemblyNumber.StartNumber}");

                // Intentar obtener todas las User Properties
                report.AppendLine();
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine("TODAS LAS USER PROPERTIES:");
                report.AppendLine("???????????????????????????????????????");

                Hashtable userProperties = new Hashtable();
                part.GetAllUserProperties(ref userProperties);

                if (userProperties.Count > 0)
                {
                    foreach (DictionaryEntry entry in userProperties)
                    {
                        report.AppendLine($"• {entry.Key} = {entry.Value}");
                    }
                }
                else
                {
                    report.AppendLine("(No hay User Properties definidas)");
                }

                // Intentar obtener Report Properties (GetReportProperty)
                report.AppendLine();
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine("REPORT PROPERTIES DE PHASE:");
                report.AppendLine("???????????????????????????????????????");

                string[] reportProperties = new string[]
                {
                    "PHASE",
                    "PHASE_NUMBER", 
                    "PHASE_NAME",
                    "ACT_PHASE",
                    "ACT_PHASE_NUMBER",
                    "ACT_PHASE_NAME",
                    "PROJECT.PHASE"
                };

                bool foundReportProp = false;
                foreach (string propName in reportProperties)
                {
                    string value = "";
                    double doubleValue = 0.0;
                    int intValue = 0;
                    
                    try
                    {
                        // Probar GetReportProperty como string
                        if (part.GetReportProperty(propName, ref value))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                report.AppendLine($"? {propName} = \"{value}\" (string)");
                                foundReportProp = true;
                            }
                        }
                        
                        // Probar GetReportProperty como double
                        if (part.GetReportProperty(propName, ref doubleValue))
                        {
                            if (doubleValue != 0.0)
                            {
                                report.AppendLine($"? {propName} = {doubleValue} (double)");
                                foundReportProp = true;
                            }
                        }
                        
                        // Probar GetReportProperty como int
                        if (part.GetReportProperty(propName, ref intValue))
                        {
                            if (intValue != 0)
                            {
                                report.AppendLine($"? {propName} = {intValue} (int)");
                                foundReportProp = true;
                            }
                        }
                    }
                    catch
                    {
                        // Algunas propiedades pueden lanzar excepciones si no existen
                    }
                }

                if (!foundReportProp)
                {
                    report.AppendLine("? No se encontraron Report Properties de Phase.");
                }

                // Intentar obtener Phase del Assembly
                report.AppendLine();
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine("PROPIEDADES DEL ASSEMBLY:");
                report.AppendLine("???????????????????????????????????????");

                Assembly assembly = part.GetAssembly();
                if (assembly != null)
                {
                    report.AppendLine($"Assembly ID: {assembly.Identifier.ID}");
                    report.AppendLine($"Assembly Name: {assembly.Name}");
                    
                    // User Properties del Assembly
                    Hashtable assemblyUserProps = new Hashtable();
                    assembly.GetAllUserProperties(ref assemblyUserProps);
                    
                    if (assemblyUserProps.Count > 0)
                    {
                        report.AppendLine();
                        report.AppendLine("User Properties del Assembly:");
                        foreach (DictionaryEntry entry in assemblyUserProps)
                        {
                            report.AppendLine($"• {entry.Key} = {entry.Value}");
                        }
                    }
                    else
                    {
                        report.AppendLine("(El Assembly no tiene User Properties)");
                    }
                    
                    // Report Properties del Assembly
                    report.AppendLine();
                    report.AppendLine("Report Properties del Assembly:");
                    bool foundAssemblyReportProp = false;
                    foreach (string propName in reportProperties)
                    {
                        string value = "";
                        try
                        {
                            if (assembly.GetReportProperty(propName, ref value))
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    report.AppendLine($"? {propName} = \"{value}\"");
                                    foundAssemblyReportProp = true;
                                }
                            }
                        }
                        catch { }
                    }
                    
                    if (!foundAssemblyReportProp)
                    {
                        report.AppendLine("? No se encontraron Report Properties de Phase en el Assembly.");
                    }
                }
                else
                {
                    report.AppendLine("? No se pudo obtener el Assembly de esta pieza.");
                }

                report.AppendLine();
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine("  RECOMENDACIÓN");
                report.AppendLine("???????????????????????????????????????");
                report.AppendLine();
                report.AppendLine("Si no ves propiedades de Phase arriba, significa que:");
                report.AppendLine("1. Esta pieza no tiene Phase asignada en Tekla");
                report.AppendLine("2. O tu configuración de Tekla no usa User Properties para Phase");
                report.AppendLine();
                report.AppendLine("SOLUCIÓN:");
                report.AppendLine("- Ve a las propiedades de la pieza en Tekla");
                report.AppendLine("- Busca la pestaña 'Phase' o 'Fase'");
                report.AppendLine("- Asigna un número de fase");
                report.AppendLine("- Luego ejecuta este diagnóstico de nuevo");
                report.AppendLine();
                report.AppendLine("IMPORTANTE:");
                report.AppendLine("Si encuentras propiedades en 'REPORT PROPERTIES',");
                report.AppendLine("tendrás que modificar el código del sincronizador");
                report.AppendLine("para usar GetReportProperty en lugar de GetUserProperty.");

                // Mostrar reporte
                Form reportForm = new Form();
                reportForm.Text = "Diagnóstico de Propiedades de Phase";
                reportForm.Size = new System.Drawing.Size(800, 600);
                reportForm.StartPosition = FormStartPosition.CenterScreen;

                TextBox txtReport = new TextBox();
                txtReport.Multiline = true;
                txtReport.ScrollBars = ScrollBars.Vertical;
                txtReport.Dock = DockStyle.Fill;
                txtReport.Font = new System.Drawing.Font("Consolas", 9F);
                txtReport.Text = report.ToString();
                txtReport.ReadOnly = true;

                Button btnClose = new Button();
                btnClose.Text = "Cerrar";
                btnClose.Dock = DockStyle.Bottom;
                btnClose.Height = 40;
                btnClose.Click += (s, e) => reportForm.Close();

                reportForm.Controls.Add(txtReport);
                reportForm.Controls.Add(btnClose);
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
