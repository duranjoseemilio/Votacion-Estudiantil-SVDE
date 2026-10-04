using System.Drawing;

namespace SDVE.Models;

public enum Convocatoria { SociedadAlumnos, ConsejoUniversitario, ConsejoRepresentantes }

public class Candidato
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public Convocatoria Puesto { get; set; }
}

public class VotoEmitido
{
    public Convocatoria Puesto { get; set; }
    public int? CandidatoId { get; set; }          // null si es no registrado o en blanco
    public string NombreNoRegistrado { get; set; } // null si es registrado o en blanco
    public bool EsBlanco { get; set; }
}

/// <summary>Textos y color de acento de cada convocatoria (solo presentación).</summary>
public static class ConvocatoriaInfo
{
    public static string Titulo(this Convocatoria c) => c switch
    {
        Convocatoria.SociedadAlumnos => "Sociedad de Alumnos",
        Convocatoria.ConsejoUniversitario => "Consejo Universitario",
        _ => "Consejo de Representantes"
    };

    public static string Descripcion(this Convocatoria c) => c switch
    {
        Convocatoria.SociedadAlumnos => "Representación estudiantil de tu centro",
        Convocatoria.ConsejoUniversitario => "Órgano máximo de gobierno universitario",
        _ => "Representantes por grupo y carrera"
    };

    public static Color Acento(this Convocatoria c) => c switch
    {
        Convocatoria.SociedadAlumnos => Color.FromArgb(79, 70, 229),   // índigo
        Convocatoria.ConsejoUniversitario => Color.FromArgb(13, 148, 136), // turquesa
        _ => Color.FromArgb(219, 39, 119)                               // rosa
    };
}
