using System.Drawing.Drawing2D;

namespace SDVE.UI;

/// <summary>Encabezado con degradado índigo → violeta y texto blanco.</summary>
public class HeaderPanel : Panel
{
    public string Titulo { get; set; } = "";
    public string Subtitulo { get; set; } = "";

    public HeaderPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        using var br = new LinearGradientBrush(ClientRectangle, Tema.Primario, Tema.Violeta, 20f);
        e.Graphics.FillRectangle(br, ClientRectangle);

        // círculos decorativos
        using var deco = new SolidBrush(Color.FromArgb(28, 255, 255, 255));
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.FillEllipse(deco, Width - 120, -60, 190, 190);
        e.Graphics.FillEllipse(deco, Width - 200, Height - 50, 110, 110);

        TextRenderer.DrawText(e.Graphics, Titulo, Tema.Titulo, new Point(32, Height / 2 - 34), Color.White);
        TextRenderer.DrawText(e.Graphics, Subtitulo, Tema.Normal, new Point(35, Height / 2 + 12),
            Color.FromArgb(225, 222, 255));
    }
}
