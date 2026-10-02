using System.Drawing.Drawing2D;

namespace SDVE.UI;

/// <summary>Paleta, tipografía y helpers de estilo. Cambia aquí y se actualiza toda la app.</summary>
public static class Tema
{
    // ---- Colores ----
    public static readonly Color Primario      = Color.FromArgb(79, 70, 229);
    public static readonly Color PrimarioHover = Color.FromArgb(67, 56, 202);
    public static readonly Color Violeta       = Color.FromArgb(124, 58, 237);
    public static readonly Color Fondo         = Color.FromArgb(245, 243, 255);
    public static readonly Color Tarjeta       = Color.White;
    public static readonly Color Borde         = Color.FromArgb(221, 214, 254);
    public static readonly Color Texto         = Color.FromArgb(30, 27, 75);
    public static readonly Color TextoSuave    = Color.FromArgb(100, 100, 140);
    public static readonly Color Error         = Color.FromArgb(225, 29, 72);
    public static readonly Color ErrorFondo    = Color.FromArgb(255, 241, 242);
    public static readonly Color Deshabilitado = Color.FromArgb(196, 192, 224);

    // ---- Tipografía ----
    // Usa Segoe UI Variable si existe (Windows 11), si no Segoe UI.
    private static readonly string Familia = FamiliaDisponible("Segoe UI Variable Text", "Segoe UI");
    private static readonly string FamiliaTitulo = FamiliaDisponible("Segoe UI Variable Display", "Segoe UI Semibold", "Segoe UI");

    public static readonly Font Normal    = new(Familia, 10.5f);
    public static readonly Font Negrita   = new(Familia, 10.5f, FontStyle.Bold);
    public static readonly Font Pequena   = new(Familia, 9f);
    public static readonly Font Subtitulo = new(FamiliaTitulo, 13f, FontStyle.Bold);
    public static readonly Font Titulo    = new(FamiliaTitulo, 22f, FontStyle.Bold);
    public static readonly Font Boton     = new(FamiliaTitulo, 11.5f, FontStyle.Bold);

    private static string FamiliaDisponible(params string[] opciones)
    {
        var instaladas = FontFamily.Families.Select(f => f.Name).ToHashSet();
        return opciones.FirstOrDefault(instaladas.Contains) ?? "Segoe UI";
    }

    // ---- Helpers ----
    public static GraphicsPath RectRedondeado(Rectangle r, int radio)
    {
        int d = radio * 2;
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    /// <summary>Mezcla un color con blanco (f = 0 blanco, 1 color original).</summary>
    public static Color Aclarar(Color c, double f) => Color.FromArgb(
        (int)(255 - (255 - c.R) * f), (int)(255 - (255 - c.G) * f), (int)(255 - (255 - c.B) * f));

    public static void BotonPrimario(Button b)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.Font = Boton;
        b.ForeColor = Color.White;
        b.Cursor = Cursors.Hand;
        Refrescar();
        b.MouseEnter += (s, e) => { if (b.Enabled) b.BackColor = PrimarioHover; };
        b.MouseLeave += (s, e) => Refrescar();
        b.EnabledChanged += (s, e) => Refrescar();

        void Refrescar()
        {
            b.BackColor = b.Enabled ? Primario : Deshabilitado;
            b.Cursor = b.Enabled ? Cursors.Hand : Cursors.Default;
        }
    }

    public static void BotonSecundario(Button b)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderColor = Borde;
        b.FlatAppearance.BorderSize = 2;
        b.FlatAppearance.MouseOverBackColor = Aclarar(Primario, 0.08);
        b.BackColor = Color.White;
        b.ForeColor = Primario;
        b.Font = Boton;
        b.Cursor = Cursors.Hand;
    }
}
