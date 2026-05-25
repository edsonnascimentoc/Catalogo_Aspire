using Catalogo.ApiProdutos.Context;
using Catalogo.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.ApiProdutos.Endpoints;

public static class ProdutoEndpoints
{
    public static void MapProdutoEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Produtos");

        var random = new Random();
        group.MapGet("/", async (ProdutoDataContext db) =>
        {
            var produtos = await db.Produtos.ToListAsync();
            return produtos;
        })
        .WithName("GetProdutos")        
        .Produces<List<Produto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (int id, ProdutoDataContext db) =>
        {
            return await db.Produtos.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Produto model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetProdutoPorId")
        .Produces<Produto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, Produto Produto, ProdutoDataContext db) =>
        {
            var resultado = await db.Produtos
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, Produto.Id)
                  .SetProperty(m => m.Nome, Produto.Nome)
                  .SetProperty(m => m.Descricao, Produto.Descricao)
                  .SetProperty(m => m.Preco, Produto.Preco)
                  .SetProperty(m => m.ImagemUrl, Produto.ImagemUrl)
                );

            return resultado == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateProduto")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent)
        .RequireAuthorization();

        group.MapPost("/", async (Produto Produto, ProdutoDataContext db) =>
        {
            db.Produtos.Add(Produto);
            await db.SaveChangesAsync();
            return Results.Created($"/api/Produto/{Produto.Id}", Produto);
        })
        .WithName("CreateProduto")
        .Produces<Produto>(StatusCodes.Status201Created)
        .RequireAuthorization();

        group.MapDelete("/{id}", async (int id, ProdutoDataContext db) =>
        {
            var resultado = await db.Produtos
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return resultado == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteProduto")
        .Produces<Produto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
