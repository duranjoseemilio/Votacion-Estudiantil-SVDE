using Microsoft.Data.Sqlite;
using SDVE.Models;

namespace SDVE.UI;

public class FormInicio : Form
{
    public FormInicio()
    {
        Text = "SDVE · Votación Estudiantil";
        ClientSize = new Size(560, 400);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Tema.Fondo;
        Font = Tema.Normal;

        var header = new HeaderPanel
        {
            Dock = DockStyle.Top, Height = 130,
            Titulo = "Tu voto cuenta",
            Subtitulo = "Sistema Digital de Votación Estudiantil"
        };

        var btnVotar = new Button { Text = "Votar  →", Size = new Size(440, 56), Location = new Point(60, 180) };
        var btnResultados = new Button { Text = "Ver resultados", Size = new Size(440, 56), Location = new Point(60, 256) };
        Tema.BotonPrimario(btnVotar);
        Tema.BotonSecundario(btnResultados);
        btnVotar.Click += (s, e) => Votar();
        btnResultados.Click += (s, e) => VerResultados();

        Controls.AddRange(new Control[] { header, btnVotar, btnResultados });
    }

    public static void MostrarErrorBD(string accion, Exception ex)
    {
        MessageBox.Show($"No se pudo {accion}.\n\nDetalle: {ex.Message}\n\n" +
                        "Revisa que la base de datos no esté abierta en otro programa e inténtalo de nuevo.",
            "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void VerResultados()
    {
        FormResultados f;
        try
        {
            f = new FormResultados();
        }
        catch (Exception ex)
        {
            MostrarErrorBD("cargar los resultados", ex);
            return;
        }

        using (f) f.ShowDialog(this);
    }

    private void Votar()
    {
        using var ident = new FormIdentificacion();
        if (ident.ShowDialog(this) != DialogResult.OK) return;

        List<Convocatoria> disponibles;
        List<Candidato> candidatos;
        try
        {
            disponibles = RepositorioVotacion.ConvocatoriasDisponibles(ident.IdAlumno);
            candidatos = RepositorioVotacion.ObtenerCandidatos();
        }
        catch (Exception ex)
        {
            MostrarErrorBD("cargar las convocatorias y los candidatos", ex);
            return;
        }

        if (disponibles.Count == 0)
        {
            MessageBox.Show(ident.NombreAlumno + " ya votó en todas las convocatorias activas.",
                "Sin convocatorias pendientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var seleccion = new FormSeleccion(ident.NombreAlumno, disponibles, candidatos);
        if (seleccion.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            RepositorioVotacion.GuardarVotos(ident.IdAlumno, seleccion.VotosConfirmados);
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 1555)
        {
            MessageBox.Show("Este alumno ya tiene un voto registrado en alguna de las convocatorias elegidas.\n" +
                            "No se guardó ningún voto.", "Voto rechazado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        catch (Exception ex)
        {
            MostrarErrorBD("guardar el voto. No se registró ningún voto", ex);
            return;
        }

        var resumen = string.Join("\n", seleccion.VotosConfirmados.Select(v => "• " + v.Puesto.Titulo()));
        MessageBox.Show("Se registró tu voto en:\n" + resumen, "¡Voto registrado!",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
