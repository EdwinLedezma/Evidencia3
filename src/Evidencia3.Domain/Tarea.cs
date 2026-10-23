namespace Evidencia3.Domain;

public class Tarea
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Responsable { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public Prioridad Prioridad { get; set; } = Prioridad.Media;
}