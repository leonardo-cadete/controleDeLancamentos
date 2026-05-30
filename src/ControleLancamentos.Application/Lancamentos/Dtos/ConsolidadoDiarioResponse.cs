namespace ControleLancamentos.Application.Lancamentos.Dtos;

public sealed record ConsolidadoDiarioResponse(
    DateOnly DataReferencia,
    decimal TotalCreditos,
    decimal TotalDebitos,
    decimal Saldo,
    int QuantidadeLancamentos);
