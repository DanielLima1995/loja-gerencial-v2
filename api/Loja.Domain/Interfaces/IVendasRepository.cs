

using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface IVendasRepository
{
    Task<List<Vendas>> GetVendasAsync(string usuarioId, bool administrador);
    Task<Vendas?> GetVendaByIdAsync(int id, string usuarioId, bool administrador);
    Task<int> AddAsync(Vendas venda);
    Task<Vendas?> UpdateStatusAsync(int id, string status);
    Task<Vendas?> AdvanceStatusAsync(int id, string expectedStatus);
}
