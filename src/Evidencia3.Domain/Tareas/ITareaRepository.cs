namespace Evidencia3.Domain.Tareas;

public interface ITareaRepository
{
    Task<Tarea?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Solo actualiza detalles sobre una tarea existente; nunca crea ni reemplaza otros campos.
    Task<bool> ActualizarDetallesAsync(Tarea tarea, CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(Guid id, CancellationToken cancellationToken = default);
}
