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

/// <summary>Datos de ejemplo para probar la interfaz. Tus compañeros los reemplazan por la carga real.</summary>
public static class CandidatosDemo
{
    public static List<Candidato> Obtener() => new()
    {
        new() { Id = 1, Puesto = Convocatoria.SociedadAlumnos, Nombre = "Valeria Montoya Ruiz" },
        new() { Id = 2, Puesto = Convocatoria.SociedadAlumnos, Nombre = "Diego Hernández Ortiz" },
        new() { Id = 3, Puesto = Convocatoria.SociedadAlumnos, Nombre = "Camila Soto Vargas" },

        new() { Id = 4, Puesto = Convocatoria.ConsejoUniversitario, Nombre = "Andrés Reyes Palacios" },
        new() { Id = 5, Puesto = Convocatoria.ConsejoUniversitario, Nombre = "Mariana Lozano Cruz" },

        new() { Id = 6, Puesto = Convocatoria.ConsejoRepresentantes, Nombre = "Luis Ángel Ramírez" },
        new() { Id = 7, Puesto = Convocatoria.ConsejoRepresentantes, Nombre = "Sofía Gutiérrez Mena" },
        new() { Id = 8, Puesto = Convocatoria.ConsejoRepresentantes, Nombre = "Emiliano Torres Ávila" },
    };
}
