using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Loja.Infrastructure.Context;
using Loja.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class TagsRepository : ITagsRepository
{
    private readonly ApplicationDbContext _tagsContext;

    public TagsRepository(ApplicationDbContext context)
    {
        _tagsContext = context;
    }

    public async Task<List<Tags>> GetTagsAsync(TagsFilter filtro)
    {
        return await _tagsContext.Tags
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(filtro.Descricao), p => p.Descricao.Contains(filtro.Descricao!))
            .WhereIf(!string.IsNullOrWhiteSpace(filtro.Tipo), p => p.Tipo.Contains(filtro.Tipo!))
            .WhereIf(!string.IsNullOrWhiteSpace(filtro.Cor), p => p.Cor!.Contains(filtro.Cor!))
            .WhereIf(filtro.IsCadastro, p => !p.Tipo.Contains("Preço"))
            .OrderBy(p => p.Descricao)
            .ToListAsync();
    }

    public async Task<Tags?> GetByIdAsync(int id)
    {
        return await _tagsContext.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Tags>> GetAllAsync()
    {
        return await _tagsContext.Tags
            .AsNoTracking()
            .OrderBy(p => p.Descricao)
            .ToListAsync();
    }

    public async Task<int> AddAsync(Tags tag)
    {
        _tagsContext.Tags.Add(tag);
        await _tagsContext.SaveChangesAsync();
        return tag.Id;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await _tagsContext.Tags.FindAsync(id);
        if (tag == null) return false;
        _tagsContext.Tags.Remove(tag);
        await _tagsContext.SaveChangesAsync();
        return true;
    }
}
