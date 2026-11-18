using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class CarrinhoRepository : ICarrinhoRepository
{
    private readonly ApplicationDbContext _carrinhoContext;

    public CarrinhoRepository(ApplicationDbContext context)
    {
        _carrinhoContext = context;
    }

    public async Task<List<Carrinho>> GetCarrinhosAsync(string usuarioId)
    {
        return await _carrinhoContext.Carrinhos
            .AsNoTracking()
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Produto)
                    .ThenInclude(p => p!.Imagens)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Produto)
                    .ThenInclude(p => p!.TagsProdutos)
                        .ThenInclude(tp => tp.Tag)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Tamanho)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Genero)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Cor)
            .Where(c => c.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<Carrinho?> GetCarrinhoAsync(string usuarioId)
    {
        return await _carrinhoContext.Carrinhos
            .AsNoTracking()
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Produto)
                    .ThenInclude(p => p!.Imagens)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Produto)
                    .ThenInclude(p => p!.TagsProdutos)
                        .ThenInclude(tp => tp.Tag)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Tamanho)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Genero)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Cor)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
    }

    public async Task<Carrinho?> GetCarrinhoAsync(int id, string usuarioId)
    {
        return await _carrinhoContext.Carrinhos
            .AsNoTracking()
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Produto)
                    .ThenInclude(p => p!.Imagens)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Produto)
                    .ThenInclude(p => p!.TagsProdutos)
                        .ThenInclude(tp => tp.Tag)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Tamanho)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Genero)
            .Include(c => c.Produtos)
                .ThenInclude(cp => cp.Cor)
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
    }

    public async Task<int> AddAsync(Carrinho carrinho)
    {
        _carrinhoContext.Carrinhos.Add(carrinho);
        await _carrinhoContext.SaveChangesAsync();
        return carrinho.Id;
    }

    public async Task<bool> DeleteAsync(int id, string usuarioId)
    {
        var carrinho = await _carrinhoContext.Carrinhos
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
        if (carrinho == null) return false;

        _carrinhoContext.Carrinhos.Remove(carrinho);
        await _carrinhoContext.SaveChangesAsync();
        return true;
    }
}
