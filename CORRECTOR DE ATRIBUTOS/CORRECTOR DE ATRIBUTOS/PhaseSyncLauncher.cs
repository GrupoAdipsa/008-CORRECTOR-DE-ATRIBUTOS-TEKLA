using System;
using System.Windows.Forms;

namespace CORRECTOR_DE_ATRIBUTOS
{
    /// <summary>
    /// Clase de lanzamiento para la herramienta de sincronización de Phase.
    /// Úsala desde Tekla Applications & Components o como aplicación externa.
    /// </summary>
    public class PhaseSyncLauncher
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            using (PhaseSyncForm form = new PhaseSyncForm())
            {
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Método alternativo para invocar desde otras partes del código.
        /// </summary>
        public static void Launch()
        {
            using (PhaseSyncForm form = new PhaseSyncForm())
            {
                form.ShowDialog();
            }
        }
    }
}

