using ControleLancamentos.Application.Lancamentos.Abstracoes;
using ControleLancamentos.Application.Lancamentos.Dtos;
using ControleLancamentos.Application.Lancamentos.Repositorios;
using ControleLancamentos.Domain.Lancamentos;

namespace ControleLancamentos.Application.Lancamentos.UseCases;

public class CalcularConsolidadoDiarioUseCase(ILancamentoRepositorio repositorio) : ICalcularConsolidadoDiarioUseCase
{
    public async Task<ConsolidadoDiarioResponse> ExecutarAsync(
        ConsolidadoDiarioRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.DataReferencia is null)
        {
            throw new ArgumentException("A data de referência é obrigatória.", nameof(request));
        }

        var lancamentos = await repositorio.ListarAsync(cancellationToken);

        var lancamentosDoDia = lancamentos
            .Where(lancamento => DateOnly.FromDateTime(lancamento.DataLancamento) == request.DataReferencia.Value)
            .ToList();

        var totalCreditos = lancamentosDoDia
            .Where(lancamento => lancamento.Tipo == TipoLancamento.Credito)
            .Sum(lancamento => lancamento.Valor);

        var totalDebitos = lancamentosDoDia
            .Where(lancamento => lancamento.Tipo == TipoLancamento.Debito)
            .Sum(lancamento => lancamento.Valor);

        return new ConsolidadoDiarioResponse(
            request.DataReferencia.Value,
            totalCreditos,
            totalDebitos,
            totalCreditos - totalDebitos,
            lancamentosDoDia.Count);
    }
}
