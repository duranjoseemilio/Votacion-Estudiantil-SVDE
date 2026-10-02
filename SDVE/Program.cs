using SDVE.Models;
using SDVE.UI;

namespace SDVE;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // TODO (equipo): reemplazar CandidatosDemo por la carga real (CSV/JSON/BD)
        var candidatos = CandidatosDemo.Obtener();

        Application.Run(new FormSeleccion(candidatos));
    }
}
