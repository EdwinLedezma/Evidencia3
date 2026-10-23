using System.Text.Json;
using System.Text.Json.Serialization;
using Evidencia3.Domain.Entities;
using Evidencia3.Domain.Repositories;
namespace Evidencia3.Infrastructure.Persistence;
public sealed class JsonTareaRepository : ITareaRepository {
 private static readonly SemaphoreSlim Gate=new(1,1);
 private static readonly JsonSerializerOptions JsonOptions=CrearOpciones();
 private readonly string ruta;
 public JsonTareaRepository(string rutaArchivo) {
  if(string.IsNullOrWhiteSpace(rutaArchivo)) throw new ArgumentException("La ruta es obligatoria.",nameof(rutaArchivo));
  ruta=Path.GetFullPath(rutaArchivo);
 }
 public async Task<IReadOnlyList<Tarea>> ObtenerTodasAsync(CancellationToken ct=default) {
  await Gate.WaitAsync(ct); try{return await LeerAsync(ct);} finally{Gate.Release();}
 }
 public async Task<Tarea?> ObtenerPorIdAsync(Guid id,CancellationToken ct=default)=>(await ObtenerTodasAsync(ct)).FirstOrDefault(t=>t.Id==id);
 public async Task AgregarAsync(Tarea tarea,CancellationToken ct=default) {
  ArgumentNullException.ThrowIfNull(tarea); await Gate.WaitAsync(ct);
  try { var tareas=(await LeerAsync(ct)).ToList(); if(tareas.Any(t=>t.Id==tarea.Id)) throw new InvalidOperationException("Ya existe ese identificador.");
   tareas.Add(tarea); await GuardarAsync(tareas,ct);
  } finally{Gate.Release();}
 }
 private async Task<List<Tarea>> LeerAsync(CancellationToken ct) {
  if(!File.Exists(ruta)) return [];
  await using var s=new FileStream(ruta,FileMode.Open,FileAccess.Read,FileShare.Read,4096,true);
  if(s.Length==0)return [];
  return await JsonSerializer.DeserializeAsync<List<Tarea>>(s,JsonOptions,ct)??[];
 }
 private async Task GuardarAsync(List<Tarea> tareas,CancellationToken ct) {
  var dir=Path.GetDirectoryName(ruta); if(!string.IsNullOrEmpty(dir))Directory.CreateDirectory(dir);
  var tmp=ruta+".tmp";
  await using(var s=new FileStream(tmp,FileMode.Create,FileAccess.Write,FileShare.None,4096,true)) {
   await JsonSerializer.SerializeAsync(s,tareas,JsonOptions,ct); await s.FlushAsync(ct);
  }
  File.Move(tmp,ruta,true);
 }
 private static JsonSerializerOptions CrearOpciones() {
  var o=new JsonSerializerOptions(JsonSerializerDefaults.Web){WriteIndented=true};
  o.Converters.Add(new JsonStringEnumConverter()); return o;
 }
}
