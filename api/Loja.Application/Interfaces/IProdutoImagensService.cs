using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;

namespace Loja.Application.Interfaces;

public interface IProdutoImagensService
{
    Task<ProdutoImagensDTO?> GetByIdAsync(int id);
    Task<int> Add(int produtoId, ProdutoImagemInsertDTO produtoImagensDTO);
    Task Remove(int id);
}
