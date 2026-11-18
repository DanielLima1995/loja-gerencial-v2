using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class CarrinhoProdutosRepository : ICarrinhoProdutosRepository
{
    private readonly ApplicationDbContext _carrinhoContext;

    public CarrinhoProdutosRepository(ApplicationDbContext context)
    {
        _carrinhoContext = context;
    }

    public async Task<List<CarrinhoProdutos>> GetProdutosCarrinhoAsync(int carrinhoId)
    {
        return await _carrinhoContext.CarrinhoProdutos
            .AsNoTracking()
            .Where(c => c.CarrinhoId == carrinhoId)
            .ToListAsync();
    }

    public async Task<CarrinhoProdutos?> GetRalacaoCarrinhoProdutoAsync(int produtoId, int carrinhoId)
    {
        return await _carrinhoContext.CarrinhoProdutos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ProdutoId == produtoId && c.CarrinhoId == carrinhoId);
    }

    public async Task<int> AddAsync(CarrinhoProdutos relation)
    {
        _carrinhoContext.CarrinhoProdutos.Add(relation);
        await _carrinhoContext.SaveChangesAsync();
        return relation.Id;
    }

    public async Task<bool> UpdateAsync(CarrinhoProdutos relation)
    {
        _carrinhoContext.CarrinhoProdutos.Update(relation);
        await _carrinhoContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var relation = await _carrinhoContext.CarrinhoProdutos.FindAsync(id);
        if (relation == null) return false;
        _carrinhoContext.CarrinhoProdutos.Remove(relation);
        await _carrinhoContext.SaveChangesAsync();
        return true;
    }
}
