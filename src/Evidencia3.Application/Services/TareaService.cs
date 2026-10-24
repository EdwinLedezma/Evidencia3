using Evidencia3.Application.Contracts;
using Evidencia3.Domain.Entities;
using Evidencia3.Domain.Repositories;
namespace Evidencia3.Application.Services;
public sealed class TareaService(ITareaRepository repositorio) {
 public async Task<TareaResponse> RegistrarAsync(CrearTareaRequest r,CancellationToken ct=default) {
  ArgumentNullException.ThrowIfNull(r);
  var t=Tarea.Crear(r.Titulo,r.Descripcion,r.Responsable);
  await repositorio.AgregarAsync(t,ct); return TareaResponse.DesdeEntidad(t);
 }
 public async Task<IReadOnlyList<TareaResponse>> ObtenerTodasAsync(CancellationToken ct=default)=>
  (await repositorio.ObtenerTodasAsync(ct)).Select(TareaResponse.DesdeEntidad).ToArray();
 public async Task<TareaResponse?> ObtenerPorIdAsync(Guid id,CancellationToken ct=default) {
  var t=await repositorio.ObtenerPorIdAsync(id,ct); return t is null?null:TareaResponse.DesdeEntidad(t);
 }
}
