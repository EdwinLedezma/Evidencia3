using Evidencia3.Domain;

Console.WriteLine("Iniciando pruebas de Joaquin: Prioridades y Filtros...");

// Verificación de la entidad compartida con tu propiedad
var tarea = new Tarea 
{ 
    Titulo = "Implementar filtros en memoria", 
    Estado = "Pendiente",
    Responsable = "Joaquin",
    Prioridad = Prioridad.Alta 
};

if (tarea.Prioridad != Prioridad.Alta)
{
    Console.WriteLine("Fallo: La prioridad Alta no se asignó correctamente a la tarea.");
    Environment.Exit(1);
}

Console.WriteLine("Éxito: La entidad Tarea soporta correctamente las prioridades asignadas.");
Environment.Exit(0);