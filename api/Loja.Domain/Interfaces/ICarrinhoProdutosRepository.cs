using Loja.Domain.Entities;

namespace Loja.Domain.Interfaces;

public interface ICarrinhoProdutosRepository
{
    Task<List<CarrinhoProdutos>> GetProdutosCarrinhoAsync(int carrinhoId);
    Task<CarrinhoProdutos?> GetRalacaoCarrinhoProdutoAsync(int produtoId, int carrinhoId);
    Task<int> AddAsync(CarrinhoProdutos carrinhoProduto);
    Task<bool> UpdateAsync(CarrinhoProdutos carrinhoProduto);
    Task<bool> DeleteAsync(int id);
}
