namespace Evidencia3.Domain.Tareas;

public enum EstadoTarea { Pendiente, EnProceso, Terminada }
public enum PrioridadTarea { Baja, Media, Alta }

public sealed class Tarea
{
    public Guid Id { get; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public string? Responsable { get; }
    public EstadoTarea Estado { get; }
    public PrioridadTarea Prioridad { get; }

    public Tarea(Guid id, string titulo, string? descripcion = null,
        string? responsable = null, EstadoTarea estado = EstadoTarea.Pendiente,
        PrioridadTarea prioridad = PrioridadTarea.Media)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("El identificador de la tarea es obligatorio.", nameof(id));
        if (!Enum.IsDefined(estado) || !Enum.IsDefined(prioridad))
            throw new ArgumentException("El estado o la prioridad no son válidos.");

        Id = id;
        Responsable = responsable;
        Estado = estado;
        Prioridad = prioridad;
        ModificarDetalles(titulo, descripcion);
    }

    public void ModificarDetalles(string titulo, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(titulo) || titulo.Length > 200)
            throw new ArgumentException("El título es obligatorio y admite hasta 200 caracteres.", nameof(titulo));
        if (descripcion?.Length > 2000)
            throw new ArgumentException("La descripción admite hasta 2000 caracteres.", nameof(descripcion));

        // Validar todo antes de mutar: un intento inválido no cambia la entidad.
        Titulo = titulo.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
    }
}
