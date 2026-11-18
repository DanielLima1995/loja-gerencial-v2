using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class TagsProdutosRepository : ITagsProdutosRepository
{
    private readonly ApplicationDbContext _tagsContext;

    public TagsProdutosRepository(ApplicationDbContext context)
    {
        _tagsContext = context;
    }

    public async Task<List<TagsProdutos>> GetRalacoesProdutoTagAsync(int produtoId, int tagId)
    {
        return await _tagsContext.TagsProdutos
            .AsNoTracking()
            .Where(t => t.ProdutoId == produtoId && t.TagId == tagId)
            .ToListAsync();
    }

    public async Task<TagsProdutos?> GetRalacaoProdutoTagIdAsync(int produtoId, int tagId)
    {
        return await _tagsContext.TagsProdutos
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.ProdutoId == produtoId && t.TagId == tagId);
    }

    public async Task<int> AddAsync(TagsProdutos relation)
    {
        _tagsContext.TagsProdutos.Add(relation);
        await _tagsContext.SaveChangesAsync();
        return relation.Id;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var relation = await _tagsContext.TagsProdutos.FindAsync(id);
        if (relation == null) return false;
        _tagsContext.TagsProdutos.Remove(relation);
        await _tagsContext.SaveChangesAsync();
        return true;
    }
}
