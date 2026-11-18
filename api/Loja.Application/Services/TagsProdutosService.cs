using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;

namespace Loja.Application.Services;

public class TagsProdutosService : ITagsProdutosService
{
    private readonly ITagsProdutosRepository _tagsProdutosRepository;

    public TagsProdutosService(ITagsProdutosRepository tagsProdutosRepository)
    {
        _tagsProdutosRepository = tagsProdutosRepository;
    }

    public async Task<TagProdutoDTO?> GetRalacaoProdutoTagId(int produtoId, int tagId)
    {
        var relation = await _tagsProdutosRepository.GetRalacaoProdutoTagIdAsync(produtoId, tagId);
        return relation?.Adapt<TagProdutoDTO>();
    }

    public async Task<int> Add(TagProdutoInsertDTO tagDto)
    {

        var relation = new TagsProdutos(
            tagDto.ProdutoId,
            tagDto.TagId
        );

        return await _tagsProdutosRepository.AddAsync(relation);
    }

    public async Task<bool> Delete(int id)
    {
        return await _tagsProdutosRepository.DeleteAsync(id);
    }
}
