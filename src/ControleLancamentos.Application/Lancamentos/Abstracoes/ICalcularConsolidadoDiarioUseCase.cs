using ControleLancamentos.Application.Lancamentos.Dtos;

namespace ControleLancamentos.Application.Lancamentos.Abstracoes;

public interface ICalcularConsolidadoDiarioUseCase
{
    Task<ConsolidadoDiarioResponse> ExecutarAsync(
        ConsolidadoDiarioRequest request,
        CancellationToken cancellationToken = default);
}
