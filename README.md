# *Moogle!*
*Moogle!* es una aplicación *totalmente original* cuyo propósito es buscar inteligentemente un texto en un conjunto de documentos.

Es una aplicación web, desarrollada con tecnología .NET Core 6.0, específicamente usando Blazor como *framework* web para la interfaz gráfica, y en el lenguaje C#.

## Para utilizar esta aplicación web:
Primeto se debe almacenar el contenido que se pretende guardar para luego buscar, en el directorio llamado *"Content"* distribuido en archivos *.txt*.
Luego está listo para ejecutarse y funcionar.

## Para ejecutar esta aplicación
Se necesita tener instalado *.NET Core 6.0* en el sistema y ejecutar en la terminal, con el directorio de la aplicación abierto, uno de estos comandos:

- Si estás en *Linux*, utiliza el comando `make dev`.
  
- Si estás en *Windows*, puedes abrir la terminal desde *Visual Studio Code*, o en el caso de Windows 11 puedes abrir la terminal directamente en el directorio del programa, y entonces utilizar el comando `dotnet watch run --project MoogleServer`.



*Moogle!* trabaja bajo el algoritmo de un modelo vectorial de recuperación de información y puede servir como buscador de archivos locales para encontrar el documento deseado en menos tiempo.
