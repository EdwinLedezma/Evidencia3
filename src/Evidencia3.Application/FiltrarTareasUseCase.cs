using Evidencia3.Domain;

namespace Evidencia3.Application;

public class FiltrarTareasUseCase
{
    private readonly ITareaRepository _repository;
    
    public FiltrarTareasUseCase(ITareaRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Tarea>> EjecutarAsync(string? estado, string? responsable)
    {
        return _repository.FiltrarAsync(estado, responsable);
    }
}