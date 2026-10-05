# Instalacion del SDVE

## Sin herramientas de programacion

El paquete `Entregable_2_Instalacion.zip` incluye el programa, .NET y las bibliotecas de SQLite para Windows de 64 bits. No requiere Visual Studio ni permisos de administrador.

La publicacion del paquete como archivo de una Release esta pendiente. Una vez publicado, se podra descargar desde [Releases del proyecto](https://github.com/duranjoseemilio/Votacion-Estudiantil-SVDE/releases).

1. Descargar el ZIP de instalacion y seleccionar Extraer todo.
2. Abrir la carpeta extraida y ejecutar `Instalar.cmd`.
3. Esperar a que termine y abrir el acceso directo SDVE del escritorio.
4. Como alternativa, ejecutar `Aplicacion/SDVE.exe` desde la carpeta extraida, sin instalar.

Conservar todos los archivos que acompanan al ejecutable, incluida la carpeta Datos. No ejecutar desde el ZIP ni copiar solamente SDVE.exe.

## Con Visual Studio y el SDK de .NET 10

El proyecto completo se encuentra en [SDVE](../../SDVE). Se conserva una sola version integrada en el repositorio para evitar copias diferentes del mismo codigo.

1. Clonar el repositorio con Git, o descargarlo mediante Code y Download ZIP. Si se descarga como ZIP, extraerlo por completo.
2. Abrir la carpeta SDVE.
3. Hacer doble clic en `SDVE.slnx` o en `SDVE.csproj`. Si Windows no los asocia a Visual Studio, abrir Visual Studio y seleccionar Abrir un proyecto o una solucion.
4. Esperar la restauracion de paquetes. Se necesita Internet si no estan disponibles localmente.
5. Seleccionar Compilar solucion y comprobar que no haya errores.
6. Pulsar F5 para ejecutar.

Requisitos: Windows de 64 bits, Visual Studio compatible con .NET 10, componente Desarrollo de escritorio con .NET y SDK de .NET 10. No hace falta instalar un servidor SQLite.

La documentacion detallada esta en el [Entregable 3](../Entregable_3/Entregable3.pdf).
