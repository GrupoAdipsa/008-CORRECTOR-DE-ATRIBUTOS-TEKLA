using System;
using System.Windows.Forms;

namespace CORRECTOR_DE_ATRIBUTOS
{
    /// <summary>
    /// Formulario para la sincronización de Phase en Assemblies de Tekla.
    /// SOLO PROCESA PARTS - Las soldaduras se manejan con la macro de Tekla.
    /// </summary>
    public partial class PhaseSyncForm : Form
    {
        private PhaseSynchronizer _synchronizer;
        private Button btnExecute;
        private Button btnClose;
        private TextBox txtReport;
        private Label lblInstructions;

        public PhaseSyncForm()
        {
            InitializeComponent();
            _synchronizer = new PhaseSynchronizer();
        }

        private void InitializeComponent()
        {
            this.Text = "Sincronización de Phase - Piezas";
            this.Size = new System.Drawing.Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Label de instrucciones
            this.lblInstructions = new Label();
            this.lblInstructions.Location = new System.Drawing.Point(20, 20);
            this.lblInstructions.Size = new System.Drawing.Size(640, 100);
            this.lblInstructions.Text = 
                "SINCRONIZACIÓN DE PHASE - PIEZAS\n\n" +
                "1. Haz clic en 'Ejecutar Sincronización'\n" +
                "2. Selecciona las piezas o assemblies en Tekla\n" +
                "3. El sistema sincronizará la Phase de la Main Part a todas las secondary parts\n" +
                "4. Verás el reporte de resultados aquí\n\n" +
                "NOTA: Para soldaduras, usa la macro de Tekla (Tools > Macros > SyncWeldPhaseFromParts)";
            this.lblInstructions.Font = new System.Drawing.Font("Segoe UI", 9F);

            // Botón Ejecutar
            this.btnExecute = new Button();
            this.btnExecute.Location = new System.Drawing.Point(20, 130);
            this.btnExecute.Size = new System.Drawing.Size(200, 35);
            this.btnExecute.Text = "Ejecutar Sincronización";
            this.btnExecute.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExecute.Click += new EventHandler(this.BtnExecute_Click);

            // TextBox para reporte
            this.txtReport = new TextBox();
            this.txtReport.Location = new System.Drawing.Point(20, 180);
            this.txtReport.Size = new System.Drawing.Size(640, 280);
            this.txtReport.Multiline = true;
            this.txtReport.ScrollBars = ScrollBars.Vertical;
            this.txtReport.ReadOnly = true;
            this.txtReport.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtReport.BackColor = System.Drawing.Color.White;

            // Botón Cerrar
            this.btnClose = new Button();
            this.btnClose.Location = new System.Drawing.Point(560, 470);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Text = "Cerrar";
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Click += new EventHandler(this.BtnClose_Click);

            // Agregar controles al formulario
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtReport);
            this.Controls.Add(this.btnClose);
        }

        private void BtnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnExecute.Enabled = false;
                txtReport.Text = "Esperando selección en Tekla...\r\n\r\n" +
                                 "INSTRUCCIONES:\r\n" +
                                 "1. Selecciona piezas o assemblies en Tekla\r\n" +
                                 "2. Presiona el BOTÓN CENTRAL del mouse (rueda) o ENTER para confirmar\r\n" +
                                 "3. O presiona ESC para cancelar\r\n\r\n" +
                                 "IMPORTANTE: Las piezas seleccionadas deben tener Phase asignada.\r\n";
                Application.DoEvents();

                // Ejecutar sincronización (SOLO PARTS)
                bool success = _synchronizer.ExecuteInteractive();

                // Mostrar reporte
                if (success || _synchronizer.Report.AssembliesProcessed > 0)
                {
                    txtReport.Text = _synchronizer.Report.GenerateReport();
                    
                    if (_synchronizer.Report.IsSuccessful)
                    {
                        MessageBox.Show(
                            _synchronizer.Report.GenerateSummary(),
                            "Sincronización Exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else if (_synchronizer.Report.HasErrors)
                    {
                        MessageBox.Show(
                            "La sincronización finalizó con errores. Revisa el reporte para más detalles.",
                            "Sincronización con Errores",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                else
                {
                    txtReport.Text = "???????????????????????????????????????\r\n";
                    txtReport.Text += "  SINCRONIZACIÓN NO REALIZADA\r\n";
                    txtReport.Text += "???????????????????????????????????????\r\n\r\n";
                    
                    if (_synchronizer.Report.HasWarnings)
                    {
                        txtReport.Text += "ADVERTENCIAS:\r\n";
                        foreach (var warning in _synchronizer.Report.GetWarnings())
                        {
                            txtReport.Text += $"  ? {warning}\r\n";
                        }
                        txtReport.Text += "\r\n";
                    }
                    
                    if (_synchronizer.Report.HasErrors)
                    {
                        txtReport.Text += "ERRORES:\r\n";
                        foreach (var error in _synchronizer.Report.GetErrors())
                        {
                            txtReport.Text += $"  ? {error}\r\n";
                        }
                        txtReport.Text += "\r\n";
                    }
                    
                    txtReport.Text += "???????????????????????????????????????\r\n";
                    txtReport.Text += "  AYUDA\r\n";
                    txtReport.Text += "???????????????????????????????????????\r\n\r\n";
                    txtReport.Text += "Para sincronizar Phase:\r\n";
                    txtReport.Text += "1. Selecciona la pieza principal del assembly en Tekla\r\n";
                    txtReport.Text += "2. Asegúrate que tiene Phase asignada (doble clic > pestaña Phase)\r\n";
                    txtReport.Text += "3. Ejecuta esta herramienta de nuevo\r\n\r\n";
                    txtReport.Text += "Para SOLDADURAS:\r\n";
                    txtReport.Text += "• Usa la macro de Tekla: Tools > Macros > SyncWeldPhaseFromParts\r\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}\n\n{ex.StackTrace}",
                    "Error Crítico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                txtReport.Text = $"ERROR CRÍTICO:\r\n{ex.Message}\r\n\r\n{ex.StackTrace}";
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnExecute.Enabled = true;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

