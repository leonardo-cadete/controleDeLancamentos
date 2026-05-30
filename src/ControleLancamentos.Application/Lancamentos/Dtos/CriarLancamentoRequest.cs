using System.ComponentModel.DataAnnotations;
using ControleLancamentos.Domain.Lancamentos;

namespace ControleLancamentos.Application.Lancamentos.Dtos;

/// <summary>
/// Dados necessários para criar um lançamento.
/// </summary>
public sealed record CriarLancamentoRequest(
    [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    decimal Valor,
    [property: EnumDataType(typeof(TipoLancamento))]
    TipoLancamento Tipo,
    [property: Required]
    DateTime DataLancamento,
    [property: StringLength(200)]
    string? Descricao);
