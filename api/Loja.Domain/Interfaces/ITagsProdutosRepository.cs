using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface ITagsProdutosRepository
{
    Task<List<TagsProdutos>> GetRalacoesProdutoTagAsync(int produtoId, int tagId);
    Task<TagsProdutos?> GetRalacaoProdutoTagIdAsync(int produtoId, int tagId);
    Task<int> AddAsync(TagsProdutos relation);
    Task<bool> DeleteAsync(int id);
}
