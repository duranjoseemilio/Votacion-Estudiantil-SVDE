using SDVE.UI;

namespace SDVE;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) =>
            MessageBox.Show("Ocurrió un error inesperado:\n" + e.Exception.Message,
                "SDVE", MessageBoxButtons.OK, MessageBoxIcon.Error);

        try
        {
            BaseDatos.Preparar();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo abrir la base de datos:\n" + ex.Message,
                "SDVE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new FormInicio());
    }
}
