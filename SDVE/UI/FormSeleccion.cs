using SDVE.Models;

namespace SDVE.UI;

/// <summary>Pantalla 1: el votante elige en qué convocatorias participa (1, 2 o las 3).</summary>
public class FormSeleccion : Form
{
    private readonly List<Candidato> _candidatos;
    private readonly List<TarjetaConvocatoria> _tarjetas = new();
    private readonly Button _btnContinuar;
    private readonly Label _lblHint;

    /// <summary>El módulo de conteo se engancha aquí para recibir los votos confirmados.</summary>
    public Action<List<VotoEmitido>> AlConfirmarVotos { get; set; }

    public FormSeleccion(IEnumerable<Candidato> candidatos)
    {
        _candidatos = candidatos.ToList();

        Text = "SDVE · Votación Estudiantil";
        ClientSize = new Size(560, 640);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Tema.Fondo;
        Font = Tema.Normal;

        var header = new HeaderPanel
        {
            Dock = DockStyle.Top, Height = 130,
            Titulo = "Tu voto cuenta",
            Subtitulo = "Sistema Digital de Votación Estudiantil"
        };

        var lblInstruccion = new Label
        {
            Text = "¿En qué convocatorias quieres votar?",
            Font = Tema.Subtitulo, ForeColor = Tema.Texto, AutoSize = true,
            Location = new Point(60, 152), BackColor = Color.Transparent
        };

        int y = 196;
        foreach (Convocatoria conv in Enum.GetValues<Convocatoria>())
        {
            var t = new TarjetaConvocatoria(conv) { Location = new Point(60, y) };
            t.SeleccionCambiada += (s, e) => ActualizarEstado();
            _tarjetas.Add(t);
            Controls.Add(t);
            y += 108;
        }

        _lblHint = new Label
        {
            AutoSize = true, Font = Tema.Pequena, ForeColor = Tema.TextoSuave,
            Location = new Point(62, y + 2), BackColor = Color.Transparent
        };

        _btnContinuar = new Button { Text = "Continuar  →", Size = new Size(440, 52), Location = new Point(60, y + 30) };
        Tema.BotonPrimario(_btnContinuar);
        _btnContinuar.Click += BtnContinuar_Click;

        Controls.AddRange(new Control[] { header, lblInstruccion, _lblHint, _btnContinuar });
        ActualizarEstado();
    }

    private void ActualizarEstado()
    {
        int n = _tarjetas.Count(t => t.Seleccionada);
        _btnContinuar.Enabled = n > 0;
        _lblHint.Text = n == 0
            ? "Selecciona al menos una convocatoria para continuar."
            : $"{n} convocatoria{(n == 1 ? "" : "s")} seleccionada{(n == 1 ? "" : "s")}.";
    }

    private void BtnContinuar_Click(object sender, EventArgs e)
    {
        var seleccion = _tarjetas.Where(t => t.Seleccionada).Select(t => t.Convocatoria).ToList();
        if (seleccion.Count == 0) return;

        using var papeleta = new FormPapeleta(seleccion, _candidatos);
        if (papeleta.ShowDialog(this) != DialogResult.OK) return;

        if (AlConfirmarVotos != null)
        {
            AlConfirmarVotos(papeleta.VotosConfirmados);
        }
        else
        {
            // Modo demo: resumen de lo votado (borrar cuando se conecte el motor de conteo)
            var resumen = string.Join("\n", papeleta.VotosConfirmados.Select(v =>
                $"• {v.Puesto.Titulo()}: " +
                (v.EsBlanco ? "voto en blanco"
                 : v.NombreNoRegistrado != null ? $"{v.NombreNoRegistrado} (no registrado)"
                 : $"candidato #{v.CandidatoId}")));
            MessageBox.Show(resumen, "¡Voto registrado!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Dejar lista la pantalla para el siguiente votante
        foreach (var t in _tarjetas) t.Tag = null;
        ReiniciarSeleccion();
    }

    private void ReiniciarSeleccion()
    {
        var actuales = _tarjetas.ToList();
        foreach (var t in actuales) Controls.Remove(t);
        _tarjetas.Clear();

        int y = 196;
        foreach (Convocatoria conv in Enum.GetValues<Convocatoria>())
        {
            var t = new TarjetaConvocatoria(conv) { Location = new Point(60, y) };
            t.SeleccionCambiada += (s, e) => ActualizarEstado();
            _tarjetas.Add(t);
            Controls.Add(t);
            y += 108;
        }
        ActualizarEstado();
    }
}
