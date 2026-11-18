using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Pagination;

namespace Loja.Application.Interfaces;

public interface IVendasProdutosService
{
    Task<List<VendasProdutosDTO>> GetProdutos(int vendaId);
    Task<VendasProdutosDTO?> GetVendaById(int vendaId, int produtoId);
    Task<int> Add(int vendaId, VendasProdutosInsertDTO vendasInsertDto);
}
