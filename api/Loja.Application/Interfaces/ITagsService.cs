using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Filters;

namespace Loja.Application.Interfaces;

public interface ITagsService
{
    Task<List<TagGroupDTO>> GetTagsAgrupadasAsync(TagsFilterDTO filtroDto);
    Task<List<TagItemDTO>> GetAll();
    Task<TagItemDTO?> GetById(int id);
    Task<int> Add(TagItemInsertDTO tagDto);
    Task<bool> Delete(int id);
}
