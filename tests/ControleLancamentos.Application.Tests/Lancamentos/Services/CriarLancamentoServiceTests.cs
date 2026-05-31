using ControleLancamentos.Application.Lancamentos.Dtos;
using ControleLancamentos.Application.Lancamentos.Eventos;
using ControleLancamentos.Application.Lancamentos.Repositorios;
using ControleLancamentos.Application.Lancamentos.Services;
using ControleLancamentos.Domain.Lancamentos;

namespace ControleLancamentos.Application.Tests.Lancamentos.Services;

[TestFixture]
public class CriarLancamentoServiceTests
{
    [Test]
    public async Task ExecutarAsync_QuandoDadosForemValidos_DevePersistirEPublicarEvento()
    {
        // Arrange
        var repositorio = new LancamentoRepositorioEmMemoria();
        var eventBus = new EventBusEmMemoria();
        var service = new CriarLancamentoService(repositorio, eventBus);
        var request = new CriarLancamentoRequest
        {
            Valor = 100m,
            Tipo = TipoLancamento.Credito,
            DataLancamento = new DateTime(2026, 5, 30, 10, 0, 0, DateTimeKind.Utc),
            Descricao = "Recebimento"
        };

        // Act
        var response = await service.ExecutarAsync(request);

        // Assert
        Assert.That(repositorio.LancamentosPersistidos, Has.Count.EqualTo(1));
        Assert.That(repositorio.SalvarAlteracoesChamado, Is.True);
        Assert.That(eventBus.EventosPublicados, Has.Count.EqualTo(1));

        var evento = eventBus.EventosPublicados.Single();
        Assert.That(evento.LancamentoId, Is.EqualTo(response.Id));
        Assert.That(evento.Valor, Is.EqualTo(request.Valor));
        Assert.That(evento.Tipo, Is.EqualTo(request.Tipo));
        Assert.That(evento.DataReferencia, Is.EqualTo(DateOnly.FromDateTime(request.DataLancamento)));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void ExecutarAsync_QuandoValorForInvalido_DeveLancarExcecao(decimal valorInvalido)
    {
        // Arrange
        var repositorio = new LancamentoRepositorioEmMemoria();
        var eventBus = new EventBusEmMemoria();
        var service = new CriarLancamentoService(repositorio, eventBus);
        var request = new CriarLancamentoRequest
        {
            Valor = valorInvalido,
            Tipo = TipoLancamento.Debito,
            DataLancamento = new DateTime(2026, 5, 30, 10, 0, 0, DateTimeKind.Utc),
            Descricao = "Teste inválido"
        };

        // Act
        async Task Acao() => await service.ExecutarAsync(request);

        // Assert
        Assert.That(Acao, Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(repositorio.LancamentosPersistidos, Is.Empty);
        Assert.That(eventBus.EventosPublicados, Is.Empty);
    }

    private sealed class LancamentoRepositorioEmMemoria : ILancamentoRepositorio
    {
        public List<Lancamento> LancamentosPersistidos { get; } = new();
        public bool SalvarAlteracoesChamado { get; private set; }

        public Task AdicionarAsync(Lancamento lancamento, CancellationToken cancellationToken = default)
        {
            LancamentosPersistidos.Add(lancamento);
            return Task.CompletedTask;
        }

        public Task AtualizarAsync(Lancamento lancamento, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RemoverAsync(Lancamento lancamento, CancellationToken cancellationToken = default)
        {
            LancamentosPersistidos.Remove(lancamento);
            return Task.CompletedTask;
        }

        public Task<Lancamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var lancamento = LancamentosPersistidos.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(lancamento);
        }

        public Task<IReadOnlyList<Lancamento>> ListarAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Lancamento>>(LancamentosPersistidos.AsReadOnly());
        }

        public Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
        {
            SalvarAlteracoesChamado = true;
            return Task.FromResult(1);
        }
    }

    private sealed class EventBusEmMemoria : ILancamentoCriadoEventBus
    {
        public List<LancamentoCriadoEvent> EventosPublicados { get; } = new();

        public ValueTask PublicarAsync(
            LancamentoCriadoEvent evento,
            CancellationToken cancellationToken = default)
        {
            EventosPublicados.Add(evento);
            return ValueTask.CompletedTask;
        }

        public async IAsyncEnumerable<LancamentoCriadoEvent> ConsumirAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
