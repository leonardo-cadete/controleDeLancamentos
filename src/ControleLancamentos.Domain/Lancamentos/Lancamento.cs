namespace ControleLancamentos.Domain.Lancamentos;

public class Lancamento
{
    private const int TamanhoMaximoDescricao = 200;

    private Lancamento()
    {
    }

    public Lancamento(Guid id, decimal valor, TipoLancamento tipo, DateTime dataLancamento, string? descricao = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("O identificador do lançamento é obrigatório.", nameof(id));
        }

        Id = id;
        Tipo = ValidarTipo(tipo);
        Valor = ValidarValor(valor);
        DataLancamento = ValidarData(dataLancamento);
        Descricao = NormalizarDescricao(descricao);
    }

    public Guid Id { get; private set; }

    public decimal Valor { get; private set; }

    public TipoLancamento Tipo { get; private set; }

    public DateTime DataLancamento { get; private set; }

    public string? Descricao { get; private set; }

    public decimal ObterValorComSinal()
        => Tipo == TipoLancamento.Credito ? Valor : -Valor;

    public void AtualizarDados(decimal valor, TipoLancamento tipo, DateTime dataLancamento, string? descricao = null)
    {
        Tipo = ValidarTipo(tipo);
        Valor = ValidarValor(valor);
        DataLancamento = ValidarData(dataLancamento);
        Descricao = NormalizarDescricao(descricao);
    }

    private static decimal ValidarValor(decimal valor)
    {
        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor do lançamento deve ser maior que zero.");
        }

        return valor;
    }

    private static TipoLancamento ValidarTipo(TipoLancamento tipo)
    {
        if (!Enum.IsDefined(tipo))
        {
            throw new ArgumentOutOfRangeException(nameof(tipo), "O tipo de lançamento informado é inválido.");
        }

        return tipo;
    }

    private static DateTime ValidarData(DateTime dataLancamento)
    {
        if (dataLancamento == default)
        {
            throw new ArgumentException("A data do lançamento é obrigatória.", nameof(dataLancamento));
        }

        return dataLancamento;
    }

    private static string? NormalizarDescricao(string? descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            return null;
        }

        var descricaoNormalizada = descricao.Trim();

        if (descricaoNormalizada.Length > TamanhoMaximoDescricao)
        {
            throw new ArgumentException(
                $"A descrição do lançamento deve ter no máximo {TamanhoMaximoDescricao} caracteres.",
                nameof(descricao));
        }

        return descricaoNormalizada;
    }
}
