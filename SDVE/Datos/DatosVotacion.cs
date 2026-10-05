using System;
using System.Collections.Generic;

namespace SDVE
{
    public static class DatosVotacion
    {
        public static List<Alumno> ObtenerAlumnos()
        {
            BaseDatos.Preparar();
            List<Alumno> lista = new List<Alumno>();

            using (var con = BaseDatos.Abrir())
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT a.IdAlumno, a.Nombre, c.NombreCarrera || ' ' || a.LetraGrupo, c.NombreCarrera, c.Centro " +
                    "FROM Alumno a JOIN Carrera c ON c.NombreCarrera = a.NombreCarrera " +
                    "ORDER BY a.IdAlumno";

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Alumno a = new Alumno();
                        a.Matricula = r.GetInt64(0).ToString();
                        a.Nombre = r.GetString(1);
                        a.Grupo = r.GetString(2);
                        a.Carrera = r.GetString(3);
                        a.Centro = r.GetString(4);
                        lista.Add(a);
                    }
                }
            }
            return lista;
        }

        public static List<Voto> ObtenerVotos()
        {
            BaseDatos.Preparar();
            List<Voto> votos = new List<Voto>();

            using (var con = BaseDatos.Abrir())
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT v.IdAlumno, v.Convocatoria, c.Nombre, c.EsRegistrado " +
                    "FROM Voto v LEFT JOIN Candidato c ON c.IdCandidato = v.IdCandidato " +
                    "ORDER BY v.IdAlumno, v.Convocatoria";

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        Voto v = new Voto();
                        v.Matricula = r.GetInt64(0).ToString();
                        v.Convocatoria = r.GetString(1);
                        v.Candidato = r.IsDBNull(2) ? "" : r.GetString(2);
                        v.EsNoRegistrado = !r.IsDBNull(3) && r.GetInt64(3) == 0;
                        votos.Add(v);
                    }
                }
            }
            return votos;
        }
    }
}
