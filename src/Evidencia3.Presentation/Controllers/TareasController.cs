using Microsoft.AspNetCore.Mvc;
using Evidencia3.Application;
using Evidencia3.Domain;

namespace Evidencia3.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TareasController : ControllerBase
{
    private readonly FiltrarTareasUseCase _filtrarUseCase;

    public TareasController(FiltrarTareasUseCase filtrarUseCase)
    {
        _filtrarUseCase = filtrarUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas([FromQuery] string? estado, [FromQuery] string? responsable)
    {
        var tareas = await _filtrarUseCase.EjecutarAsync(estado, responsable);
        return Ok(tareas);
    }
}