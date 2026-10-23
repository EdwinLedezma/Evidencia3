# Instrucciones para agentes y colaboradores

Este archivo guía a Copilot, ChatGPT y otros asistentes que trabajen en este repositorio. Antes de realizar cambios, léelo completo. Si el asistente no lo carga automáticamente, el integrante debe indicarle: «Lee AGENTS.md y trabaja únicamente en mi rama y responsabilidad».

Repositorio: https://github.com/EdwinLedezma/Evidencia3/

## Objetivo y alcance

Desarrollar una aplicación funcional para administrar tareas de un equipo y demostrar la participación individual mediante commits, Pull Requests (PR), revisiones e integraciones verificables.

La aplicación debe permitir registrar, consultar, modificar, eliminar y asignar tareas; establecer estados (Pendiente, En proceso, Terminada), prioridades (Baja, Media, Alta) y consultar por estado o responsable.

Antes de programar, revisa el código y las decisiones documentadas por el equipo. No elijas ni cambies unilateralmente el lenguaje, framework, arquitectura o estructura compartida. Si aún no están definidos y son necesarios para implementar, solicita esa definición al integrante. No crees cuatro aplicaciones independientes: todas las funcionalidades deben integrarse en la misma aplicación.

## Tecnologías y arquitectura acordadas

**Todos los integrantes deben usar C#, .NET 10 (`net10.0`) y Domain-Driven Design (DDD) con estas capas compartidas.** No crear otra aplicación ni cambiar de stack por iniciativa del asistente.

```text
Evidencia3.slnx
src/
  Evidencia3.Domain/          # Entidades, invariantes e interfaces de repositorio
  Evidencia3.Application/     # Casos de uso y contratos de entrada/salida
  Evidencia3.Infrastructure/  # Implementaciones de persistencia
  Evidencia3.Presentation/    # API HTTP ASP.NET Core, controladores y composición DI
tests/
  Evidencia3.Tests/           # Verificaciones ejecutables de dominio, persistencia y HTTP
```

- `Domain` no depende de las otras capas ni de ASP.NET Core o de una tecnología de persistencia.
- `Application` depende de `Domain`. Orquesta los casos de uso; no accede directamente a archivos ni conoce controladores.
- `Infrastructure` depende de `Domain` e implementa sus interfaces de repositorio.
- `Presentation` depende de `Application` y referencia `Infrastructure` para registrar las implementaciones mediante inyección de dependencias. No contiene reglas de negocio.
- La separación en carpetas por sí sola no constituye DDD: mantener la lógica e invariantes de la entidad en `Domain` y usar el mismo lenguaje de tareas en todas las capas.
- Usar ASP.NET Core Web API con controladores, inyección de dependencias integrada y `System.Text.Json`. La base inicial no requiere paquetes NuGet externos.
- La persistencia inicial es un archivo JSON local configurable con `Tareas:RutaArchivo` (variable de entorno `Tareas__RutaArchivo`). Es una base para una sola instancia del proceso; no usarla con varios servidores concurrentes. Si se acuerda una base de datos, sustituir la implementación en `Infrastructure`, conservando los contratos y casos de uso.
- Reutilizar la entidad `Tarea` y el repositorio `ITareaRepository`. Extenderlos coordinadamente para registro, consulta, asignación y filtros; no duplicar modelos de tarea por integrante.
- Contrato base de una tarea: `Id` (`Guid`), `Titulo`, `Descripcion`, `Responsable` (texto opcional), `Estado` (`Pendiente`, `EnProceso`, `Terminada`) y `Prioridad` (`Baja`, `Media`, `Alta`). En JSON HTTP los campos usan camelCase y los enums se representan con esos nombres de texto.
- Un título es obligatorio, se recorta y admite hasta 200 caracteres. La descripción es opcional y admite hasta 2000 caracteres. La edición de Edwin modifica únicamente título y descripción; conserva identificador, responsable, estado y prioridad.
- Endpoints de Edwin: `PUT /api/tareas/{id}` con `{ "titulo": "...", "descripcion": "..." }` devuelve `200` con la tarea actualizada; `DELETE /api/tareas/{id}` devuelve `204`. Un identificador inexistente devuelve `404`; una entrada inválida devuelve `400`. El endpoint de edición reemplaza ambos campos editables; omitir la descripción la deja vacía.
- Los endpoints de registro y consulta general corresponden a Angel; asignación y estados, a Agustin; prioridades y filtros, a Joaquin. Los datos de prueba se crean en las verificaciones, no mediante endpoints que suplanten el trabajo de otros integrantes.
- Compilar: `dotnet build Evidencia3.slnx`. Ejecutar API: `dotnet run --project src/Evidencia3.Presentation --urls http://localhost:5000`. Ejecutar verificaciones: `dotnet run --project tests/Evidencia3.Tests`. Este último es un ejecutable de pruebas sin paquetes externos, devuelve código de salida distinto de cero si falla y no utiliza `dotnet test`.
- No subir `bin/`, `obj/` ni archivos de datos locales. No agregar frameworks, bibliotecas ni servicios externos sin una necesidad concreta y coordinación del equipo.

Referencias de arquitectura: [DDD y capas](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice) e [interfaces y persistencia](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design).

## Responsabilidades y revisores

| Integrante | Responsabilidad | Rama existente | Revisor de su PR |
| --- | --- | --- | --- |
| Joaquin Lopez | Prioridades y Filtros | `feature/joaquin-prioridades-filtros` | Angel Espinoza |
| Angel Espinoza | Registro y Consulta general | `feature/angel-registro-consulta` | Edwin Ledezma |
| Edwin Ledezma | Modificación y Eliminación | `feature/edwin-modificar-eliminar` | Agustin Montoya |
| Agustin Montoya | Asignación y Estados | `feature/agustin-asignacion-estados` | Joaquin Lopez |

- Joaquin implementa prioridades y filtros por estado o responsable, coordinando los campos y consultas con Agustin y Angel.
- Angel implementa el registro y la consulta general, coordinando el modelo de tarea y la persistencia con el equipo.
- Edwin implementa la modificación y eliminación, además de ser el único responsable de editar el README.
- Agustin implementa la asignación de responsables y los cambios de estado.
- Limita cada cambio a la responsabilidad del autor. Coordina previamente cualquier cambio de contratos, campos, dependencias o archivos compartidos que afecte a otros módulos.
- No implementes la responsabilidad completa de otro integrante ni generes commits haciéndote pasar por él.

## Ramas y flujo de integración

El equipo utiliza un flujo basado en Git Flow, adaptado al tamaño del proyecto: ramas individuales para desarrollar, una rama de integración y una rama estable. Permite revisar cada aporte y comprobar el trabajo de cada integrante antes de publicar una versión funcional.

**Nombres reales de este repositorio:**

- `develop`: integración; corresponde a «dev» en las instrucciones del equipo.
- `main`: versión estable; corresponde a «master» en las instrucciones del equipo.
- `feature/<integrante>-<funcionalidad>`: trabajo individual; las cuatro ramas de la tabla ya existen.

No crees ramas `dev` o `master` adicionales ni renombres las existentes. No uses otras ramas remotas como parte del flujo sin acuerdo del equipo.

El flujo obligatorio para todos es:

```text
feature del integrante
    └── PR revisado y aprobado → develop (Create a merge commit)
                                    └── PR revisado y aprobado → main (Squash and merge)
```

1. Cada integrante programa, realiza sus commits y hace push a su propia rama.
2. Abre un PR desde su `feature/...` hacia `develop` y solicita al revisor asignado.
3. El revisor revisa los cambios desde su propia cuenta, deja observaciones o aprueba. El autor atiende las observaciones en la misma rama y PR.
4. Con aprobación de otro integrante y verificaciones satisfactorias, integra el PR a `develop` usando **Create a merge commit** para conservar los commits individuales.
5. Con el trabajo integrado y validado, abre un PR desde `develop` hacia `main`. Otro integrante debe revisarlo y aprobarlo antes de integrarlo usando **Squash and merge**.
6. El PR a `main` puede agrupar varios aportes ya aprobados en `develop`. Su descripción debe enlazar los PR de origen, sus autores y las verificaciones realizadas.

No abras PR directos de `feature/...` a `main`. No hagas push directo a `develop` ni a `main`. No uses squash en el paso `feature → develop`, pues allí se preserva la trazabilidad individual. No integres un PR sin revisión ajena ni simules aprobaciones humanas.

Los cuatro integrantes siguen el mismo procedimiento; la exclusividad del README no cambia el flujo de Edwin. No borres ramas existentes automáticamente. Al terminar el proyecto, el equipo puede eliminar las ramas feature ya integradas si conserva los PR y el historial como evidencia.

## Regla exclusiva del README

**Solo Edwin Ledezma puede crear, modificar, reemplazar o eliminar `README.md` y sus variantes de nombre o ubicación.**

- Para Joaquin, Angel y Agustin: no edites el README, ni siquiera para corregir formato, comandos o documentación de tu funcionalidad.
- Si necesitas proponer una actualización, inclúyela como sugerencia en la descripción del PR para que Edwin la incorpore desde `feature/edwin-modificar-eliminar`.
- Edwin realiza los cambios del README en su propia rama y los pasa por PR a `develop`, con revisión de Agustin; después llegan a `main` mediante el PR de promoción con squash.
- Si al sincronizar tu rama llegan cambios del README ya presentes en `develop`, consérvalos. Eso no autoriza a crear modificaciones propias. Ante un conflicto en ese archivo, solicita a Edwin que determine la resolución.
- Antes de abrir un PR de un autor distinto de Edwin, comprueba que el diff contra `origin/develop` no incluya cambios propios del README.

## Procedimiento para el asistente

### 1. Inspeccionar antes de editar

```powershell
git status --short
git branch --show-current
git remote -v
git fetch origin
```

Identifica al integrante por la rama activa y la tabla. Si la rama activa no corresponde al integrante que solicita el trabajo, aclara su identidad antes de implementar. No deduzcas que todas las solicitudes pertenecen a Edwin por ser el propietario del repositorio.

Respeta los cambios locales existentes. No ejecutes `reset --hard`, `clean`, `push --force`, ni descartes archivos para resolver problemas. Si hay cambios pendientes que impiden cambiar de rama o sincronizar, pide al integrante que decida cómo conservarlos.

### 2. Trabajar en la rama individual

Con el árbol de trabajo limpio, sustituye `<rama-personal>` por la rama exacta de la tabla; los marcadores de estos ejemplos no son comandos literales:

```powershell
git switch <rama-personal>
git pull --ff-only origin <rama-personal>
git merge origin/develop
```

Si la rama solo existe remotamente, utiliza `git switch --track origin/<rama-personal>`. No vuelvas a crear una rama que ya existe. Si el pull no puede avanzar directamente, inspecciona la divergencia y coordina la solución sin reescribir historial publicado.

Resuelve conflictos dentro de tu responsabilidad y ejecuta las verificaciones correspondientes. Los conflictos con funcionalidades de otros integrantes se coordinan con sus autores; los del README, con Edwin.

### 3. Implementar, verificar y registrar

- Inspecciona la estructura y reutiliza los modelos, servicios, validaciones y estilos existentes.
- Implementa incrementos coherentes y verificables. No concentres todo el proyecto en un único commit final ni crees commits vacíos para aparentar participación.
- Ejecuta los comandos de compilación, pruebas o validación que realmente existan en el proyecto. Documenta los resultados reales y cualquier limitación; no afirmes haber ejecutado pruebas que no realizaste.
- Comprueba el diff y agrega solo los archivos relacionados. No incluyas secretos, credenciales, archivos temporales ni cambios ajenos.
- Verifica `git config user.name` y `git config user.email` antes de hacer commits. Deben corresponder al autor real; no cambies la identidad a la de otro integrante ni uses credenciales de compañeros.

Mensajes de commit: `tipo: descripción concreta`, por ejemplo:

```text
feat: registrar tareas con validación de campos obligatorios
feat: filtrar tareas por responsable
fix: conservar la prioridad al modificar una tarea
docs: documentar la ejecución de la aplicación
```

El ejemplo `docs` relacionado con el README corresponde exclusivamente a Edwin.

```powershell
git diff --check
git diff
git add <archivos-relacionados>
git diff --cached
git commit -m "feat: describir el cambio realizado"
git push -u origin <rama-personal>
git diff --name-only origin/develop...HEAD
```

### 4. Preparar el PR hacia develop

Usa como base `develop` y como origen la rama personal. La descripción debe incluir:

- Autor, responsabilidad y revisor asignado.
- Problema resuelto y comportamiento implementado.
- Archivos o componentes relevantes y dependencias de otros aportes.
- Cómo probarlo, resultados obtenidos y limitaciones conocidas.
- Capturas cuando aporten evidencia del funcionamiento.
- Confirmación de que se respetó la propiedad del README.

Atiende las solicitudes de cambios mediante nuevos commits en la misma rama y solicita una nueva revisión cuando corresponda. No apruebes tu propio PR ni integres mientras existan conflictos, revisiones pendientes o fallos de validación sin resolver.

### 5. Promover de develop a main

Antes del PR de promoción, verifica la aplicación integrada: registro, consulta, modificación, eliminación, asignación, estados, prioridades y filtros, según las funcionalidades incluidas en esa entrega. Detalla qué se probó y qué queda pendiente.

Abre el PR con base `main` y origen `develop`; enlaza los PR incorporados y solicita revisión a otro integrante. Con aprobación y verificaciones satisfactorias, utiliza **Squash and merge** con un mensaje que describa la entrega. Mantén los PR y commits individuales en `develop` como evidencia.

Después de cada squash, revisa la comparación entre `develop` y `main` antes de una nueva promoción: el squash produce un commit diferente a los de `develop`. Si hace falta sincronizar el historial, prepara un PR de `main` hacia `develop`, revisado e integrado con merge commit; no reinicies ni fuerces las ramas compartidas.

## Evidencia y entrega del asistente

Cada integrante opera desde su propia cuenta. El historial real debe permitir verificar la secuencia rama → cambios → commits → push → PR → revisión de otro integrante → integración.

Al finalizar una intervención, informa la rama usada, los cambios realizados, las verificaciones y sus resultados, y los commits o enlaces de PR si realmente se crearon. Si no tienes acceso al remoto o no se te pidió publicar, entrega el trabajo local y las instrucciones concretas para continuar; no inventes pushes, PR, revisiones ni merges.

Este archivo documenta acuerdos; no configura por sí mismo protecciones en GitHub. El equipo debe habilitar las opciones **merge commit** y **squash merge**, y configurar reglas para `develop` y `main` que exijan PR, aprobación de otro integrante y verificaciones automáticas cuando existan. La asignación de revisores y la exclusividad del README deben comprobarse también durante la revisión.
