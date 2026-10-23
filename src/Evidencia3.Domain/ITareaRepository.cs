namespace Evidencia3.Domain;

public interface ITareaRepository
{
    Task<IEnumerable<Tarea>> ObtenerTodasAsync();
    Task<Tarea?> ObtenerPorIdAsync(Guid id);
    Task AgregarAsync(Tarea tarea);
    Task ActualizarAsync(Tarea tarea);
    Task EliminarAsync(Guid id);
    Task<IEnumerable<Tarea>> FiltrarAsync(string? estado, string? responsable);
}