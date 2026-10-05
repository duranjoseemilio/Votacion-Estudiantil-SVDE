using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SDVE
{
    // Guarda los resultados en archivos (CSV y XML)
    public static class Exportador
    {
        public static void ExportarCandidatosCsv(string ruta, List<ResultadoCandidato> lista)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Agrupacion,Candidato,Tipo,Votos,Porcentaje");

            foreach (ResultadoCandidato r in lista)
            {
                sb.AppendLine(Campo(r.Agrupacion) + "," +
                              Campo(r.Candidato) + "," +
                              Campo(r.Tipo) + "," +
                              r.Votos + "," +
                              r.Porcentaje.ToString(CultureInfo.InvariantCulture));
            }

            File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8);
        }

        public static void ExportarParticipacionCsv(string ruta, List<ResultadoParticipacion> lista)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Agrupacion,Padron,Votaron,Abstenciones,PorcentajeParticipacion,PorcentajeAbstencion");

            foreach (ResultadoParticipacion r in lista)
            {
                sb.AppendLine(Campo(r.Agrupacion) + "," +
                              r.Padron + "," +
                              r.Votaron + "," +
                              r.Abstenciones + "," +
                              r.PorcentajeParticipacion.ToString(CultureInfo.InvariantCulture) + "," +
                              r.PorcentajeAbstencion.ToString(CultureInfo.InvariantCulture));
            }

            File.WriteAllText(ruta, sb.ToString(), Encoding.UTF8);
        }

        public static void ExportarXml(string ruta, string convocatoria, string agrupadoPor,
                                       List<ResultadoCandidato> candidatos, List<ResultadoParticipacion> participacion)
        {
            ResultadosExportados datos = new ResultadosExportados();
            datos.Convocatoria = convocatoria;
            datos.AgrupadoPor = agrupadoPor;
            datos.Fecha = DateTime.Now;
            datos.Candidatos = candidatos;
            datos.Participacion = participacion;

            XmlSerializer serializador = new XmlSerializer(typeof(ResultadosExportados));
            using (StreamWriter escritor = new StreamWriter(ruta))
            {
                serializador.Serialize(escritor, datos);
            }
        }

        // Si el texto tiene comas o comillas lo encerramos entre comillas para que no rompa el CSV
        private static string Campo(string texto)
        {
            if (texto == null) return "";
            if (texto.Contains(",") || texto.Contains("\""))
            {
                return "\"" + texto.Replace("\"", "\"\"") + "\"";
            }
            return texto;
        }
    }
}
