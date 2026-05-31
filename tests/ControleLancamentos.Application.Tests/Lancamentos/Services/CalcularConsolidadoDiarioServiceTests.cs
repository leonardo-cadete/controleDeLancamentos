using ControleLancamentos.Application.Lancamentos.Dtos;
using ControleLancamentos.Application.Lancamentos.Eventos;
using ControleLancamentos.Application.Lancamentos.Repositorios;
using ControleLancamentos.Application.Lancamentos.Services;
using ControleLancamentos.Domain.Lancamentos;

namespace ControleLancamentos.Application.Tests.Lancamentos.Services;

[TestFixture]
public class CalcularConsolidadoDiarioServiceTests
{
    [Test]
    public void ExecutarAsync_QuandoRequestForNulo_DeveLancarExcecao()
    {
        // Arrange
        var repositorio = new ConsolidadoDiarioRepositorioEmMemoria();
        var service = new CalcularConsolidadoDiarioService(repositorio);

        // Act
        async Task Acao() => await service.ExecutarAsync(null!);

        // Assert
        Assert.That(Acao, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void ExecutarAsync_QuandoDataReferenciaNaoForInformada_DeveLancarExcecao()
    {
        // Arrange
        var repositorio = new ConsolidadoDiarioRepositorioEmMemoria();
        var service = new CalcularConsolidadoDiarioService(repositorio);
        var request = new ConsolidadoDiarioRequest(null);

        // Act
        async Task Acao() => await service.ExecutarAsync(request);

        // Assert
        Assert.That(Acao, Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public async Task ExecutarAsync_QuandoNaoHouverConsolidadoParaAData_DeveRetornarValoresZerados()
    {
        // Arrange
        var dataReferencia = new DateOnly(2026, 5, 30);
        var repositorio = new ConsolidadoDiarioRepositorioEmMemoria();
        var service = new CalcularConsolidadoDiarioService(repositorio);

        // Act
        var resultado = await service.ExecutarAsync(new ConsolidadoDiarioRequest(dataReferencia));

        // Assert
        Assert.That(resultado.DataReferencia, Is.EqualTo(dataReferencia));
        Assert.That(resultado.TotalCreditos, Is.EqualTo(0m));
        Assert.That(resultado.TotalDebitos, Is.EqualTo(0m));
        Assert.That(resultado.Saldo, Is.EqualTo(0m));
        Assert.That(resultado.QuantidadeLancamentos, Is.EqualTo(0));
    }

    [Test]
    public async Task ExecutarAsync_DadoListaDeEventosCreditoEDebitoNaMesmaData_DeveRetornarSaldoAcumuladoCorreto()
    {
        // Arrange
        var dataReferencia = new DateOnly(2026, 5, 30);
        var eventos = new[]
        {
            new LancamentoCriadoEvent(Guid.NewGuid(), dataReferencia, 100m, TipoLancamento.Credito),
            new LancamentoCriadoEvent(Guid.NewGuid(), dataReferencia, 30m, TipoLancamento.Debito)
        };

        var repositorio = new ConsolidadoDiarioRepositorioEmMemoria();
        foreach (var evento in eventos)
        {
            await repositorio.AcumularLancamentoAsync(evento);
        }

        var service = new CalcularConsolidadoDiarioService(repositorio);

        // Act
        var resultado = await service.ExecutarAsync(new ConsolidadoDiarioRequest(dataReferencia));

        // Assert
        Assert.That(resultado.TotalCreditos, Is.EqualTo(100m));
        Assert.That(resultado.TotalDebitos, Is.EqualTo(30m));
        Assert.That(resultado.Saldo, Is.EqualTo(70m));
        Assert.That(resultado.QuantidadeLancamentos, Is.EqualTo(2));
    }

    private sealed class ConsolidadoDiarioRepositorioEmMemoria : IConsolidadoDiarioRepositorio
    {
        private readonly Dictionary<DateOnly, Acumulado> _acumuladosPorData = new();

        public Task<ConsolidadoDiario?> ObterPorDataAsync(
            DateOnly dataReferencia,
            CancellationToken cancellationToken = default)
        {
            if (!_acumuladosPorData.TryGetValue(dataReferencia, out var acumulado))
            {
                return Task.FromResult<ConsolidadoDiario?>(null);
            }

            var consolidado = CriarConsolidadoDiario(
                dataReferencia,
                acumulado.TotalCreditos,
                acumulado.TotalDebitos,
                acumulado.Saldo,
                acumulado.QuantidadeLancamentos);

            return Task.FromResult<ConsolidadoDiario?>(consolidado);
        }

        public Task AcumularLancamentoAsync(
            LancamentoCriadoEvent evento,
            CancellationToken cancellationToken = default)
        {
            if (!_acumuladosPorData.TryGetValue(evento.DataReferencia, out var acumulado))
            {
                acumulado = new Acumulado();
            }

            if (evento.Tipo == TipoLancamento.Credito)
            {
                acumulado.TotalCreditos += evento.Valor;
                acumulado.Saldo += evento.Valor;
            }
            else
            {
                acumulado.TotalDebitos += evento.Valor;
                acumulado.Saldo -= evento.Valor;
            }

            acumulado.QuantidadeLancamentos++;
            _acumuladosPorData[evento.DataReferencia] = acumulado;

            return Task.CompletedTask;
        }

        private static ConsolidadoDiario CriarConsolidadoDiario(
            DateOnly dataReferencia,
            decimal totalCreditos,
            decimal totalDebitos,
            decimal saldo,
            int quantidadeLancamentos)
        {
            var consolidado = (ConsolidadoDiario)Activator.CreateInstance(
                typeof(ConsolidadoDiario),
                nonPublic: true)!;

            DefinirPropriedade(consolidado, nameof(ConsolidadoDiario.DataReferencia), dataReferencia);
            DefinirPropriedade(consolidado, nameof(ConsolidadoDiario.TotalCreditos), totalCreditos);
            DefinirPropriedade(consolidado, nameof(ConsolidadoDiario.TotalDebitos), totalDebitos);
            DefinirPropriedade(consolidado, nameof(ConsolidadoDiario.Saldo), saldo);
            DefinirPropriedade(consolidado, nameof(ConsolidadoDiario.QuantidadeLancamentos), quantidadeLancamentos);

            return consolidado;
        }

        private static void DefinirPropriedade<T>(ConsolidadoDiario consolidado, string nomePropriedade, T valor)
        {
            var propriedade = typeof(ConsolidadoDiario).GetProperty(nomePropriedade);
            propriedade!.SetValue(consolidado, valor);
        }

        private sealed class Acumulado
        {
            public decimal TotalCreditos { get; set; }
            public decimal TotalDebitos { get; set; }
            public decimal Saldo { get; set; }
            public int QuantidadeLancamentos { get; set; }
        }
    }
}
