namespace SDVE.UI;

public class FormIdentificacion : Form
{
    private readonly TextBox _txtMatricula;
    private readonly Label _lblError;

    public int IdAlumno { get; private set; }
    public string NombreAlumno { get; private set; }

    public FormIdentificacion()
    {
        Text = "SDVE · Identificación";
        ClientSize = new Size(560, 360);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Tema.Fondo;
        Font = Tema.Normal;

        var header = new HeaderPanel
        {
            Dock = DockStyle.Top, Height = 100,
            Titulo = "Identifícate",
            Subtitulo = "Escribe tu matrícula para votar"
        };

        var lbl = new Label
        {
            Text = "Matrícula", Font = Tema.Subtitulo, ForeColor = Tema.Texto, AutoSize = true,
            Location = new Point(60, 130), BackColor = Color.Transparent
        };

        _txtMatricula = new TextBox
        {
            Location = new Point(60, 168), Width = 440, MaxLength = 9,
            Font = Tema.Subtitulo, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Escribe tu matrícula (solo números)"
        };

        _lblError = new Label
        {
            AutoSize = true, Font = Tema.Pequena, ForeColor = Tema.Error,
            Location = new Point(62, 210), BackColor = Color.Transparent
        };

        var btnCancelar = new Button { Text = "Cancelar", Size = new Size(190, 50), Location = new Point(60, 270) };
        var btnContinuar = new Button { Text = "Continuar  →", Size = new Size(230, 50), Location = new Point(270, 270) };
        Tema.BotonSecundario(btnCancelar);
        Tema.BotonPrimario(btnContinuar);
        btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        btnContinuar.Click += (s, e) => Continuar();
        _txtMatricula.TextChanged += (s, e) => _lblError.Text = "";

        AcceptButton = btnContinuar;
        CancelButton = btnCancelar;
        Controls.AddRange(new Control[] { header, lbl, _txtMatricula, _lblError, btnCancelar, btnContinuar });
    }

    private void Continuar()
    {
        if (!int.TryParse(_txtMatricula.Text.Trim(), out int id) || id <= 0)
        {
            _lblError.Text = "⚠  La matrícula solo debe contener números.";
            return;
        }

        string nombre;
        try
        {
            nombre = RepositorioVotacion.BuscarAlumno(id);
        }
        catch (Exception ex)
        {
            FormInicio.MostrarErrorBD("consultar el padrón", ex);
            return;
        }

        if (nombre == null)
        {
            _lblError.Text = "⚠  Esa matrícula no está en el padrón.";
            return;
        }

        IdAlumno = id;
        NombreAlumno = nombre;
        DialogResult = DialogResult.OK;
        Close();
    }
}
