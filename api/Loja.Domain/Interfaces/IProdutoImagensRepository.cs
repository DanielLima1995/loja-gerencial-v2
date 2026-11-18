using Loja.Domain.Entities;

namespace Loja.Domain.Interfaces;

public interface IProdutoImagensRepository
{
    Task<ProdutoImagens?> GetByIdAsync(int id);
    Task<ProdutoImagens> CreateAsync(ProdutoImagens produtoImagens);
    Task RemoveAsync(ProdutoImagens produtoImagens);
}
