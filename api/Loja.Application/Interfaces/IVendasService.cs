using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Pagination;
using Loja.Application.DTOs.Update;

namespace Loja.Application.Interfaces;

public interface IVendasService
{
    Task<PagedList<VendasDTO>> GetVendas(string usuarioId, bool administrador);
    Task<VendasDTO?> GetVendaById(int id, string usuarioId, bool administrador);
    Task<int> Add(string usuarioId, decimal valorTotal, VendasInsertDTO vendasInsertDto);
    Task<VendasDTO?> Update(int id, VendasStatusUpdateDTO status);
    Task<VendasDTO?> AdvanceStatus(int id, string expectedStatus);
}
