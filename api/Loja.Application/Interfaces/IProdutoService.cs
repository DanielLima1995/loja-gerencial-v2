using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Pagination;
using Loja.Application.DTOs.Update;

namespace Loja.Application.Interfaces;

public interface IProdutoService
{
    Task<PagedList<ProdutoDTO>> GetProdutos(ProdutoFilterDTO filtro);
    Task<ProdutoDTO?> GetById(int id);
    Task<List<ProdutoDTO>> GetProdutosRelacionados(int? id);
    Task<List<ProdutoDTO>> GetProdutosSemLogin(List<int> produtosIds);
    Task<int> Add(ProdutoDTO produtoDto);
    Task Update(int id, ProdutoUpdateDTO dto);
    Task Remove(int id);
}
