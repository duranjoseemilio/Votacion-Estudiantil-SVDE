using SDVE.Models;

namespace SDVE.UI;

/// <summary>Pantalla 2: papeleta dinámica con un PanelPuesto por convocatoria elegida.</summary>
public class FormPapeleta : Form
{
    private readonly List<PanelPuesto> _paneles = new();
    private readonly FlowLayoutPanel _flowPaneles;

    public List<VotoEmitido> VotosConfirmados { get; private set; }

    public FormPapeleta(IEnumerable<Convocatoria> seleccion, IEnumerable<Candidato> todosLosCandidatos)
    {
        Text = "SDVE · Papeleta electrónica";
        ClientSize = new Size(560, 700);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Tema.Fondo;
        Font = Tema.Normal;

        _flowPaneles = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false,
            AutoScroll = true, Padding = new Padding(58, 22, 20, 10), BackColor = Tema.Fondo
        };

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 84, BackColor = Color.White };
        var btnConfirmar = new Button { Text = "Confirmar voto ✓", Size = new Size(260, 50), Location = new Point(280, 17) };
        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(190, 50), Location = new Point(70, 17) };
        Tema.BotonPrimario(btnConfirmar);
        Tema.BotonSecundario(btnCancelar);
        btnConfirmar.Click += BtnConfirmar_Click;
        btnCancelar.Click += BtnCancelar_Click;
        footer.Controls.AddRange(new Control[] { btnCancelar, btnConfirmar });

        var header = new HeaderPanel
        {
            Dock = DockStyle.Top, Height = 100,
            Titulo = "Tu papeleta",
            Subtitulo = "Revisa bien antes de confirmar"
        };

        // Orden: Fill primero, luego Bottom y Top (así el Fill respeta los bordes)
        Controls.Add(_flowPaneles);
        Controls.Add(footer);
        Controls.Add(header);

        var candidatos = todosLosCandidatos.ToList();
        foreach (var conv in seleccion)
        {
            var panel = new PanelPuesto(conv, candidatos.Where(c => c.Puesto == conv));
            _paneles.Add(panel);
            _flowPaneles.Controls.Add(panel);
        }
    }

    private void BtnConfirmar_Click(object sender, EventArgs e)
    {
        PanelPuesto primerError = null;
        foreach (var p in _paneles)
        {
            bool ok = p.Validar(out var msg);
            p.MarcarError(!ok, msg);
            if (!ok && primerError == null) primerError = p;
        }

        if (primerError != null)
        {
            _flowPaneles.ScrollControlIntoView(primerError);
            return;
        }

        var r = MessageBox.Show("Una vez confirmado, el voto no se puede cambiar.\n\n¿Deseas continuar?",
            "Confirmar voto", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (r != DialogResult.Yes) return;

        VotosConfirmados = _paneles.Select(p => p.ObtenerVoto()).ToList();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnCancelar_Click(object sender, EventArgs e)
    {
        var r = MessageBox.Show("Si sales ahora no se registrará ningún voto.\n\n¿Salir de la papeleta?",
            "Cancelar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (r == DialogResult.Yes)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
