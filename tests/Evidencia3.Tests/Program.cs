using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Evidencia3.Application.Tareas;
using Evidencia3.Domain.Tareas;
using Evidencia3.Infrastructure.Persistence;

var aprobadas = 0;
var temporal = Path.Combine(Path.GetTempPath(), "Evidencia3.Tests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temporal);
Process? servidor = null;
try
{
    var id = Guid.NewGuid();
    var otroId = Guid.NewGuid();
    var tarea = new Tarea(id, "Inicial", "Descripción", "Angel", EstadoTarea.EnProceso, PrioridadTarea.Alta);
    tarea.ModificarDetalles("  Editada  ", "  Nueva descripción  ");
    Verificar(tarea.Titulo == "Editada" && tarea.Descripcion == "Nueva descripción", "Recorta los detalles");
    Verificar(tarea.Id == id && tarea.Responsable == "Angel" && tarea.Estado == EstadoTarea.EnProceso
        && tarea.Prioridad == PrioridadTarea.Alta, "La edición conserva los campos ajenos");
    foreach (var titulo in new[] { "", "   ", new string('a', 201) })
        Rechazar(() => tarea.ModificarDetalles(titulo, "Cambio"), "Rechaza título inválido");
    Rechazar(() => tarea.ModificarDetalles("No debe cambiar", new string('a', 2001)), "Rechaza descripción larga");
    Verificar(tarea.Titulo == "Editada", "Una validación fallida no muta la entidad");
    tarea.ModificarDetalles(new string('a', 200), new string('b', 2000));
    Verificar(tarea.Titulo.Length == 200 && tarea.Descripcion.Length == 2000, "Acepta longitudes límite");
    tarea.ModificarDetalles("Sin descripción", null);
    Verificar(tarea.Descripcion == "", "Permite quitar la descripción");
    Rechazar(() => _ = new Tarea(Guid.Empty, "Título"), "Rechaza identificador vacío");

    var ruta = Path.Combine(temporal, "tareas.json");
    await SembrarAsync();
    using (var repository = new JsonTareaRepository(ruta))
    {
        var service = new TareaService(repository);
        var editada = await service.ModificarAsync(id, "Modificada", "Desde el caso de uso");
        Verificar(editada?.Titulo == "Modificada" && editada.Estado == EstadoTarea.EnProceso
            && editada.Prioridad == PrioridadTarea.Alta && editada.Responsable == "Angel", "Caso de uso persiste y conserva campos");
        using (var datos = JsonDocument.Parse(await File.ReadAllTextAsync(ruta)))
            Verificar(datos.RootElement[0].GetProperty("etiquetaFutura").GetString() == "conservar", "Conserva campos JSON de futuras funcionalidades");
        Verificar(await service.ModificarAsync(Guid.NewGuid(), "Ausente", null) is null, "No modifica una tarea inexistente");
        Verificar(!await service.EliminarAsync(Guid.NewGuid()), "No elimina una tarea inexistente");
        var eliminada = (await repository.ObtenerPorIdAsync(id))!;
        Verificar(await service.EliminarAsync(id), "Elimina una tarea existente");
        Verificar(!await repository.ActualizarDetallesAsync(eliminada), "Una actualización tardía no recrea una tarea eliminada");
        Verificar(await repository.ObtenerPorIdAsync(otroId) is not null, "Eliminar conserva las otras tareas");
    }
    using (var reiniciado = new JsonTareaRepository(ruta))
        Verificar(await reiniciado.ObtenerPorIdAsync(id) is null
            && await reiniciado.ObtenerPorIdAsync(otroId) is not null, "La eliminación persiste al reiniciar el repositorio");

    var corrupto = Path.Combine(temporal, "corrupto.json");
    await File.WriteAllTextAsync(corrupto, "JSON inválido");
    using (var repository = new JsonTareaRepository(corrupto))
    {
        try
        {
            await repository.EliminarAsync(id);
            throw new Exception("Se esperaba rechazar el archivo corrupto.");
        }
        catch (JsonException) { Verificar(await File.ReadAllTextAsync(corrupto) == "JSON inválido", "No sobrescribe archivos corruptos"); }
    }
    using (var repository = new JsonTareaRepository(Path.Combine(temporal, "ausente.json")))
        Verificar(await repository.ObtenerPorIdAsync(id) is null && !await repository.EliminarAsync(id), "Archivo ausente representa un repositorio vacío");

    await SembrarAsync();
    using (var repository = new JsonTareaRepository(ruta))
    {
        var service = new TareaService(repository);
        await Task.WhenAll(service.ModificarAsync(id, "Primera", null), service.ModificarAsync(otroId, "Segunda", null));
        Verificar((await repository.ObtenerPorIdAsync(id))?.Titulo == "Primera"
            && (await repository.ObtenerPorIdAsync(otroId))?.Titulo == "Segunda", "Actualizaciones concurrentes no pierden otras tareas");
    }

    // Integración HTTP contra Kestrel real, sin dependencias de paquetes de pruebas.
    await SembrarAsync();
    var raiz = new DirectoryInfo(AppContext.BaseDirectory);
    while (raiz is not null && !File.Exists(Path.Combine(raiz.FullName, "Evidencia3.slnx"))) raiz = raiz.Parent;
    if (raiz is null) throw new Exception("No se encontró la solución.");
    var configuracion = Directory.GetParent(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar))!.Name;
    var dll = Path.Combine(raiz.FullName, "src", "Evidencia3.Presentation", "bin", configuracion, "net10.0", "Evidencia3.Presentation.dll");
    var inicio = new ProcessStartInfo("dotnet")
    {
        UseShellExecute = false, CreateNoWindow = true,
        RedirectStandardOutput = true, RedirectStandardError = true,
        WorkingDirectory = temporal
    };
    inicio.ArgumentList.Add(dll);
    inicio.ArgumentList.Add("--urls");
    inicio.ArgumentList.Add("http://127.0.0.1:0");
    inicio.Environment["Tareas__RutaArchivo"] = ruta;
    inicio.Environment["ASPNETCORE_ENVIRONMENT"] = "Production";
    inicio.Environment["Logging__LogLevel__Default"] = "Information";
    var direccion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
    var salida = new StringBuilder();
    servidor = new Process { StartInfo = inicio };
    servidor.OutputDataReceived += (_, evento) =>
    {
        if (evento.Data is not { } linea) return;
        lock (salida) salida.AppendLine(linea);
        const string prefijo = "Now listening on: ";
        var posicion = linea.IndexOf(prefijo, StringComparison.Ordinal);
        if (posicion >= 0) direccion.TrySetResult(linea[(posicion + prefijo.Length)..].Trim());
    };
    servidor.ErrorDataReceived += (_, evento) => { lock (salida) salida.AppendLine(evento.Data); };
    servidor.Start();
    servidor.BeginOutputReadLine();
    servidor.BeginErrorReadLine();
    string url;
    try { url = await direccion.Task.WaitAsync(TimeSpan.FromSeconds(25)); }
    catch (TimeoutException) { throw new Exception("La API no inició: " + salida); }
    using var cliente = new HttpClient { BaseAddress = new Uri(url), Timeout = TimeSpan.FromSeconds(10) };
    using (var respuesta = await cliente.PutAsJsonAsync($"/api/tareas/{id}", new { titulo = "Editada por HTTP", descripcion = "Cambio" }))
    {
        if (respuesta.StatusCode != HttpStatusCode.OK)
            throw new Exception($"PUT devolvió {(int)respuesta.StatusCode}: {await respuesta.Content.ReadAsStringAsync()}\n{salida}");
        Verificar(true, "PUT devuelve 200");
        using var json = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        Verificar(json.RootElement.GetProperty("titulo").GetString() == "Editada por HTTP"
            && json.RootElement.GetProperty("estado").GetString() == "EnProceso"
            && json.RootElement.GetProperty("prioridad").GetString() == "Alta"
            && json.RootElement.GetProperty("responsable").GetString() == "Angel", "HTTP devuelve detalles actualizados y conserva el resto");
    }
    foreach (var titulo in new[] { "", "   ", new string('a', 201) })
    {
        using var respuesta = await cliente.PutAsJsonAsync($"/api/tareas/{id}", new { titulo });
        Verificar(respuesta.StatusCode == HttpStatusCode.BadRequest, "PUT inválido devuelve 400");
    }
    using (var respuesta = await cliente.PutAsJsonAsync($"/api/tareas/{id}", new { titulo = "Título", descripcion = new string('a', 2001) }))
        Verificar(respuesta.StatusCode == HttpStatusCode.BadRequest, "HTTP rechaza descripción larga");
    using (var respuesta = await cliente.PutAsJsonAsync($"/api/tareas/{Guid.NewGuid()}", new { titulo = "Ausente" }))
        Verificar(respuesta.StatusCode == HttpStatusCode.NotFound, "PUT inexistente devuelve 404");
    using (var contenido = new StringContent("{inválido", Encoding.UTF8, "application/json"))
    using (var respuesta = await cliente.PutAsync($"/api/tareas/{id}", contenido))
        Verificar(respuesta.StatusCode == HttpStatusCode.BadRequest, "JSON malformado devuelve 400");
    using (var respuesta = await cliente.DeleteAsync($"/api/tareas/{Guid.Empty}"))
        Verificar(respuesta.StatusCode == HttpStatusCode.BadRequest, "DELETE con Guid vacío devuelve 400");
    using (var respuesta = await cliente.PutAsJsonAsync($"/api/tareas/{id}", new { titulo = "Sin descripción" }))
    {
        using var json = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
        Verificar(respuesta.StatusCode == HttpStatusCode.OK && json.RootElement.GetProperty("descripcion").GetString() == "", "PUT permite quitar la descripción");
    }
    using (var respuesta = await cliente.DeleteAsync($"/api/tareas/{id}"))
        Verificar(respuesta.StatusCode == HttpStatusCode.NoContent && (await respuesta.Content.ReadAsByteArrayAsync()).Length == 0, "DELETE devuelve 204 sin cuerpo");
    using (var respuesta = await cliente.DeleteAsync($"/api/tareas/{id}"))
        Verificar(respuesta.StatusCode == HttpStatusCode.NotFound, "DELETE repetido devuelve 404");
    using (var repository = new JsonTareaRepository(ruta))
        Verificar(await repository.ObtenerPorIdAsync(id) is null && await repository.ObtenerPorIdAsync(otroId) is not null, "HTTP persiste la eliminación sin afectar otra tarea");

    Console.WriteLine($"RESULTADO: {aprobadas} verificaciones aprobadas.");

    async Task SembrarAsync() => await File.WriteAllTextAsync(ruta, JsonSerializer.Serialize(new[]
    {
        new { id, titulo = "Inicial", descripcion = "Descripción", responsable = "Angel", estado = "EnProceso", prioridad = "Alta", etiquetaFutura = "conservar" },
        new { id = otroId, titulo = "Otra tarea", descripcion = "", responsable = "Agustin", estado = "Pendiente", prioridad = "Baja", etiquetaFutura = "otra" }
    }));
}
catch (Exception error)
{
    Console.Error.WriteLine(error);
    Environment.ExitCode = 1;
}
finally
{
    if (servidor is not null)
    {
        if (!servidor.HasExited) servidor.Kill(entireProcessTree: true);
        await servidor.WaitForExitAsync();
        servidor.Dispose();
    }
    // Solo elimina el directorio temporal creado por esta ejecución.
    var rutaCompleta = Path.GetFullPath(temporal);
    var baseTemporal = Path.GetFullPath(Path.GetTempPath()) + (Path.EndsInDirectorySeparator(Path.GetTempPath()) ? "" : Path.DirectorySeparatorChar);
    if (rutaCompleta.StartsWith(baseTemporal + "Evidencia3.Tests-", StringComparison.OrdinalIgnoreCase))
        Directory.Delete(rutaCompleta, recursive: true);
}

void Verificar(bool condicion, string nombre)
{
    if (!condicion) throw new Exception("FALLÓ: " + nombre);
    aprobadas++;
    Console.WriteLine("OK: " + nombre);
}

void Rechazar(Action accion, string nombre)
{
    try { accion(); }
    catch (ArgumentException) { Verificar(true, nombre); return; }
    throw new Exception("FALLÓ: " + nombre);
}
