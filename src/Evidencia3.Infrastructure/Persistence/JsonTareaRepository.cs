using System.Text.Json;
using System.Text.Json.Serialization;
using Evidencia3.Domain.Tareas;

namespace Evidencia3.Infrastructure.Persistence;

// Registrar como singleton: el semáforo coordina las operaciones de esta instancia de la API.
public sealed class JsonTareaRepository(string rutaArchivo) : ITareaRepository, IDisposable
{
    private readonly string ruta = Path.GetFullPath(rutaArchivo);
    private readonly SemaphoreSlim mutex = new(1, 1);
    private static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<Tarea?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await mutex.WaitAsync(cancellationToken);
        try
        {
            var tareas = await LeerAsync(cancellationToken);
            return tareas.Find(t => t.Id == id)?.AEntidad();
        }
        finally { mutex.Release(); }
    }

    public async Task<bool> ActualizarDetallesAsync(Tarea tarea, CancellationToken cancellationToken = default)
    {
        await mutex.WaitAsync(cancellationToken);
        try
        {
            var tareas = await LeerAsync(cancellationToken);
            var indice = tareas.FindIndex(t => t.Id == tarea.Id);
            if (indice < 0) return false;

            // Conservar responsable, estado, prioridad y campos futuros del documento actual.
            tareas[indice] = tareas[indice] with { Titulo = tarea.Titulo, Descripcion = tarea.Descripcion };
            await EscribirAsync(tareas, cancellationToken);
            return true;
        }
        finally { mutex.Release(); }
    }

    public async Task<bool> EliminarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await mutex.WaitAsync(cancellationToken);
        try
        {
            var tareas = await LeerAsync(cancellationToken);
            if (tareas.RemoveAll(t => t.Id == id) == 0) return false;
            await EscribirAsync(tareas, cancellationToken);
            return true;
        }
        finally { mutex.Release(); }
    }

    private async Task<List<TareaDocumento>> LeerAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(ruta)) return [];
        await using var archivo = File.OpenRead(ruta);
        // Un archivo corrupto provoca un error; nunca se trata como una lista vacía para sobrescribirlo.
        var tareas = await JsonSerializer.DeserializeAsync<List<TareaDocumento>>(archivo, Opciones, cancellationToken)
            ?? throw new JsonException("El archivo de tareas debe contener una lista JSON.");
        if (tareas.Any(t => t is null) || tareas.Select(t => t.Id).Distinct().Count() != tareas.Count)
            throw new JsonException("El archivo contiene tareas nulas o identificadores duplicados.");
        foreach (var tarea in tareas) _ = tarea.AEntidad();
        return tareas;
    }

    private async Task EscribirAsync(List<TareaDocumento> tareas, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
        var temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var archivo = new FileStream(temporal, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await JsonSerializer.SerializeAsync(archivo, tareas, Opciones, cancellationToken);
                await archivo.FlushAsync(cancellationToken);
            }
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporal, ruta, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporal)) File.Delete(temporal);
        }
    }

    public void Dispose() => mutex.Dispose();

    private sealed record TareaDocumento(Guid Id, string Titulo, string? Descripcion,
        string? Responsable, EstadoTarea Estado, PrioridadTarea Prioridad)
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? CamposAdicionales { get; init; }

        public Tarea AEntidad() => new(Id, Titulo, Descripcion, Responsable, Estado, Prioridad);
    }
}
