using ControleLancamentos.Domain.Lancamentos;
using Microsoft.EntityFrameworkCore;

namespace ControleLancamentos.Infrastructure.Persistencia;

public class ControleLancamentosDbContext(DbContextOptions<ControleLancamentosDbContext> options) : DbContext(options)
{
    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configuracoes.LancamentoConfiguracao());

        base.OnModelCreating(modelBuilder);
    }
}
