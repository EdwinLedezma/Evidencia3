namespace Evidencia3.Application.Contracts;
public sealed record CrearTareaRequest(string Titulo,string? Descripcion,string? Responsable=null);
