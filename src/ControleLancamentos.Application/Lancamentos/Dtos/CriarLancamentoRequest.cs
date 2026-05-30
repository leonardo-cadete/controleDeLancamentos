using ControleLancamentos.Domain.Lancamentos;

namespace ControleLancamentos.Application.Lancamentos.Dtos;

public sealed record CriarLancamentoRequest(
    decimal Valor,
    TipoLancamento Tipo,
    DateTime DataLancamento,
    string? Descricao);
