using Evidencia3.Application.Contracts;
using Evidencia3.Application.Services;
using Evidencia3.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
namespace Evidencia3.Presentation.Controllers;
[ApiController]
[Route("api/tareas")]
public sealed class TareasController(TareaService servicio):ControllerBase {
 [HttpPost]
 public async Task<IActionResult> Registrar([FromBody] CrearTareaRequest solicitud,CancellationToken ct) {
  try {var tarea=await servicio.RegistrarAsync(solicitud,ct); return CreatedAtAction(nameof(ObtenerPorId),new{id=tarea.Id},tarea);}
  catch(ReglaNegocioException ex) {ModelState.AddModelError(string.Empty,ex.Message);return ValidationProblem(ModelState);}
 }
 [HttpGet]
 public async Task<ActionResult<IReadOnlyList<TareaResponse>>> ObtenerTodas(CancellationToken ct)=>Ok(await servicio.ObtenerTodasAsync(ct));
 [HttpGet("{id:guid}")]
 public async Task<ActionResult<TareaResponse>> ObtenerPorId(Guid id,CancellationToken ct) {
  var tarea=await servicio.ObtenerPorIdAsync(id,ct);
  if(tarea is null) return NotFound(new{mensaje=$"No se encontró una tarea con el id '{id}'."});
  return Ok(tarea);
 }
}
