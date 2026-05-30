using ControleLancamentos.Application.Lancamentos.Dtos;

namespace ControleLancamentos.Application.Lancamentos.Abstracoes;

public interface ICriarLancamentoUseCase
{
    Task<CriarLancamentoResponse> ExecutarAsync(
        CriarLancamentoRequest request,
        CancellationToken cancellationToken = default);
}
