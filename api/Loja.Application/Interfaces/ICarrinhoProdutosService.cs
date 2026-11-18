using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Filters;

namespace Loja.Application.Interfaces;

public interface ICarrinhoProdutosService
{
    Task<CarrinhoProdutosDTO?> GetRalacaoCarrinhoProduto(int produtoId, int carrinhoId);
    Task<List<CarrinhoProdutosDTO>> GetProdutosCarrinho(int carrinhoId);
    Task<int> Add(int meuCarrinhoId, CarrinhoProdutosInsertDTO carrinhoProdutoDto);
    Task<bool> Delete(int id);
}
