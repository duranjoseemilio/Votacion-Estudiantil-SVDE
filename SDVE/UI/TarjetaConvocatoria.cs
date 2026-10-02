using System.Drawing.Drawing2D;
using SDVE.Models;

namespace SDVE.UI;

/// <summary>Tarjeta clicable con CheckBox para elegir una convocatoria.</summary>
public class TarjetaConvocatoria : Panel
{
    private readonly CheckBox _chk = new() { AutoSize = true, BackColor = Color.Transparent, Cursor = Cursors.Hand };
    private readonly Label _lblTitulo = new() { AutoSize = true, BackColor = Color.Transparent, Font = Tema.Subtitulo, Cursor = Cursors.Hand };
    private readonly Label _lblDesc = new() { AutoSize = true, BackColor = Color.Transparent, Font = Tema.Pequena, ForeColor = Tema.TextoSuave, Cursor = Cursors.Hand };
    private readonly Color _acento;

    public Convocatoria Convocatoria { get; }
    public bool Seleccionada => _chk.Checked;
    public event EventHandler SeleccionCambiada;

    public TarjetaConvocatoria(Convocatoria conv)
    {
        Convocatoria = conv;
        _acento = conv.Acento();
        Size = new Size(440, 92);
        BackColor = Tema.Fondo;
        DoubleBuffered = true;
        Cursor = Cursors.Hand;

        _lblTitulo.Text = conv.Titulo();
        _lblTitulo.ForeColor = Tema.Texto;
        _lblDesc.Text = conv.Descripcion();

        _chk.Location = new Point(26, 37);
        _lblTitulo.Location = new Point(62, 20);
        _lblDesc.Location = new Point(64, 52);
        Controls.AddRange(new Control[] { _chk, _lblTitulo, _lblDesc });

        void Alternar(object s, EventArgs e) => _chk.Checked = !_chk.Checked;
        Click += Alternar;
        _lblTitulo.Click += Alternar;
        _lblDesc.Click += Alternar;

        _chk.CheckedChanged += (s, e) =>
        {
            Invalidate();
            SeleccionCambiada?.Invoke(this, EventArgs.Empty);
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        using var path = Tema.RectRedondeado(rect, 14);

        using (var fill = new SolidBrush(_chk.Checked ? Tema.Aclarar(_acento, 0.09) : Tema.Tarjeta))
            g.FillPath(fill, path);

        // barra de color a la izquierda
        g.SetClip(path);
        using (var barra = new SolidBrush(_chk.Checked ? _acento : Tema.Borde))
            g.FillRectangle(barra, 0, 0, 8, Height);
        g.ResetClip();

        using var pen = new Pen(_chk.Checked ? _acento : Tema.Borde, _chk.Checked ? 2f : 1.5f);
        g.DrawPath(pen, path);
    }
}
