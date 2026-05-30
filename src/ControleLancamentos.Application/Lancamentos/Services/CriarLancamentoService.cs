using ControleLancamentos.Application.Lancamentos.Dtos;
using ControleLancamentos.Application.Lancamentos.Repositorios;
using ControleLancamentos.Domain.Lancamentos;

namespace ControleLancamentos.Application.Lancamentos.Services;

public class CriarLancamentoService(ILancamentoRepositorio repositorio) : ICriarLancamentoService
{
    public async Task<CriarLancamentoResponse> ExecutarAsync(
        CriarLancamentoRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        LancamentoRegras.ValidarDados(request.Valor, request.Tipo, request.DataLancamento, request.Descricao);

        var lancamento = LancamentoRegras.Criar(
            Guid.NewGuid(),
            request.Valor,
            request.Tipo,
            request.DataLancamento,
            request.Descricao);

        await repositorio.AdicionarAsync(lancamento, cancellationToken);
        await repositorio.SalvarAlteracoesAsync(cancellationToken);

        return new CriarLancamentoResponse(
            lancamento.Id,
            lancamento.Valor,
            lancamento.Tipo,
            lancamento.DataLancamento,
            lancamento.Descricao);
    }
}
