using ControleLancamentos.Domain.Lancamentos;

namespace ControleLancamentos.Application.Lancamentos.Dtos;

public sealed record CriarLancamentoResponse(
    Guid Id,
    decimal Valor,
    TipoLancamento Tipo,
    DateTime DataLancamento,
    string? Descricao);
