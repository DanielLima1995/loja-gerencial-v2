using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;

namespace Loja.Application.Services;

public class TagsService : ITagsService
{
    private readonly ITagsRepository _tagsRepository;

    public TagsService(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository;
    }

    public async Task<List<TagGroupDTO>> GetTagsAgrupadasAsync(TagsFilterDTO filtroDto)
    {
        var filtro = filtroDto.Adapt<TagsFilter>();
        var tags = await _tagsRepository.GetTagsAsync(filtro);

        return tags
            .GroupBy(t => t.Tipo)
            .Select(grupo => new TagGroupDTO
            {
                Tipo = grupo.Key,
                Tags = grupo.Select(item => new TagItemDTO
                {
                    Id = item.Id,
                    Descricao = item.Descricao,
                    Cor = item.Cor
                }).ToList()
            })
            .OrderBy(g => g.Tipo)
            .ToList();
    }

    public async Task<List<TagItemDTO>> GetAll()
    {
        var tags = await _tagsRepository.GetAllAsync();
        return tags.Adapt<List<TagItemDTO>>();
    }

    public async Task<TagItemDTO?> GetById(int id)
    {
        var tag = await _tagsRepository.GetByIdAsync(id);
        return tag?.Adapt<TagItemDTO>();
    }

    public async Task<int> Add(TagItemInsertDTO tagDto)
    {

        var tag = new Tags(
            tagDto.Tipo,
            tagDto.Descricao,
            tagDto.Cor
        );

        return await _tagsRepository.AddAsync(tag);
    }

    public async Task<bool> Delete(int id)
    {
        return await _tagsRepository.DeleteAsync(id);
    }
}
