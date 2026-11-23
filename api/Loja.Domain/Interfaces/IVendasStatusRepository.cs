using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface IVendasStatusRepository
{
    Task<List<VendasStatus>> GetVendasStatusAsync(int vendaId);
    Task<int> AddAsync(VendasStatus relation);
}
