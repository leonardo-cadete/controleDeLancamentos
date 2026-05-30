using ControleLancamentos.Application.Lancamentos.Abstracoes;
using ControleLancamentos.Application.Lancamentos.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace ControleLancamentos.Application;

public static class InjecaoDependencias
{
    public static IServiceCollection AdicionarAplicacao(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ICriarLancamentoUseCase, CriarLancamentoUseCase>();
        services.AddScoped<ICalcularConsolidadoDiarioUseCase, CalcularConsolidadoDiarioUseCase>();

        return services;
    }
}
