using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Loja.Infrastructure.Context;
using Loja.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class VendasRepository : IVendasRepository
{
    private readonly ApplicationDbContext _vendasContext;

    public VendasRepository(ApplicationDbContext context)
    {
        _vendasContext = context;
    }

    public async Task<List<Vendas>> GetVendasAsync(string usuarioId, bool administrador)
    {
        var query = VendasComDetalhes();

        if (!administrador)
        {
            query = query.Where(v => v.UsuarioId == usuarioId);
        }

        return await query.ToListAsync();
    }

    public async Task<Vendas?> GetVendaByIdAsync(int id, string usuarioId, bool administrador)
    {
        var query = VendasComDetalhes();

        if (!administrador)
        {
            query = query.Where(v => v.UsuarioId == usuarioId);
        }

        return await query.FirstOrDefaultAsync(v => v.Id == id);
    }

    private IQueryable<Vendas> VendasComDetalhes()
    {
        return _vendasContext.Vendas
            .AsNoTracking()
            .Include(v => v.Endereco)
            .Include(v => v.Produtos)
                .ThenInclude(vp => vp.Produto)
                    .ThenInclude(p => p!.Imagens)
            .Include(v => v.StatusHistorico)
            .Include(v => v.Produtos)
                .ThenInclude(vp => vp.Produto)
                    .ThenInclude(p => p!.TagsProdutos)
                        .ThenInclude(tp => tp.Tag);
    }

    public async Task<int> AddAsync(Vendas venda)
    {
        _vendasContext.Vendas.Add(venda);
        await _vendasContext.SaveChangesAsync();
        return venda.Id;
    }

    public async Task<Vendas?> UpdateStatusAsync(int id, string status)
    {
        var venda = await _vendasContext.Vendas.FindAsync(id);
        if (venda == null) return null;

        venda.AtualizarStatus(status);
        _vendasContext.VendasStatus.Add(new VendasStatus(venda.Id, venda.Status));

        await _vendasContext.SaveChangesAsync();
        return venda;
    }

    public async Task<Vendas?> AdvanceStatusAsync(int id, string expectedStatus)
    {
        var venda = await _vendasContext.Vendas.FindAsync(id);
        if (venda == null || venda.Status != expectedStatus || !venda.AvancarStatus())
        {
            return null;
        }

        _vendasContext.VendasStatus.Add(new VendasStatus(venda.Id, venda.Status));
        await _vendasContext.SaveChangesAsync();
        return venda;
    }
}