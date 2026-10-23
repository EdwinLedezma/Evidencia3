using Evidencia3.Domain.Enums;
using Evidencia3.Domain.Exceptions;
namespace Evidencia3.Domain.Entities;
public sealed class Tarea {
 public Guid Id {get; private set;}
 public string Titulo {get; private set;}
 public string? Descripcion {get; private set;}
 public string? Responsable {get; private set;}
 public EstadoTarea Estado {get; private set;}
 public PrioridadTarea Prioridad {get; private set;}
 public Tarea(Guid id,string titulo,string? descripcion,string? responsable,EstadoTarea estado,PrioridadTarea prioridad) {
  if(id==Guid.Empty) throw new ReglaNegocioException("El identificador no es válido.");
  Id=id; Titulo=ValidarTitulo(titulo); Descripcion=ValidarDescripcion(descripcion);
  Responsable=Normalizar(responsable); Estado=estado; Prioridad=prioridad;
 }
 public static Tarea Crear(string titulo,string? descripcion,string? responsable=null) =>
  new(Guid.NewGuid(),titulo,descripcion,responsable,EstadoTarea.Pendiente,PrioridadTarea.Media);
 public void CambiarTitulo(string titulo)=>Titulo=ValidarTitulo(titulo);
 public void CambiarDescripcion(string? descripcion)=>Descripcion=ValidarDescripcion(descripcion);
 public void AsignarResponsable(string? responsable)=>Responsable=Normalizar(responsable);
 public void CambiarEstado(EstadoTarea estado)=>Estado=estado;
 public void CambiarPrioridad(PrioridadTarea prioridad)=>Prioridad=prioridad;
 private static string ValidarTitulo(string titulo) {
  var v=titulo?.Trim();
  if(string.IsNullOrWhiteSpace(v)) throw new ReglaNegocioException("El título es obligatorio.");
  if(v.Length>200) throw new ReglaNegocioException("El título no puede superar los 200 caracteres.");
  return v;
 }
 private static string? ValidarDescripcion(string? descripcion) {
  var v=Normalizar(descripcion);
  if(v is {Length:>2000}) throw new ReglaNegocioException("La descripción no puede superar los 2000 caracteres.");
  return v;
 }
 private static string? Normalizar(string? v)=>string.IsNullOrWhiteSpace(v)?null:v.Trim();
}
