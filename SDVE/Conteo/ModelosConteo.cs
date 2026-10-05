using System;
using System.Collections.Generic;

namespace SDVE
{
    // Un alumno del padrón (lista de todos los que pueden votar)
    public class Alumno
    {
        public string Matricula { get; set; }
        public string Nombre { get; set; }
        public string Grupo { get; set; }
        public string Carrera { get; set; }
        public string Centro { get; set; }
    }

    // Un voto emitido en UNA convocatoria.
    // Si un alumno vota en las 3 convocatorias, hay 3 votos con la misma matrícula.
    public class Voto
    {
        public string Matricula { get; set; }
        public string Convocatoria { get; set; }
        public string Candidato { get; set; }
        public bool EsNoRegistrado { get; set; }   // true = candidato escrito a mano (write-in)
    }

    // Una fila de la tabla "Votos por candidato"
    public class ResultadoCandidato
    {
        public string Agrupacion { get; set; }     // "General", un grupo, una carrera o un centro
        public string Candidato { get; set; }
        public string Tipo { get; set; }           // "Registrado" o "No registrado"
        public int Votos { get; set; }
        public double Porcentaje { get; set; }     // % dentro de su agrupación
    }

    // Una fila de la tabla "Participación y abstencionismo"
    public class ResultadoParticipacion
    {
        public string Agrupacion { get; set; }
        public int Padron { get; set; }            // alumnos que podían votar
        public int Votaron { get; set; }
        public int Abstenciones { get; set; }
        public double PorcentajeParticipacion { get; set; }
        public double PorcentajeAbstencion { get; set; }
    }

    // Clase que se usa para exportar todo a XML
    public class ResultadosExportados
    {
        public string Convocatoria { get; set; }
        public string AgrupadoPor { get; set; }
        public DateTime Fecha { get; set; }
        public List<ResultadoCandidato> Candidatos { get; set; }
        public List<ResultadoParticipacion> Participacion { get; set; }

        public ResultadosExportados()
        {
            Candidatos = new List<ResultadoCandidato>();
            Participacion = new List<ResultadoParticipacion>();
        }
    }
}
