using System.Text.Json.Serialization;
using Evidencia3.Application.Tareas;
using Evidencia3.Domain.Tareas;
using Evidencia3.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddScoped<TareaService>();
builder.Services.AddSingleton<ITareaRepository>(_ =>
{
    var ruta = builder.Configuration["Tareas:RutaArchivo"] ?? "App_Data/tareas.json";
    return new JsonTareaRepository(Path.IsPathRooted(ruta)
        ? ruta : Path.Combine(builder.Environment.ContentRootPath, ruta));
});

var app = builder.Build();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
