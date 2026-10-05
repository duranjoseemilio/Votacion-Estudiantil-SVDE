using SDVE;

var alumnos = DatosVotacion.ObtenerAlumnos();
var votos = DatosVotacion.ObtenerVotos();

Console.WriteLine($"Alumnos: {alumnos.Count}   Votos: {votos.Count}");
Console.WriteLine($"Ejemplo alumno: {alumnos[0].Matricula} | {alumnos[0].Nombre} | {alumnos[0].Grupo} | {alumnos[0].Carrera} | {alumnos[0].Centro}");
Console.WriteLine($"Grupos distintos: {alumnos.Select(a => a.Grupo).Distinct().Count()}");
Console.WriteLine($"Votos en blanco: {votos.Count(v => v.Candidato == "")}   No registrados: {votos.Count(v => v.EsNoRegistrado)}");
foreach (var g in votos.GroupBy(v => v.Convocatoria))
    Console.WriteLine($"  {g.Key}: {g.Count()} votos");
Console.WriteLine($"Matrículas con voto repetido en una convocatoria: {votos.GroupBy(v => v.Matricula + v.Convocatoria).Count(g => g.Count() > 1)}");
