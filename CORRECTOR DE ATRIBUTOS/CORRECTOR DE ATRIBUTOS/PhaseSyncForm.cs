using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CORRECTOR_DE_ATRIBUTOS
{
    /// <summary>
    /// Formulario para la sincronización de Phase en Assemblies de Tekla.
    /// SOLO PROCESA PARTS - Las soldaduras se manejan con la macro de Tekla.
    /// </summary>
    public partial class PhaseSyncForm : Form
    {
        private PhaseSynchronizer _synchronizer;
        private Button btnExecuteSelected;
        private Button btnExecuteAll;
        private Button btnClose;
        private TextBox txtReport;
        private Label lblInstructions;
        private Timer _windowSyncTimer;
        private IntPtr _teklaHandle = IntPtr.Zero;
        
        // Importar funciones de Windows para sincronizar ventana con Tekla
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const int SW_RESTORE = 9;
        private const int SW_MINIMIZE = 6;
        private const int SW_MAXIMIZE = 3;

        public PhaseSyncForm()
        {
            InitializeComponent();
            _synchronizer = new PhaseSynchronizer();
            
            // Buscar ventana de Tekla para sincronización
            FindTeklaWindow();
            
            // Configurar timer para sincronizar estado de la ventana con Tekla
            _windowSyncTimer = new Timer();
            _windowSyncTimer.Interval = 500; // Revisar cada 500ms
            _windowSyncTimer.Tick += WindowSyncTimer_Tick;
            _windowSyncTimer.Start();
            
            // Mantener ventana siempre al frente
            this.TopMost = true;
        }
        
        /// <summary>
        /// Busca la ventana principal de Tekla Structures.
        /// </summary>
        private void FindTeklaWindow()
        {
            try
            {
                // Buscar proceso de Tekla
                Process[] teklaProcesses = Process.GetProcessesByName("TeklaStructures");
                if (teklaProcesses.Length == 0)
                {
                    teklaProcesses = Process.GetProcessesByName("TeklaStructures.exe");
                }
                
                if (teklaProcesses.Length > 0)
                {
                    _teklaHandle = teklaProcesses[0].MainWindowHandle;
                }
            }
            catch
            {
                // Si no se puede encontrar, continuamos sin sincronización
            }
        }
        
        /// <summary>
        /// Timer para sincronizar el estado de maximizado/minimizado con Tekla.
        /// NOTA: Solo sincroniza minimizado, NO maximiza para no tapar Tekla.
        /// </summary>
        private void WindowSyncTimer_Tick(object sender, EventArgs e)
        {
            if (_teklaHandle == IntPtr.Zero)
            {
                FindTeklaWindow();
                return;
            }
            
            try
            {
                // Verificar estado de la ventana de Tekla
                if (IsIconic(_teklaHandle))
                {
                    // Tekla está minimizada -> minimizar esta ventana
                    if (this.WindowState != FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Minimized;
                    }
                }
                else
                {
                    // Tekla NO está minimizada -> asegurar que esta ventana esté visible
                    // pero SIN maximizar para no tapar Tekla
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    
                    // NO sincronizar maximizado - dejar que el usuario controle el tamaño
                    // Esto evita que la ventana tape completamente a Tekla
                }
                
                // Mantener ventana siempre al frente
                if (!this.TopMost)
                {
                    this.TopMost = true;
                }
            }
            catch
            {
                // Si hay error, buscar ventana de nuevo
                _teklaHandle = IntPtr.Zero;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Sincronización de Phase - Piezas v2.1";
            this.Size = new System.Drawing.Size(850, 650);
            this.StartPosition = FormStartPosition.Manual;
            
            // Posicionar la ventana en la esquina superior derecha
            // Dejar espacio para ver Tekla a la izquierda
            int screenWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            this.Location = new System.Drawing.Point(
                screenWidth - this.Width - 20,  // 20px desde el borde derecho
                20  // 20px desde el borde superior
            );
            
            this.FormBorderStyle = FormBorderStyle.Sizable; // Permitir redimensionar
            this.MaximizeBox = true; // Permitir maximizar (pero el usuario decide)
            this.MinimizeBox = true; // Permitir minimizar

            // Label de instrucciones
            this.lblInstructions = new Label();
            this.lblInstructions.Location = new System.Drawing.Point(20, 20);
            this.lblInstructions.Size = new System.Drawing.Size(740, 120);
            this.lblInstructions.Text = 
                "SINCRONIZACIÓN DE PHASE - PIEZAS\n\n" +
                "MODO 1 - OBJETOS SELECCIONADOS:\n" +
                "  - Haz clic en 'Sincronizar Seleccionados'\n" +
                "  - Selecciona piezas o assemblies en Tekla\n" +
                "  - Ideal para correcciones puntuales\n\n" +
                "MODO 2 - TODO EL MODELO (Procesamiento por lotes):\n" +
                "  - Haz clic en 'Sincronizar Todo el Modelo'\n" +
                "  - Procesa todos los assemblies automáticamente\n" +
                "  - Ideal para sincronización completa (alto rendimiento)\n\n" +
                "NOTA: Para soldaduras, usa la macro de Tekla (Tools > Macros > SyncWeldPhaseFromParts)";
            this.lblInstructions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInstructions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Botón Sincronizar Seleccionados
            this.btnExecuteSelected = new Button();
            this.btnExecuteSelected.Location = new System.Drawing.Point(20, 150);
            this.btnExecuteSelected.Size = new System.Drawing.Size(250, 40);
            this.btnExecuteSelected.Text = "Sincronizar Seleccionados";
            this.btnExecuteSelected.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExecuteSelected.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnExecuteSelected.ForeColor = System.Drawing.Color.White;
            this.btnExecuteSelected.FlatStyle = FlatStyle.Flat;
            this.btnExecuteSelected.Click += new EventHandler(this.BtnExecuteSelected_Click);
            
            // Botón Sincronizar Todo el Modelo
            this.btnExecuteAll = new Button();
            this.btnExecuteAll.Location = new System.Drawing.Point(290, 150);
            this.btnExecuteAll.Size = new System.Drawing.Size(250, 40);
            this.btnExecuteAll.Text = "Sincronizar Todo el Modelo";
            this.btnExecuteAll.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExecuteAll.BackColor = System.Drawing.Color.FromArgb(16, 137, 62);
            this.btnExecuteAll.ForeColor = System.Drawing.Color.White;
            this.btnExecuteAll.FlatStyle = FlatStyle.Flat;
            this.btnExecuteAll.Click += new EventHandler(this.BtnExecuteAll_Click);

            // TextBox para reporte
            this.txtReport = new TextBox();
            this.txtReport.Location = new System.Drawing.Point(20, 210);
            this.txtReport.Size = new System.Drawing.Size(740, 320);
            this.txtReport.Multiline = true;
            this.txtReport.ScrollBars = ScrollBars.Vertical;
            this.txtReport.ReadOnly = true;
            this.txtReport.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtReport.BackColor = System.Drawing.Color.White;
            this.txtReport.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Botón Cerrar
            this.btnClose = new Button();
            this.btnClose.Location = new System.Drawing.Point(660, 540);
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.Text = "Cerrar";
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Click += new EventHandler(this.BtnClose_Click);

            // Agregar controles al formulario
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnExecuteSelected);
            this.Controls.Add(this.btnExecuteAll);
            this.Controls.Add(this.txtReport);
            this.Controls.Add(this.btnClose);
        }

        /// <summary>
        /// Maneja el clic en el botón "Sincronizar Seleccionados".
        /// Permite al usuario seleccionar objetos específicos en Tekla.
        /// </summary>
        private void BtnExecuteSelected_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnExecuteSelected.Enabled = false;
                btnExecuteAll.Enabled = false;
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
                btnExecuteSelected.Enabled = true;
                btnExecuteAll.Enabled = true;
            }
        }
        
        /// <summary>
        /// Maneja el clic en el botón "Sincronizar Todo el Modelo".
        /// Procesa todos los assemblies del modelo en lotes para mejor rendimiento.
        /// </summary>
        private void BtnExecuteAll_Click(object sender, EventArgs e)
        {
            try
            {
                // Confirmación del usuario
                DialogResult result = MessageBox.Show(
                    "¿Est?s seguro de que deseas sincronizar TODO el modelo?\n\n" +
                    "Esto procesar? TODOS los assemblies del modelo en lotes.\n" +
                    "El proceso puede tardar varios minutos dependiendo del tamaño del modelo.\n\n" +
                    "¿Continuar?",
                    "Confirmar Sincronizaci?n Completa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                
                if (result != DialogResult.Yes)
                {
                    return;
                }
                
                this.Cursor = Cursors.WaitCursor;
                btnExecuteSelected.Enabled = false;
                btnExecuteAll.Enabled = false;
                txtReport.Text = "Iniciando sincronizaci?n de TODO el modelo...\r\n\r\n" +
                                 "Este proceso se ejecutar? en LOTES para mantener la estabilidad.\r\n" +
                                 "Por favor espera...\r\n\r\n";
                Application.DoEvents();

                // Ejecutar sincronizaci?n de todo el modelo en lotes
                bool success = _synchronizer.ExecuteAllModelInBatches(
                    (progress) => UpdateProgress(progress)
                );

                // Mostrar reporte
                if (success || _synchronizer.Report.AssembliesProcessed > 0)
                {
                    txtReport.Text = _synchronizer.Report.GenerateReport();
                    
                    if (_synchronizer.Report.IsSuccessful)
                    {
                        MessageBox.Show(
                            _synchronizer.Report.GenerateSummary(),
                            "Sincronizaci?n Completa Exitosa",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else if (_synchronizer.Report.HasErrors)
                    {
                        MessageBox.Show(
                            "La sincronizaci?n finaliz? con errores. Revisa el reporte para m?s detalles.",
                            "Sincronizaci?n con Errores",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }
                else
                {
                    txtReport.Text = "======================================\r\n";
                    txtReport.Text += "  SINCRONIZACI?N NO REALIZADA\r\n";
                    txtReport.Text += "======================================\r\n\r\n";
                    
                    if (_synchronizer.Report.HasWarnings)
                    {
                        txtReport.Text += "ADVERTENCIAS:\r\n";
                        foreach (var warning in _synchronizer.Report.GetWarnings())
                        {
                            txtReport.Text += $"  - {warning}\r\n";
                        }
                        txtReport.Text += "\r\n";
                    }
                    
                    if (_synchronizer.Report.HasErrors)
                    {
                        txtReport.Text += "ERRORES:\r\n";
                        foreach (var error in _synchronizer.Report.GetErrors())
                        {
                            txtReport.Text += $"  - {error}\r\n";
                        }
                        txtReport.Text += "\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}\n\n{ex.StackTrace}",
                    "Error Cr?tico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                txtReport.Text = $"ERROR CR?TICO:\r\n{ex.Message}\r\n\r\n{ex.StackTrace}";
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnExecuteSelected.Enabled = true;
                btnExecuteAll.Enabled = true;
            }
        }
        
        /// <summary>
        /// Actualiza el progreso en el reporte durante el procesamiento por lotes.
        /// </summary>
        private void UpdateProgress(string progress)
        {
            txtReport.AppendText(progress + "\r\n");
            txtReport.SelectionStart = txtReport.Text.Length;
            txtReport.ScrollToCaret();
            Application.DoEvents();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            // Detener el timer antes de cerrar
            if (_windowSyncTimer != null)
            {
                _windowSyncTimer.Stop();
                _windowSyncTimer.Dispose();
            }
            this.Close();
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Limpiar recursos
            if (_windowSyncTimer != null)
            {
                _windowSyncTimer.Stop();
                _windowSyncTimer.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}

