using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Infrastructure.Context;

namespace Loja.Infrastructure.Repositories;

public class ProdutoImagensRepository : IProdutoImagensRepository
{
    private readonly ApplicationDbContext _context;

    public ProdutoImagensRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProdutoImagens?> GetByIdAsync(int id)
    {
        return await _context.ProdutoImagens.FindAsync(id);
    }

    public async Task<ProdutoImagens> CreateAsync(ProdutoImagens produtoImagens)
    {
        await _context.ProdutoImagens.AddAsync(produtoImagens);
        await _context.SaveChangesAsync();
        return produtoImagens;
    }

    public async Task RemoveAsync(ProdutoImagens produtoImagens)
    {
        _context.ProdutoImagens.Remove(produtoImagens);
        await _context.SaveChangesAsync();
    }
}
