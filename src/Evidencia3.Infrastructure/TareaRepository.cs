using System.Text.Json;
using Evidencia3.Domain;

namespace Evidencia3.Infrastructure;

public class TareaRepository : ITareaRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public TareaRepository()
    {
        _filePath = Environment.GetEnvironmentVariable("Tareas__RutaArchivo") ?? "tareas.json";

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private async Task<List<Tarea>> LeerArchivoAsync()
    {
        if (!File.Exists(_filePath)) return new List<Tarea>();
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Tarea>>(json, _jsonOptions) ?? new List<Tarea>();
    }

    public async Task<IEnumerable<Tarea>> FiltrarAsync(string? estado, string? responsable)
    {
        var tareas = await LeerArchivoAsync();
        var query = tareas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(t => t.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(responsable))
        {
            query = query.Where(t => !string.IsNullOrEmpty(t.Responsable) && 
                                     t.Responsable.Equals(responsable, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    public Task<IEnumerable<Tarea>> ObtenerTodasAsync() => throw new NotImplementedException();
    public Task<Tarea?> ObtenerPorIdAsync(Guid id) => throw new NotImplementedException();
    public Task AgregarAsync(Tarea tarea) => throw new NotImplementedException();
    public Task ActualizarAsync(Tarea tarea) => throw new NotImplementedException();
    public Task EliminarAsync(Guid id) => throw new NotImplementedException();
}