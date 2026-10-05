using System;
using System.Windows.Forms;

namespace SDVE
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Aquí arranca el programa: abre la ventana de resultados (módulo 3)
            Application.Run(new FormResultados());
        }
    }
}
