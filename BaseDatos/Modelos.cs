namespace SDVE
{
    public class Alumno
    {
        public string Matricula { get; set; }
        public string Nombre { get; set; }
        public string Grupo { get; set; }
        public string Carrera { get; set; }
        public string Centro { get; set; }
    }

    public class Voto
    {
        public string Matricula { get; set; }
        public string Convocatoria { get; set; }
        public string Candidato { get; set; }
        public bool EsNoRegistrado { get; set; }
    }
}
