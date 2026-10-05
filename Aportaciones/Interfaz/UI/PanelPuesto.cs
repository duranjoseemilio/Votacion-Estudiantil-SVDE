using System.Drawing.Drawing2D;
using SDVE.Models;

namespace SDVE.UI;

/// <summary>Tarjeta de votación de UN puesto: candidatos oficiales, "otro" y voto en blanco.</summary>
public class PanelPuesto : Panel
{
    private readonly FlowLayoutPanel _flow;
    private readonly Label _lblError;
    private readonly RadioButton _rbOtro;
    private readonly RadioButton _rbBlanco;
    private readonly TextBox _txtOtro;
    private readonly Dictionary<RadioButton, Candidato> _mapa = new();
    private readonly Color _acento;
    private bool _hayError;

    public Convocatoria Puesto { get; }

    public PanelPuesto(Convocatoria puesto, IEnumerable<Candidato> candidatos)
    {
        Puesto = puesto;
        _acento = puesto.Acento();

        DoubleBuffered = true;
        ResizeRedraw = true;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        MinimumSize = new Size(440, 0);
        Padding = new Padding(28, 18, 18, 18);
        Margin = new Padding(0, 0, 0, 18);
        BackColor = Tema.Fondo;

        _flow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Location = new Point(Padding.Left, Padding.Top)
        };

        _flow.Controls.Add(new Label
        {
            Text = puesto.Titulo(), Font = Tema.Subtitulo, ForeColor = _acento,
            AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 0)
        });
        _flow.Controls.Add(new Label
        {
            Text = "Elige una sola opción", Font = Tema.Pequena, ForeColor = Tema.TextoSuave,
            AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(2, 0, 0, 8)
        });

        foreach (var c in candidatos)
        {
            var rb = CrearRadio(c.Nombre);
            _mapa[rb] = c;
            _flow.Controls.Add(rb);
        }

        _rbOtro = CrearRadio("Otro (candidato no registrado)");
        _txtOtro = new TextBox
        {
            Width = 340, Font = Tema.Normal, Enabled = false, MaxLength = 60,
            BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Nombre y apellido del candidato",
            Margin = new Padding(26, 0, 0, 8)
        };
        _rbBlanco = CrearRadio("Voto en blanco");

        _lblError = new Label
        {
            AutoSize = true, MaximumSize = new Size(370, 0), Visible = false,
            Font = Tema.Pequena, ForeColor = Tema.Error, BackColor = Color.Transparent,
            Margin = new Padding(2, 6, 0, 0)
        };

        _flow.Controls.Add(_rbOtro);
        _flow.Controls.Add(_txtOtro);
        _flow.Controls.Add(_rbBlanco);
        _flow.Controls.Add(_lblError);
        Controls.Add(_flow);

        _rbOtro.CheckedChanged += (s, e) =>
        {
            _txtOtro.Enabled = _rbOtro.Checked;
            if (_rbOtro.Checked) _txtOtro.Focus(); else _txtOtro.Clear();
        };
        _txtOtro.TextChanged += (s, e) => LimpiarError();
    }

    private RadioButton CrearRadio(string texto)
    {
        var rb = new RadioButton
        {
            Text = texto, AutoSize = true, Font = Tema.Normal, ForeColor = Tema.Texto,
            BackColor = Color.Transparent, Cursor = Cursors.Hand, Margin = new Padding(0, 5, 0, 5)
        };
        rb.CheckedChanged += (s, e) => { if (rb.Checked) LimpiarError(); };
        return rb;
    }

    private void LimpiarError()
    {
        if (_hayError) MarcarError(false);
    }

    // ---------- Validación ----------
    public bool Validar(out string error)
    {
        error = null;
        bool hayOpcion = _mapa.Keys.Any(r => r.Checked) || _rbOtro.Checked || _rbBlanco.Checked;
        if (!hayOpcion)
        {
            error = $"Selecciona una opción en {Puesto.Titulo()}.";
            return false;
        }

        if (_rbOtro.Checked)
        {
            var nombre = _txtOtro.Text.Trim();
            if (nombre.Length < 3)
            {
                error = $"Escribe el nombre completo del candidato en {Puesto.Titulo()} (mín. 3 caracteres).";
                return false;
            }
            if (!nombre.All(ch => char.IsLetter(ch) || ch == ' ' || ch == '.' || ch == '-' || ch == '\''))
            {
                error = $"El nombre en {Puesto.Titulo()} solo puede contener letras.";
                return false;
            }
            if (_mapa.Values.Any(c => string.Equals(c.Nombre, nombre, StringComparison.OrdinalIgnoreCase)))
            {
                error = $"\"{nombre}\" ya está en la lista oficial de {Puesto.Titulo()}; elígelo ahí.";
                return false;
            }
        }
        return true;
    }

    public VotoEmitido ObtenerVoto()
    {
        var voto = new VotoEmitido { Puesto = Puesto };
        var elegido = _mapa.FirstOrDefault(kv => kv.Key.Checked);

        if (elegido.Key != null) voto.CandidatoId = elegido.Value.Id;
        else if (_rbOtro.Checked) voto.NombreNoRegistrado = _txtOtro.Text.Trim();
        else voto.EsBlanco = true;
        return voto;
    }

    /// <summary>Resalta la tarjeta en rojo y muestra el mensaje debajo de las opciones.</summary>
    public void MarcarError(bool hayError, string mensaje = null)
    {
        _hayError = hayError;
        _lblError.Text = hayError ? "⚠  " + mensaje : "";
        _lblError.Visible = hayError;
        Invalidate();
    }

    // ---------- Dibujo ----------
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        using var path = Tema.RectRedondeado(rect, 16);

        using (var fill = new SolidBrush(_hayError ? Tema.ErrorFondo : Tema.Tarjeta))
            g.FillPath(fill, path);

        g.SetClip(path);
        using (var barra = new SolidBrush(_hayError ? Tema.Error : _acento))
            g.FillRectangle(barra, 0, 0, 8, Height);
        g.ResetClip();

        using var pen = new Pen(_hayError ? Tema.Error : Tema.Borde, _hayError ? 2f : 1.5f);
        g.DrawPath(pen, path);
    }
}
