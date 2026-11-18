

using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface ITagsRepository
{
    Task<List<Tags>> GetTagsAsync(TagsFilter filtro);
    Task<Tags?> GetByIdAsync(int id);
    Task<List<Tags>> GetAllAsync();
    Task<int> AddAsync(Tags tag);
    Task<bool> DeleteAsync(int id);
}
