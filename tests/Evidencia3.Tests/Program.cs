using Evidencia3.Domain;

Console.WriteLine("Iniciando pruebas de Joaquin: Prioridades y Filtros...");

var tarea = new Tarea 
{ 
    Titulo = "Implementar filtros en memoria", 
    Estado = "Pendiente",
    Responsable = "Joaquin",
    Prioridad = Prioridad.Alta 
};

if (tarea.Prioridad != Prioridad.Alta)
{
    Console.WriteLine("Fallo: La prioridad Alta no se asignó correctamente.");
    Environment.Exit(1);
}

Console.WriteLine("Éxito: La entidad Tarea soporta correctamente las prioridades asignadas.");
Environment.Exit(0);