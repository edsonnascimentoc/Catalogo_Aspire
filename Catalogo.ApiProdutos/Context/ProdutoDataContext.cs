using Catalogo.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.ApiProdutos.Context;

public class ProdutoDataContext : DbContext
{
    public ProdutoDataContext(DbContextOptions<ProdutoDataContext> options)
        : base(options)
    {
    }
    public DbSet<Produto> Produtos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.Property(e => e.Nome)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.Descricao)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(e => e.ImagemUrl)
                .HasMaxLength(250);

            entity.Property(e => e.Preco)
                 .HasPrecision(10, 2)
                 .IsRequired();
        });
    }
}

public static class Extensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ProdutoDataContext>();

            if (context.Database.EnsureCreated())
            {
                //usar so no desenvolvimento
                DbInitializer.Initialize(context);
            }
            else
            {
                var logger = services.GetRequiredService<ILogger<ProdutoDataContext>>();
                logger.LogWarning("Não foi possível conectar ao banco de dados. Pulando a inicialização...");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<ProdutoDataContext>>();
            logger.LogError(ex, "Erro ao criar os dados para popular o banco!");
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(ProdutoDataContext context)
        {
            if (context.Produtos.Any())
                return;

            var Produtos = new List<Produto>
            {
                new Produto { Nome = "Caderno espiral", Descricao = "Caderno espiral com 100 fôlhas", Preco = 19.99m, ImagemUrl = "produto1.jpg" },
                new Produto { Nome = "Caneta esferográfica", Descricao = "Caneta esferográfica azul", Preco = 5.99m, ImagemUrl = "produto2.jpg" },
                new Produto { Nome = "Lápis borracha", Descricao = "Lápis borracha branco", Preco = 3.99m, ImagemUrl = "produto3.jpg" },
                new Produto { Nome = "Borracha branca", Descricao = "Borracha branca pequena", Preco = 2.99m, ImagemUrl = "produto4.jpg" },
                new Produto { Nome = "Estojo pequeno", Descricao = "Estojo pequeno de tecido amarelo ", Preco = 8.45m, ImagemUrl = "produto5.jpg" },
                new Produto { Nome = "Compasso para desenho", Descricao = "Compasso para desenho pequeno", Preco = 15.35m, ImagemUrl = "produto6.jpg" },
                new Produto { Nome = "Caderno Brochura", Descricao = "Caderno brochura 100 fôlhas azul", Preco = 16.55m, ImagemUrl = "produto7.jpg" },
                new Produto { Nome = "Caderno para desenho", Descricao = "Caderno para desenho com 100 fôlhas", Preco = 18.30m, ImagemUrl = "produto8.jpg" },
                new Produto { Nome = "Lápis preto", Descricao = "Lápis preto pequeno", Preco = 5.99m, ImagemUrl = "Produto9.jpg" },
                new Produto { Nome = "Calculadora simples", Descricao = "Calculadora simples com 4 operações", Preco = 12.55m, ImagemUrl = "produto10.jpg" }
            };

            context.AddRange(Produtos);
            context.SaveChanges();
        }
    }
}
