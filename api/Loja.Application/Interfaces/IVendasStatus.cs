using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Filters;

namespace Loja.Application.Interfaces;

public interface IVendasStatusService
{
    Task<List<VendasStatusDTO>> GetVendasStatusAsync(int vendaId);
    Task<int> Add(VendasStatusInsertDTO vendasStatusDto);
}
