using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Loja.Infrastructure.Context;
using Loja.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _productContext;

    public ProdutoRepository(ApplicationDbContext context)
    {
        _productContext = context;
    }

    public async Task<(IEnumerable<Produto> Items, int TotalCount)> GetProdutosAsync(ProdutoFilter filtro)
    {
        var query = _productContext.Produtos
            .AsNoTracking()
            .Include(p => p.Usuario)
            .Include(p => p.Imagens)
            .Include(p => p.TagsProdutos)
                .ThenInclude(tp => tp.Tag)
            .WhereIf(!string.IsNullOrWhiteSpace(filtro.Titulo), p => p.Titulo.Contains(filtro.Titulo!))
            .WhereIf(filtro.PrecoMin.HasValue, p => p.Valor >= filtro.PrecoMin!.Value)
            .WhereIf(filtro.PrecoMax.HasValue, p => p.Valor <= filtro.PrecoMax!.Value)
            .WhereIf(filtro.Ativo.HasValue, p => p.Ativo == filtro.Ativo!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(filtro.UsuarioId), p => p.UsuarioId == filtro.UsuarioId);

        var tagIds = filtro.Tags?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (tagIds != null && tagIds.Count > 0)
        {
            if (tagIds.Count == 1)
            {
                var singleTagId = tagIds[0];
                query = query.Where(p => p.TagsProdutos.Any(tp => tp.TagId == singleTagId));
            }
            else
            {
                var totalTagsSolicitadas = tagIds.Count;
                query = query
                    .Where(p => p.TagsProdutos.Count(tp => tagIds.Contains(tp.TagId)) >= totalTagsSolicitadas)
                    .Where(p => !p.TagsProdutos.Any(tp => !tagIds.Contains(tp.TagId)));
            }
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.Id)
            .Skip((filtro.Page - 1) * filtro.PerPage)
            .Take(filtro.PerPage)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _productContext.Produtos
            .AsNoTracking()
            .Include(p => p.Imagens)
            .Include(p => p.TagsProdutos)
                .ThenInclude(tp => tp.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Produto>> GetProdutosRelacionadosAsync(int? produtoId)
    {
        var query = _productContext.Produtos
            .AsNoTracking()
            .Include(p => p.Imagens)
            .Include(p => p.TagsProdutos)
                .ThenInclude(tp => tp.Tag);

        if (!produtoId.HasValue)
        {
            return await query
                .OrderBy(_ => EF.Functions.Random())
                .Take(8)
                .ToListAsync();
        }

        var produto = await _productContext.Produtos
            .AsNoTracking()
            .Include(p => p.TagsProdutos)
            .FirstOrDefaultAsync(p => p.Id == produtoId.Value);
        if (produto == null || produto.TagsProdutos.Count == 0)
            return new List<Produto>();

        var tagIds = produto.TagsProdutos.Select(tp => tp.TagId).ToList();
        return await query
            .Where(p => p.Id != produtoId.Value && p.TagsProdutos.Any(tp => tagIds.Contains(tp.TagId)))
            .OrderByDescending(p => p.Id)
            .Take(8)
            .ToListAsync();
    }

    public async Task<List<Produto>> GetProdutosSemLoginAsync(List<int> produtosIds)
    {
        if (produtosIds.Count == 0)
            return [];

        var ids = produtosIds.Distinct().ToList();
        return await _productContext.Produtos
            .AsNoTracking()
            .Include(p => p.Imagens)
            .Include(p => p.TagsProdutos)
                .ThenInclude(tp => tp.Tag)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<Produto> CreateAsync(Produto produto)
    {
        await _productContext.Produtos.AddAsync(produto);
        await _productContext.SaveChangesAsync();
        return produto;
    }

    public async Task UpdateAsync(Produto produto)
    {
        _productContext.Produtos.Update(produto);
        await _productContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Produto produto)
    {
        _productContext.Produtos.Remove(produto);
        await _productContext.SaveChangesAsync();
    }
}
