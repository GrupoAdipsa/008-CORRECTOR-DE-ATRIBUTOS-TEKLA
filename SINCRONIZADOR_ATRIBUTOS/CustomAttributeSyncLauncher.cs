using System;
using System.Windows.Forms;

namespace SincronizadorAtributos
{
    /// <summary>
    /// Punto de entrada de la aplicación.
    /// Inicializa y muestra el formulario principal.
    /// </summary>
    class CustomAttributeSyncLauncher
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                CustomAttributeSyncForm form = new CustomAttributeSyncForm();
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al iniciar la aplicación:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Error Crítico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
