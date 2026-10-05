# Sistema Digital de Votacion Estudiantil

Miniproyecto de Programacion Visual desarrollado en C# con Windows Forms. Permite votar en Sociedad de Alumnos, Consejo Universitario y Consejo de Representantes, registrar candidatos oficiales y no registrados, consultar resultados por grupo, carrera y centro universitario y exportarlos a CSV y XML.

## Proyecto que se debe ejecutar

La version completa e integrada esta en **SDVE**. Abrir `SDVE/SDVE.slnx` o `SDVE/SDVE.csproj` con Visual Studio compatible con .NET 10 y el componente Desarrollo de escritorio con .NET. Se necesita el SDK de .NET 10 y Windows de 64 bits.

```powershell
git clone https://github.com/duranjoseemilio/Votacion-Estudiantil-SVDE.git
cd Votacion-Estudiantil-SVDE/SDVE
dotnet restore SDVE.csproj
dotnet build SDVE.csproj
dotnet run --project SDVE.csproj
```

La restauracion inicial requiere Internet si los paquetes no estan disponibles localmente. La aplicacion crea una base SQLite local con datos de demostracion. La matricula 1002 no tiene votos iniciales en una base recien creada. Los datos se guardan en `%LOCALAPPDATA%\SDVE\sdve.db` y no se sincronizan entre equipos.

## Organizacion

- `SDVE`: proyecto integrado que se utiliza para ejecutar el sistema.
- `Aportaciones`: versiones originales de los modulos que estaban en el repositorio antes de la integracion. Se conservan como referencia del trabajo del equipo; no son la version final ejecutable.
- `Entregables`: documentos finales e instrucciones de instalacion.

## Entregables

1. **Respaldo del repositorio completo:** se genera despues de integrar los cambios en main y se entrega por separado. No se incluye un respaldo dentro de si mismo.
2. **Instalacion:** [instrucciones](Entregables/Entregable_2/Instalacion.md) y [descarga del paquete SDVE 1.0](https://github.com/duranjoseemilio/Votacion-Estudiantil-SVDE/releases/tag/v1.0.0).
3. **Documentacion de usuario y tecnica:** [PDF](Entregables/Entregable_3/Entregable3.pdf) 
4. **Presentacion del sistema:** [PDF con enlace al video](Entregables/Entregable_4/Entregable_4.pdf).
5. **Reporte de participacion:** [PDF](Entregables/Entregable_5/Entregable5.pdf)


