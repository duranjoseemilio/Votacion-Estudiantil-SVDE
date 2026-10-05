CREATE TABLE IF NOT EXISTS Carrera (
    NombreCarrera TEXT NOT NULL PRIMARY KEY,
    Centro        TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Grupo (
    LetraGrupo    TEXT NOT NULL,
    NombreCarrera TEXT NOT NULL REFERENCES Carrera(NombreCarrera),
    PRIMARY KEY (LetraGrupo, NombreCarrera)
);

CREATE TABLE IF NOT EXISTS Alumno (
    IdAlumno      INTEGER NOT NULL PRIMARY KEY,
    Nombre        TEXT    NOT NULL,
    LetraGrupo    TEXT    NOT NULL,
    NombreCarrera TEXT    NOT NULL,
    FOREIGN KEY (LetraGrupo, NombreCarrera) REFERENCES Grupo(LetraGrupo, NombreCarrera)
);

CREATE TABLE IF NOT EXISTS Convocatoria (
    Nombre TEXT    NOT NULL PRIMARY KEY,
    Activa INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Candidato (
    IdCandidato  INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    Nombre       TEXT    NOT NULL,
    Convocatoria TEXT    NOT NULL REFERENCES Convocatoria(Nombre),
    EsRegistrado INTEGER NOT NULL DEFAULT 1,
    UNIQUE (Nombre, Convocatoria)
);

CREATE TABLE IF NOT EXISTS Voto (
    IdAlumno     INTEGER NOT NULL REFERENCES Alumno(IdAlumno),
    Convocatoria TEXT    NOT NULL REFERENCES Convocatoria(Nombre),
    IdCandidato  INTEGER NULL REFERENCES Candidato(IdCandidato),
    PRIMARY KEY (IdAlumno, Convocatoria)
);
