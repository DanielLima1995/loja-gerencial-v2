using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class VendasStatusRepository : IVendasStatusRepository
{
    private readonly ApplicationDbContext _vendasContext;

    public VendasStatusRepository(ApplicationDbContext context)
    {
        _vendasContext = context;
    }

    public async Task<List<VendasStatus>> GetVendasStatusAsync(int vendaId)
    {
        return await _vendasContext.VendasStatus
            .AsNoTracking()
            .Where(v => v.VendaId == vendaId)
            .OrderBy(v => v.DataAtualizacao)
            .ThenBy(v => v.Id)
            .ToListAsync();
    }

    public async Task<int> AddAsync(VendasStatus relation)
    {
        _vendasContext.VendasStatus.Add(relation);
        await _vendasContext.SaveChangesAsync();
        return relation.Id;
    }    
}
