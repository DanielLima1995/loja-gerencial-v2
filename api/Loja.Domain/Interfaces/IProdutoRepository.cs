using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<(IEnumerable<Produto> Items, int TotalCount)> GetProdutosAsync(ProdutoFilter filtro);
    Task<Produto?> GetByIdAsync(int id);
    Task<Produto> CreateAsync(Produto produto);
    Task<IEnumerable<Produto>> GetProdutosRelacionadosAsync(int? produtoId);
    Task<List<Produto>> GetProdutosSemLoginAsync(List<int> produtosIds);
    Task UpdateAsync(Produto produto);
    Task RemoveAsync(Produto produto);
}
