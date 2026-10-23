# Evidencia3 — Administrador de tareas

Proyecto de la Unidad 2 para administrar tareas y demostrar desarrollo colaborativo mediante Git, commits individuales, Pull Requests y revisión entre integrantes.

## Tecnologías y arquitectura

C#, .NET 10 y ASP.NET Core Web API, con un enfoque Domain-Driven Design (DDD) separado en cuatro proyectos:

| Capa | Responsabilidad |
| --- | --- |
| `Domain` | Entidad `Tarea`, invariantes e interfaz del repositorio; sin dependencias de otras capas. |
| `Application` | Casos de uso de modificación y eliminación y DTO de salida; depende del dominio. |
| `Infrastructure` | Repositorio JSON, lectura y escritura persistente; depende del dominio. |
| `Presentation` | Controladores HTTP, validación de entrada e inyección de dependencias. |

La persistencia inicial usa `System.Text.Json` sin paquetes externos. Un semáforo protege las operaciones del repositorio singleton y las escrituras reemplazan el archivo desde un temporal. Esta implementación es para una única instancia de la aplicación; una base de datos compartida requiere otra implementación del repositorio.

## Integrantes y distribución

| Integrante | Funcionalidad | Rama | Revisor |
| --- | --- | --- | --- |
| Joaquin Lopez | Prioridades y filtros | `feature/joaquin-prioridades-filtros` | Angel Espinoza |
| Angel Espinoza | Registro y consulta general | `feature/angel-registro-consulta` | Edwin Ledezma |
| Edwin Ledezma | Modificación y eliminación; README | `feature/edwin-modificar-eliminar` | Agustin Montoya |
| Agustin Montoya | Asignación y estados | `feature/agustin-asignacion-estados` | Joaquin Lopez |

Solo Edwin modifica este README. Las instrucciones para asistentes están en [AGENTS.md](AGENTS.md) y los prompts individuales en [PROMPTS_EQUIPO.md](PROMPTS_EQUIPO.md).

## Estado de esta rama

Implementados: base de las cuatro capas, modificación de título y descripción, eliminación, persistencia local y verificaciones automáticas. La edición conserva ID, responsable, estado y prioridad, además de los campos JSON adicionales.

Pendientes de las ramas de sus responsables: registro, consulta general, asignación, cambios de estado, prioridades y filtros. Esta entrega es una API parcial; no contiene una interfaz gráfica ni endpoints de alta o listado. El archivo de ejemplo permite probar la funcionalidad de Edwin mientras se integran los demás módulos.

## Compilar y ejecutar

Requisito: SDK de **.NET 10**. Ejecutar desde la raíz del repositorio:

```powershell
dotnet build Evidencia3.slnx
dotnet run --project src/Evidencia3.Presentation --urls http://localhost:5000
```

Por defecto, los datos se guardan en `App_Data/tareas.json` dentro del directorio de contenido de la API. Si el archivo no existe, el repositorio está vacío y los IDs consultados por estos endpoints devuelven `404`.

Para cargar datos de demostración, detener primero la API y ejecutar desde la raíz. El ejemplo evita reemplazar datos existentes:

```powershell
$rutaDatos = Join-Path $PWD 'src/Evidencia3.Presentation/App_Data/tareas.json'
New-Item -ItemType Directory -Force -Path (Split-Path $rutaDatos) | Out-Null
if (-not (Test-Path -LiteralPath $rutaDatos)) {
    Copy-Item -LiteralPath 'samples/tareas.demo.json' -Destination $rutaDatos
}
$env:Tareas__RutaArchivo = $rutaDatos
dotnet run --project src/Evidencia3.Presentation --urls http://localhost:5000
```

La ruta también puede configurarse con `Tareas:RutaArchivo`. Los archivos de datos locales están excluidos de Git. No editar el archivo manualmente mientras la API está en ejecución.

## Endpoints de Edwin

| Método y ruta | Resultado |
| --- | --- |
| `PUT /api/tareas/{id}` | Reemplaza título y descripción; devuelve `200` y la tarea persistida. |
| `DELETE /api/tareas/{id}` | Elimina la tarea; devuelve `204` sin cuerpo. |

ID inexistente: `404`. Campos inválidos o GUID vacío: `400`. Una ruta cuyo ID no tiene formato GUID no coincide con estos endpoints y devuelve `404`. Los errores usan respuestas Problem Details.

El título es obligatorio, se recorta y admite hasta 200 caracteres. La descripción es opcional y admite hasta 2000 caracteres; si se omite o se envía `null`, se deja vacía. El endpoint de edición no cambia responsable, estado ni prioridad.

Ejemplo de edición con los datos de demostración:

```powershell
$idTarea = '11111111-1111-1111-1111-111111111111'
$cuerpo = @{ titulo = 'Preparar evidencia de modificación'; descripcion = 'Cambio realizado por Edwin' } | ConvertTo-Json
Invoke-RestMethod -Method Put -Uri "http://localhost:5000/api/tareas/$idTarea" -ContentType 'application/json; charset=utf-8' -Body ([System.Text.Encoding]::UTF8.GetBytes($cuerpo))
```

Ejemplo de eliminación; una segunda eliminación del mismo ID devuelve `404`:

```powershell
Invoke-RestMethod -Method Delete -Uri "http://localhost:5000/api/tareas/$idTarea"
```

## Verificaciones

```powershell
dotnet run --project tests/Evidencia3.Tests
```

Es un ejecutable de verificaciones sin paquetes externos, no un proyecto para `dotnet test`. Comprueba invariantes, límites y ausencia de mutaciones en entradas inválidas; conservación de campos; persistencia tras reiniciar el repositorio; tareas inexistentes; concurrencia entre distintas tareas; protección frente a archivos corruptos y respuestas HTTP contra un proceso real de la API. Usa un directorio temporal aislado y devuelve un código de salida distinto de cero si falla. Inicia y detiene su propio servidor en un puerto local disponible.

## Flujo de trabajo

Flujo basado en Git Flow adaptado: cada integrante desarrolla en su feature y abre un PR a `develop`, revisado por el integrante asignado. Se integra con **Create a merge commit** para conservar los commits individuales. Después, un PR de `develop` a `main`, revisado por otro integrante, se integra con **Squash and merge** para publicar una entrega estable.

`develop` corresponde a «dev» y `main` a «master». No se realizan pushes directos a estas ramas ni integraciones sin revisión. Esta intervención solo publica en `feature/edwin-modificar-eliminar`; la integración se hará posteriormente por el equipo.
