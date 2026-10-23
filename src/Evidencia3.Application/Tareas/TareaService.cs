using Evidencia3.Domain.Tareas;

namespace Evidencia3.Application.Tareas;

public sealed record TareaDto(Guid Id, string Titulo, string Descripcion,
    string? Responsable, EstadoTarea Estado, PrioridadTarea Prioridad)
{
    public static TareaDto Desde(Tarea tarea) => new(tarea.Id, tarea.Titulo,
        tarea.Descripcion, tarea.Responsable, tarea.Estado, tarea.Prioridad);
}

public sealed class TareaService(ITareaRepository repository)
{
    public async Task<TareaDto?> ModificarAsync(Guid id, string titulo, string? descripcion,
        CancellationToken cancellationToken = default)
    {
        var tarea = await repository.ObtenerPorIdAsync(id, cancellationToken);
        if (tarea is null) return null;

        tarea.ModificarDetalles(titulo, descripcion);
        if (!await repository.ActualizarDetallesAsync(tarea, cancellationToken))
            return null; // Puede haber sido eliminada desde que fue consultada.

        // Devuelve el estado persistido, incluyendo los campos que este caso de uso no edita.
        var actualizada = await repository.ObtenerPorIdAsync(id, cancellationToken);
        return actualizada is null ? null : TareaDto.Desde(actualizada);
    }

    public Task<bool> EliminarAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.EliminarAsync(id, cancellationToken);
}
