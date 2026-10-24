using Evidencia3.Domain.Entities;
using Evidencia3.Domain.Enums;
namespace Evidencia3.Application.Contracts;
public sealed record TareaResponse(Guid Id,string Titulo,string? Descripcion,string? Responsable,EstadoTarea Estado,PrioridadTarea Prioridad) {
 public static TareaResponse DesdeEntidad(Tarea t)=>new(t.Id,t.Titulo,t.Descripcion,t.Responsable,t.Estado,t.Prioridad);
}
