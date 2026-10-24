using Evidencia3.Application.Contracts;
using Evidencia3.Application.Services;
using Evidencia3.Domain.Entities;
using Evidencia3.Domain.Enums;
using Evidencia3.Domain.Exceptions;
using Evidencia3.Infrastructure.Persistence;
var tests=new (string,Func<Task>)[] {
 ("Registro y valores iniciales",Registro),("Rechaza título vacío",TituloVacio),
 ("Rechaza título mayor a 200",TituloLargo),("Consulta listado y detalle",Consulta),
 ("Guarda y recupera JSON",Persistencia)
};
int failed=0;
foreach(var (name,run) in tests)try{await run();Console.WriteLine($"[OK] {name}");}catch(Exception e){failed++;Console.WriteLine($"[ERROR] {name}: {e.Message}");}
Console.WriteLine($"{tests.Length-failed}/{tests.Length} pruebas correctas.");return failed==0?0:1;
static Task Registro(){var t=Tarea.Crear(" Título "," Descripción ");Check(t.Titulo=="Título"&&t.Descripcion=="Descripción","No normalizó texto.");Check(t.Estado==EstadoTarea.Pendiente&&t.Prioridad==PrioridadTarea.Media,"Valores iniciales incorrectos.");return Task.CompletedTask;}
static Task TituloVacio(){try{Tarea.Crear(" ",null);throw new Exception("Aceptó título vacío.");}catch(ReglaNegocioException){return Task.CompletedTask;}}
static Task TituloLargo(){try{Tarea.Crear(new string('x',201),null);throw new Exception("Aceptó título largo.");}catch(ReglaNegocioException){return Task.CompletedTask;}}
static async Task Consulta(){var p=Temp();try{var s=new TareaService(new JsonTareaRepository(p));var t=await s.RegistrarAsync(new("Consulta","Detalle"));Check((await s.ObtenerTodasAsync()).Count==1,"Listado incorrecto.");Check((await s.ObtenerPorIdAsync(t.Id))?.Id==t.Id,"Detalle incorrecto.");Check(await s.ObtenerPorIdAsync(Guid.NewGuid()) is null,"Id inexistente debe ser null.");}finally{Delete(p);}}
static async Task Persistencia(){var p=Temp();try{var t=Tarea.Crear("Persistencia",null);await new JsonTareaRepository(p).AgregarAsync(t);Check((await new JsonTareaRepository(p).ObtenerPorIdAsync(t.Id))?.Titulo==t.Titulo,"No recuperó del JSON.");}finally{Delete(p);}}
static string Temp()=>Path.Combine(Path.GetTempPath(),$"tareas-{Guid.NewGuid():N}.json");
static void Delete(string p){if(File.Exists(p))File.Delete(p);if(File.Exists(p+".tmp"))File.Delete(p+".tmp");}
static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
