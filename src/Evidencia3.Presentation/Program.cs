using Evidencia3.Application;
using Evidencia3.Domain;
using Evidencia3.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddScoped<ITareaRepository, TareaRepository>();
builder.Services.AddScoped<FiltrarTareasUseCase>();

var app = builder.Build();
app.MapControllers();
app.Run();
