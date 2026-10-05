using System;
using System.Collections.Generic;

namespace SDVE
{
    // DATOS DE PRUEBA.
    // Esta clase inventa alumnos y votos para poder probar el módulo 3 sin base de datos.
    // Cuando la Persona 2 tenga lista la BD, solo hay que cambiar lo de adentro de
    // ObtenerAlumnos() y ObtenerVotos() para que lean de la BD (y regresen las mismas listas).
    public static class DatosVotacion
    {
        private static string[] carreras = { "Ing. en Computación", "Ing. Industrial", "Contaduría", "Psicología" };
        private static string[] siglas = { "ICO", "IIN", "CON", "PSI" };
        private static string[] centros = { "Ciencias Básicas", "Ciencias de la Ingeniería", "Ciencias Económicas", "Ciencias Sociales" };
        private static string[] semestres = { "1A", "1B", "3A", "3B", "5A" };

        public static List<Alumno> ObtenerAlumnos()
        {
            Random azar = new Random(123);   // el 123 hace que siempre salgan los mismos datos
            List<Alumno> lista = new List<Alumno>();

            for (int i = 1; i <= 240; i++)
            {
                int c = azar.Next(carreras.Length);

                Alumno a = new Alumno();
                a.Matricula = "A" + i.ToString("0000");
                a.Nombre = "Alumno " + i;
                a.Carrera = carreras[c];
                a.Centro = centros[c];
                a.Grupo = siglas[c] + "-" + semestres[azar.Next(semestres.Length)];
                lista.Add(a);
            }
            return lista;
        }

        public static List<Voto> ObtenerVotos()
        {
            Random azar = new Random(456);
            List<Alumno> alumnos = ObtenerAlumnos();
            List<Voto> votos = new List<Voto>();

            // Candidatos oficiales de cada convocatoria
            string[][] oficiales = new string[3][];
            oficiales[0] = new string[] { "Planilla Azul", "Planilla Roja", "Planilla Verde" };
            oficiales[1] = new string[] { "Luis Hernández", "María Torres" };
            oficiales[2] = new string[] { "Ana Ramírez", "Jorge Medina", "Sofía Díaz", "Pedro Luna" };

            // Candidatos no registrados (los que el alumno escribe a mano)
            string[] noRegistrados = { "Juan Pérez", "Carlos Soto", "juan pérez" };

            foreach (Alumno a in alumnos)
            {
                // Más o menos el 35% de los alumnos no vota nada (abstencionismo)
                if (azar.NextDouble() < 0.35) continue;

                for (int c = 0; c < 3; c++)
                {
                    // Cada convocatoria la vota con 80% de probabilidad
                    if (azar.NextDouble() > 0.80) continue;

                    Voto v = new Voto();
                    v.Matricula = a.Matricula;
                    v.Convocatoria = ConteoService.Convocatorias[c];

                    if (azar.NextDouble() < 0.08)
                    {
                        v.Candidato = noRegistrados[azar.Next(noRegistrados.Length)];
                        v.EsNoRegistrado = true;
                    }
                    else
                    {
                        v.Candidato = oficiales[c][azar.Next(oficiales[c].Length)];
                        v.EsNoRegistrado = false;
                    }
                    votos.Add(v);
                }
            }
            return votos;
        }
    }
}
