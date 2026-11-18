using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Filters;

namespace Loja.Application.Interfaces;

public interface ITagsProdutosService
{
    Task<TagProdutoDTO?> GetRalacaoProdutoTagId(int produtoId, int tagId);
    Task<int> Add(TagProdutoInsertDTO tagDto);
    Task<bool> Delete(int id);
}
