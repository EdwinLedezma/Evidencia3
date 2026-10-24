using Evidencia3.Domain.Entities;
namespace Evidencia3.Domain.Repositories;
public interface ITareaRepository {
 Task<IReadOnlyList<Tarea>> ObtenerTodasAsync(CancellationToken ct=default);
 Task<Tarea?> ObtenerPorIdAsync(Guid id,CancellationToken ct=default);
 Task AgregarAsync(Tarea tarea,CancellationToken ct=default);
}
