using Microsoft.Data.Sqlite;
using SDVE.Models;

namespace SDVE;

public static class RepositorioVotacion
{
    public static string BuscarAlumno(int idAlumno)
    {
        using var con = BaseDatos.Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Nombre FROM Alumno WHERE IdAlumno = $id";
        cmd.Parameters.AddWithValue("$id", idAlumno);
        return cmd.ExecuteScalar() as string;
    }

    public static List<Convocatoria> ConvocatoriasDisponibles(int idAlumno)
    {
        var activas = new HashSet<string>();
        var votadas = new HashSet<string>();

        using var con = BaseDatos.Abrir();
        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = "SELECT Nombre FROM Convocatoria WHERE Activa = 1";
            using var r = cmd.ExecuteReader();
            while (r.Read()) activas.Add(r.GetString(0));
        }
        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = "SELECT Convocatoria FROM Voto WHERE IdAlumno = $id";
            cmd.Parameters.AddWithValue("$id", idAlumno);
            using var r = cmd.ExecuteReader();
            while (r.Read()) votadas.Add(r.GetString(0));
        }

        return Enum.GetValues<Convocatoria>()
            .Where(c => activas.Contains(c.Titulo()) && !votadas.Contains(c.Titulo()))
            .ToList();
    }

    public static List<Candidato> ObtenerCandidatos()
    {
        var porNombre = Enum.GetValues<Convocatoria>().ToDictionary(c => c.Titulo());
        var lista = new List<Candidato>();

        using var con = BaseDatos.Abrir();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT IdCandidato, Nombre, Convocatoria FROM Candidato " +
                          "WHERE EsRegistrado = 1 ORDER BY Convocatoria, Nombre";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            if (!porNombre.TryGetValue(r.GetString(2), out var conv)) continue;
            lista.Add(new Candidato { Id = (int)r.GetInt64(0), Nombre = r.GetString(1), Puesto = conv });
        }
        return lista;
    }

    public static void GuardarVotos(int idAlumno, List<VotoEmitido> votos)
    {
        using var con = BaseDatos.Abrir();
        using var tx = con.BeginTransaction();

        foreach (var v in votos)
        {
            string convocatoria = v.Puesto.Titulo();
            object idCandidato = DBNull.Value;

            if (v.CandidatoId != null)
                idCandidato = v.CandidatoId.Value;
            else if (!string.IsNullOrWhiteSpace(v.NombreNoRegistrado))
                idCandidato = ObtenerOCrearNoRegistrado(con, convocatoria, v.NombreNoRegistrado.Trim());

            using var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO Voto (IdAlumno, Convocatoria, IdCandidato) VALUES ($a, $c, $k)";
            cmd.Parameters.AddWithValue("$a", idAlumno);
            cmd.Parameters.AddWithValue("$c", convocatoria);
            cmd.Parameters.AddWithValue("$k", idCandidato);
            cmd.ExecuteNonQuery();
        }

        tx.Commit();
    }

    private static long ObtenerOCrearNoRegistrado(SqliteConnection con, string convocatoria, string nombre)
    {
        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = "SELECT IdCandidato, Nombre FROM Candidato WHERE Convocatoria = $c";
            cmd.Parameters.AddWithValue("$c", convocatoria);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                if (string.Equals(r.GetString(1), nombre, StringComparison.CurrentCultureIgnoreCase))
                    return r.GetInt64(0);
            }
        }

        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO Candidato (Nombre, Convocatoria, EsRegistrado) VALUES ($n, $c, 0); " +
                              "SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$n", nombre);
            cmd.Parameters.AddWithValue("$c", convocatoria);
            return (long)cmd.ExecuteScalar();
        }
    }
}
