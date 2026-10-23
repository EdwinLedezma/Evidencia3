using System.ComponentModel.DataAnnotations;
using Evidencia3.Application.Tareas;
using Microsoft.AspNetCore.Mvc;

namespace Evidencia3.Presentation.Controllers;

public sealed record ModificarTareaRequest(
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(200, ErrorMessage = "El título admite hasta 200 caracteres.")]
    string Titulo,
    [StringLength(2000, ErrorMessage = "La descripción admite hasta 2000 caracteres.")]
    string? Descripcion);

[ApiController]
[Route("api/tareas")]
public sealed class TareasController(TareaService service) : ControllerBase
{
    [HttpPut("{id:guid}")]
    [ProducesResponseType<TareaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaDto>> Modificar(Guid id, ModificarTareaRequest request,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty) return Problem(statusCode: 400, title: "El identificador no puede estar vacío.");
        try
        {
            var tarea = await service.ModificarAsync(id, request.Titulo, request.Descripcion, cancellationToken);
            return tarea is null ? Problem(statusCode: 404, title: "Tarea no encontrada.") : Ok(tarea);
        }
        catch (ArgumentException error)
        {
            return Problem(statusCode: 400, title: "Datos de tarea inválidos.", detail: error.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty) return Problem(statusCode: 400, title: "El identificador no puede estar vacío.");
        return await service.EliminarAsync(id, cancellationToken)
            ? NoContent() : Problem(statusCode: 404, title: "Tarea no encontrada.");
    }
}
