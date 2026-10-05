using System;
using System.Collections.Generic;
using System.Linq;

namespace SDVE
{
    // Aquí está toda la lógica del módulo 3 (sin pantallas).
    public static class ConteoService
    {
        public static readonly string[] Convocatorias =
        {
            "Sociedad de Alumnos",
            "Consejo Universitario",
            "Consejo de Representantes"
        };

        public static readonly string[] FormasAgrupar = { "General", "Grupo", "Carrera", "Centro" };

        // Dado un alumno, regresa en qué "cajón" cae según cómo se quiera agrupar
        public static string ObtenerAgrupacion(Alumno a, string agrupar)
        {
            if (agrupar == "Grupo") return a.Grupo;
            if (agrupar == "Carrera") return a.Carrera;
            if (agrupar == "Centro") return a.Centro;
            return "General";
        }

        // Cuenta los votos de cada candidato en una convocatoria.
        // Se agrupan según el grupo/carrera/centro del alumno que votó.
        public static List<ResultadoCandidato> ContarCandidatos(List<Voto> votos, List<Alumno> alumnos,
                                                                string convocatoria, string agrupar)
        {
            // Para buscar rápido a un alumno por su matrícula
            Dictionary<string, Alumno> dicAlumnos = new Dictionary<string, Alumno>();
            foreach (Alumno a in alumnos)
            {
                dicAlumnos[a.Matricula] = a;
            }

            Dictionary<string, ResultadoCandidato> conteo = new Dictionary<string, ResultadoCandidato>();
            Dictionary<string, int> totalPorAgrupacion = new Dictionary<string, int>();

            foreach (Voto v in votos)
            {
                if (v.Convocatoria != convocatoria) continue;
                if (!dicAlumnos.ContainsKey(v.Matricula)) continue;   // matrícula que no está en el padrón

                Alumno alumno = dicAlumnos[v.Matricula];
                string agrup = ObtenerAgrupacion(alumno, agrupar);

                string nombre = (v.Candidato ?? "").Trim();
                if (nombre == "") nombre = "Voto en blanco";

                // La clave junta agrupación y nombre (en mayúsculas para que "juan" y "Juan" sean el mismo)
                string clave = agrup + "|" + nombre.ToUpper();

                if (!conteo.ContainsKey(clave))
                {
                    ResultadoCandidato nuevo = new ResultadoCandidato();
                    nuevo.Agrupacion = agrup;
                    nuevo.Candidato = nombre;
                    nuevo.Tipo = v.EsNoRegistrado ? "No registrado" : "Registrado";
                    nuevo.Votos = 0;
                    conteo[clave] = nuevo;
                }
                conteo[clave].Votos++;

                if (!totalPorAgrupacion.ContainsKey(agrup))
                {
                    totalPorAgrupacion[agrup] = 0;
                }
                totalPorAgrupacion[agrup]++;
            }

            // Ya con todo contado, calculamos el porcentaje de cada uno
            List<ResultadoCandidato> resultados = new List<ResultadoCandidato>();
            foreach (ResultadoCandidato r in conteo.Values)
            {
                r.Porcentaje = Redondear(r.Votos * 100.0 / totalPorAgrupacion[r.Agrupacion]);
                resultados.Add(r);
            }

            // Ordenado por agrupación y de más votos a menos
            return resultados.OrderBy(r => r.Agrupacion).ThenByDescending(r => r.Votos).ToList();
        }

        // Calcula cuántos alumnos votaron y cuántos no (abstencionismo) en una convocatoria
        public static List<ResultadoParticipacion> CalcularParticipacion(List<Voto> votos, List<Alumno> alumnos,
                                                                         string convocatoria, string agrupar)
        {
            // Matrículas de los alumnos que votaron en esta convocatoria (sin repetir)
            HashSet<string> quienesVotaron = new HashSet<string>();
            foreach (Voto v in votos)
            {
                if (v.Convocatoria == convocatoria)
                {
                    quienesVotaron.Add(v.Matricula);
                }
            }

            Dictionary<string, int> padron = new Dictionary<string, int>();
            Dictionary<string, int> votaron = new Dictionary<string, int>();
            int padronTotal = 0;
            int votaronTotal = 0;

            foreach (Alumno a in alumnos)
            {
                string agrup = ObtenerAgrupacion(a, agrupar);

                if (!padron.ContainsKey(agrup))
                {
                    padron[agrup] = 0;
                    votaron[agrup] = 0;
                }

                padron[agrup]++;
                padronTotal++;

                if (quienesVotaron.Contains(a.Matricula))
                {
                    votaron[agrup]++;
                    votaronTotal++;
                }
            }

            List<ResultadoParticipacion> resultados = new List<ResultadoParticipacion>();
            foreach (string agrup in padron.Keys.OrderBy(k => k))
            {
                resultados.Add(CrearFila(agrup, padron[agrup], votaron[agrup]));
            }

            // Si se agrupa por algo, agregamos una fila con el total
            if (agrupar != "General")
            {
                resultados.Add(CrearFila("TOTAL", padronTotal, votaronTotal));
            }

            return resultados;
        }

        private static ResultadoParticipacion CrearFila(string agrupacion, int padron, int votaron)
        {
            ResultadoParticipacion fila = new ResultadoParticipacion();
            fila.Agrupacion = agrupacion;
            fila.Padron = padron;
            fila.Votaron = votaron;
            fila.Abstenciones = padron - votaron;

            if (padron > 0)
            {
                double participacion = votaron * 100.0 / padron;
                fila.PorcentajeParticipacion = Redondear(participacion);
                fila.PorcentajeAbstencion = Redondear(100.0 - participacion);
            }
            else
            {
                fila.PorcentajeParticipacion = 0;
                fila.PorcentajeAbstencion = 0;
            }
            return fila;
        }

        private static double Redondear(double numero)
        {
            return Math.Round(numero, 2);
        }
    }
}
