

using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface IVendasProdutosRepository
{
    Task<List<VendasProdutos>> GetProdutosAsync(int vendaId);
    Task<VendasProdutos?> GetVendaProdutoByIdAsync(int vendaId, int produtoId);
    Task<int> AddAsync(VendasProdutos vendaProduto);
}
