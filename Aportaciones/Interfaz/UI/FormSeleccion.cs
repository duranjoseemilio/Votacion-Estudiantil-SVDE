using SDVE.Models;

namespace SDVE.UI;

/// <summary>Pantalla 1: el votante elige en qué convocatorias participa (1, 2 o las 3).</summary>
public class FormSeleccion : Form
{
    private readonly List<Candidato> _candidatos;
    private readonly List<TarjetaConvocatoria> _tarjetas = new();
    private readonly Button _btnContinuar;
    private readonly Label _lblHint;

    public List<VotoEmitido> VotosConfirmados { get; private set; }

    public FormSeleccion(string nombreVotante, IEnumerable<Convocatoria> disponibles, IEnumerable<Candidato> candidatos)
    {
        _candidatos = candidatos.ToList();

        Text = "SDVE · Votación Estudiantil";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Tema.Fondo;
        Font = Tema.Normal;

        var header = new HeaderPanel
        {
            Dock = DockStyle.Top, Height = 130,
            Titulo = "Tu voto cuenta",
            Subtitulo = "Votante: " + nombreVotante
        };

        var lblInstruccion = new Label
        {
            Text = "¿En qué convocatorias quieres votar?",
            Font = Tema.Subtitulo, ForeColor = Tema.Texto, AutoSize = true,
            Location = new Point(60, 152), BackColor = Color.Transparent
        };

        int y = 196;
        foreach (Convocatoria conv in disponibles)
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

        ClientSize = new Size(560, y + 30 + 52 + 30);
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

        VotosConfirmados = papeleta.VotosConfirmados;
        DialogResult = DialogResult.OK;
        Close();
    }
}
