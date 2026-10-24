using System.Text.Json.Serialization;
using Evidencia3.Application.Services;
using Evidencia3.Domain.Repositories;
using Evidencia3.Infrastructure.Persistence;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(o=>o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
var configured=builder.Configuration["Tareas:RutaArchivo"];
var ruta=string.IsNullOrWhiteSpace(configured)?Path.Combine(builder.Environment.ContentRootPath,"App_Data","tareas.json"):
 Path.IsPathRooted(configured)?configured:Path.Combine(builder.Environment.ContentRootPath,configured);
builder.Services.AddSingleton<ITareaRepository>(_=>new JsonTareaRepository(ruta));
builder.Services.AddScoped<TareaService>();
var app=builder.Build(); app.MapControllers(); app.Run();
public partial class Program { }
