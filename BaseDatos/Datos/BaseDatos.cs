using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SDVE
{
    public static class BaseDatos
    {
        public static readonly string Ruta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SDVE", "sdve.db");

        private static readonly string[] Convocatorias =
        {
            "Sociedad de Alumnos", "Consejo Universitario", "Consejo de Representantes"
        };

        public static SqliteConnection Abrir()
        {
            SqliteConnection con = new SqliteConnection("Data Source=" + Ruta + ";Pooling=False");
            con.Open();

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "PRAGMA foreign_keys = ON;";
                cmd.ExecuteNonQuery();
            }
            return con;
        }

        public static void Preparar()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Ruta));

            using (var con = Abrir())
            {
                Ejecutar(con, File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Datos", "esquema.sql")));

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Alumno";
                    if ((long)cmd.ExecuteScalar() > 0) return;
                }

                using (var tx = con.BeginTransaction())
                {
                    SembrarDatosDePrueba(con);
                    tx.Commit();
                }
            }
        }

        private static void SembrarDatosDePrueba(SqliteConnection con)
        {
            string[] carreras = { "Ing. en Computación", "Ing. Industrial", "Contaduría", "Psicología" };
            string[] centros = { "Ciencias Básicas", "Ciencias de la Ingeniería", "Ciencias Económicas", "Ciencias Sociales" };
            string[] letras = { "A", "B", "C" };

            for (int c = 0; c < carreras.Length; c++)
            {
                Ejecutar(con, "INSERT INTO Carrera VALUES ($a, $b)", ("$a", carreras[c]), ("$b", centros[c]));
                foreach (string l in letras)
                    Ejecutar(con, "INSERT INTO Grupo VALUES ($a, $b)", ("$a", l), ("$b", carreras[c]));
            }

            foreach (string conv in Convocatorias)
                Ejecutar(con, "INSERT INTO Convocatoria VALUES ($a, 1)", ("$a", conv));

            string[][] oficiales = new string[3][];
            oficiales[0] = new[] { "Planilla Azul", "Planilla Roja", "Planilla Verde" };
            oficiales[1] = new[] { "Luis Hernández", "María Torres" };
            oficiales[2] = new[] { "Ana Ramírez", "Jorge Medina", "Sofía Díaz", "Pedro Luna" };

            string[] escritos = { "Juan Pérez", "Carlos Soto" };

            List<long>[] idsOficiales = new List<long>[3];
            List<long>[] idsNoRegistrados = new List<long>[3];
            for (int c = 0; c < 3; c++)
            {
                idsOficiales[c] = new List<long>();
                idsNoRegistrados[c] = new List<long>();
                foreach (string n in oficiales[c])
                    idsOficiales[c].Add(InsertarCandidato(con, n, Convocatorias[c], 1));
                foreach (string n in escritos)
                    idsNoRegistrados[c].Add(InsertarCandidato(con, n, Convocatorias[c], 0));
            }

            Random azar = new Random(123);
            for (int i = 1; i <= 240; i++)
            {
                int c = azar.Next(carreras.Length);
                Ejecutar(con, "INSERT INTO Alumno VALUES ($id, $n, $l, $c)",
                    ("$id", 1000 + i), ("$n", "Alumno " + i),
                    ("$l", letras[azar.Next(letras.Length)]), ("$c", carreras[c]));
            }

            Random azarVotos = new Random(456);
            for (int i = 1; i <= 240; i++)
            {
                if (azarVotos.NextDouble() < 0.35) continue;

                for (int c = 0; c < 3; c++)
                {
                    if (azarVotos.NextDouble() > 0.80) continue;

                    object idCandidato;
                    double tipo = azarVotos.NextDouble();
                    if (tipo < 0.05) idCandidato = DBNull.Value;
                    else if (tipo < 0.12) idCandidato = idsNoRegistrados[c][azarVotos.Next(escritos.Length)];
                    else idCandidato = idsOficiales[c][azarVotos.Next(oficiales[c].Length)];

                    Ejecutar(con, "INSERT INTO Voto VALUES ($a, $c, $k)",
                        ("$a", 1000 + i), ("$c", Convocatorias[c]), ("$k", idCandidato));
                }
            }
        }

        private static long InsertarCandidato(SqliteConnection con, string nombre, string convocatoria, int registrado)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Candidato (Nombre, Convocatoria, EsRegistrado) VALUES ($n, $c, $r); " +
                                  "SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("$n", nombre);
                cmd.Parameters.AddWithValue("$c", convocatoria);
                cmd.Parameters.AddWithValue("$r", registrado);
                return (long)cmd.ExecuteScalar();
            }
        }

        private static void Ejecutar(SqliteConnection con, string sql, params (string, object)[] parametros)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;
                foreach (var p in parametros) cmd.Parameters.AddWithValue(p.Item1, p.Item2);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
