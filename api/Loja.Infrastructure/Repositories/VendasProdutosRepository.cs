using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Loja.Infrastructure.Context;
using Loja.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class VendasProdutosRepository : IVendasProdutosRepository
{
    private readonly ApplicationDbContext _vendasContext;

    public VendasProdutosRepository(ApplicationDbContext context)
    {
        _vendasContext = context;
    }

    public async Task<List<VendasProdutos>> GetProdutosAsync(int vendaId)
    {
        return await _vendasContext.VendasProdutos
            .AsNoTracking()
            .Where(vp => vp.VendaId == vendaId)
            .ToListAsync();
    }   
    public async Task<VendasProdutos?> GetVendaProdutoByIdAsync(int vendaId, int produtoId)
    {
        return await _vendasContext.VendasProdutos
            .AsNoTracking()
            .FirstOrDefaultAsync(vp => vp.VendaId == vendaId && vp.ProdutoId == produtoId);
    }
    public async Task<int> AddAsync(VendasProdutos vendaProduto)
    {
        _vendasContext.VendasProdutos.Add(vendaProduto);
        await _vendasContext.SaveChangesAsync();
        return vendaProduto.Id;
    }
}