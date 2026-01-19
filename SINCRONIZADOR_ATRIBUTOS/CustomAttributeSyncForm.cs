using System;
using System.Drawing;
using System.Windows.Forms;

namespace SincronizadorAtributos
{
    /// <summary>
    /// Formulario principal para sincronizar atributos personalizados.
    /// Permite al usuario elegir entre procesar objetos seleccionados o todo el modelo.
    /// </summary>
    public class CustomAttributeSyncForm : Form
    {
        private Button btnSyncSelected;
        private Button btnSyncAll;
        private Button btnDiagnostic;
        private Button btnClose;
        private Label lblTitle;
        private Label lblDescription;
        private Panel pnlButtons;
        private TextBox txtInfo;

        public CustomAttributeSyncForm()
        {
            InitializeComponents();
            ConfigureFormPosition();
        }

        private void InitializeComponents()
        {
            // Configuración del formulario
            this.Text = "Sincronizador de Atributos - ESTATUS_PIEZA y PRIORIDAD";
            this.Width = 500;
            this.Height = 400;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;

            // Título
            lblTitle = new Label();
            lblTitle.Text = "Sincronizador de Atributos Personalizados";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 40;
            lblTitle.BackColor = Color.FromArgb(0, 120, 215);
            lblTitle.ForeColor = Color.White;

            // Descripción
            lblDescription = new Label();
            lblDescription.Text = "Sincroniza ESTATUS_PIEZA y PRIORIDAD desde la Main Part\n" +
                                 "hacia todas las Secondary Parts y Bolts del Assembly.";
            lblDescription.Font = new Font("Segoe UI", 9);
            lblDescription.TextAlign = ContentAlignment.TopCenter;
            lblDescription.Dock = DockStyle.Top;
            lblDescription.Height = 60;
            lblDescription.Padding = new Padding(10, 10, 10, 0);

            // Información de atributos
            txtInfo = new TextBox();
            txtInfo.Multiline = true;
            txtInfo.ReadOnly = true;
            txtInfo.ScrollBars = ScrollBars.Vertical;
            txtInfo.Dock = DockStyle.Fill;
            txtInfo.Font = new Font("Consolas", 9);
            txtInfo.BackColor = Color.FromArgb(250, 250, 250);
            txtInfo.Text = GetAttributesInfo();

            // Panel de botones
            pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 80;
            pnlButtons.Padding = new Padding(10);


            // Botón: Sincronizar Seleccionados
            btnSyncSelected = new Button();
            btnSyncSelected.Text = "Procesar Objetos\nSeleccionados";
            btnSyncSelected.Width = 120;
            btnSyncSelected.Height = 60;
            btnSyncSelected.Left = 140;
            btnSyncSelected.Top = 10;
            btnSyncSelected.BackColor = Color.FromArgb(0, 120, 215);
            btnSyncSelected.ForeColor = Color.White;
            btnSyncSelected.FlatStyle = FlatStyle.Flat;
            btnSyncSelected.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnSyncSelected.Cursor = Cursors.Hand;
            btnSyncSelected.Click += BtnSyncSelected_Click;

            // Botón: Sincronizar Todo
            btnSyncAll = new Button();
            btnSyncAll.Text = "Sincronizar\nTodo el Modelo";
            btnSyncAll.Width = 120;
            btnSyncAll.Height = 60;
            btnSyncAll.Left = 270;
            btnSyncAll.Top = 10;
            btnSyncAll.BackColor = Color.FromArgb(16, 137, 62);
            btnSyncAll.ForeColor = Color.White;
            btnSyncAll.FlatStyle = FlatStyle.Flat;
            btnSyncAll.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnSyncAll.Cursor = Cursors.Hand;
            btnSyncAll.Click += BtnSyncAll_Click;

            // Botón: Cerrar
            btnClose = new Button();
            btnClose.Text = "Cerrar";
            btnClose.Width = 90;
            btnClose.Height = 60;
            btnClose.Left = 400;
            btnClose.Top = 10;
            btnClose.BackColor = Color.FromArgb(200, 200, 200);
            btnClose.ForeColor = Color.Black;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += BtnClose_Click;

            // Agregar controles al panel
            // No agregar botón de diagnóstico
            pnlButtons.Controls.Add(btnSyncSelected);
            pnlButtons.Controls.Add(btnSyncAll);
            pnlButtons.Controls.Add(btnClose);

            // Agregar controles al formulario
            this.Controls.Add(txtInfo);
            this.Controls.Add(lblDescription);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlButtons);
        }

        /// <summary>
        /// Configura la posición del formulario para que no tape Tekla.
        /// Posición: esquina superior derecha de la pantalla.
        /// </summary>
        private void ConfigureFormPosition()
        {
            // Obtener dimensiones de la pantalla
            Rectangle screenBounds = Screen.PrimaryScreen.WorkingArea;

            // Posicionar en esquina superior derecha
            int x = screenBounds.Right - this.Width - 20;
            int y = screenBounds.Top + 20;

            this.Location = new Point(x, y);
        }

        /// <summary>
        /// Obtiene información sobre los atributos que se sincronizan.
        /// </summary>
        private string GetAttributesInfo()
        {
            return "------------------------------\n" +
                   "  ATRIBUTOS A SINCRONIZAR\n" +
                   "------------------------------\n\n" +
                   "1. ESTATUS_PIEZA\n" +
                   "   Nombre: \"Estatus de Pieza:\"\n" +
                   "   Tipo: Option (lista de valores)\n" +
                   "   Valores posibles:\n" +
                   "     - (vacío)\n" +
                   "     - Programado\n" +
                   "     - Conectado\n" +
                   "     - Detallado\n" +
                   "     - Revisado\n" +
                   "     - Liberado\n\n" +
                   "2. PRIORIDAD\n" +
                   "   Nombre: \"PRIORIDAD DETALLADO:\"\n" +
                   "   Tipo: String (texto libre)\n\n" +
                   "------------------------------\n" +
                   "  FUNCIONAMIENTO\n" +
                   "------------------------------\n\n" +
                   "El sistema lee estos atributos de la Main Part\n" +
                   "de cada Assembly y los propaga a:\n\n" +
                   "   - Todas las Secondary Parts\n" +
                   "   - Todos los Bolts del Assembly\n\n" +
                   "------------------------------\n";
        }

        // ============================================================================
        // EVENTOS DE BOTONES
        // ============================================================================

        private void BtnDiagnostic_Click(object sender, EventArgs e)
        {
            try
            {
                AttributeDiagnostic.ShowAssemblyMapping();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error durante el mapeo:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnSyncSelected_Click(object sender, EventArgs e)
        {
            try
            {
                // Deshabilitar botones durante el proceso
                SetButtonsEnabled(false);

                // Crear sincronizador
                CustomAttributeSynchronizer synchronizer = new CustomAttributeSynchronizer();

                // Ejecutar sincronización interactiva
                bool success = synchronizer.ExecuteInteractive();

                if (success)
                {
                    // Mostrar reporte
                    CustomAttributeReport report = synchronizer.GetReport();
                    ShowReport(report);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error durante la sincronización:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Rehabilitar botones
                SetButtonsEnabled(true);
            }
        }

        private void BtnSyncAll_Click(object sender, EventArgs e)
        {
            try
            {
                // Confirmación del usuario
                var result = MessageBox.Show(
                    "¿Estás seguro de que deseas sincronizar TODO el modelo?\n\n" +
                    "Esto procesará todos los Assemblies del modelo.\n" +
                    "Esta operación puede tardar varios minutos en modelos grandes.",
                    "Confirmar Sincronización Total",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                // Deshabilitar botones durante el proceso
                SetButtonsEnabled(false);

                // Crear sincronizador
                CustomAttributeSynchronizer synchronizer = new CustomAttributeSynchronizer();

                // Ejecutar sincronización completa
                bool success = synchronizer.ExecuteOnAllModel();

                if (success)
                {
                    // Mostrar reporte
                    CustomAttributeReport report = synchronizer.GetReport();
                    ShowReport(report);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error durante la sincronización:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Rehabilitar botones
                SetButtonsEnabled(true);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ============================================================================
        // MÉTODOS AUXILIARES
        // ============================================================================

        private void SetButtonsEnabled(bool enabled)
        {
            btnSyncSelected.Enabled = enabled;
            btnSyncAll.Enabled = enabled;
            btnClose.Enabled = enabled;

            if (!enabled)
            {
                btnSyncSelected.Text = "Procesando...";
                btnSyncAll.Text = "Procesando...";
            }
            else
            {
                btnSyncSelected.Text = "Procesar Objetos\nSeleccionados";
                btnSyncAll.Text = "Sincronizar\nTodo el Modelo";
            }

            Application.DoEvents();
        }

        private void ShowReport(CustomAttributeReport report)
        {
            // Solo mostrar mensaje de éxito o error
            if (report != null)
            {
                string msg = report.GenerateShortSummary();
                if (msg.Contains("Errores") || msg.Contains("Advertencias"))
                {
                    MessageBox.Show(msg, "Sincronización finalizada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Sincronización completada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
