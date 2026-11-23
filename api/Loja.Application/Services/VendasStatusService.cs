using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;

namespace Loja.Application.Services;

public class VendasStatusService : IVendasStatusService
{
    private readonly IVendasStatusRepository _vendasStatusRepository;

    public VendasStatusService(IVendasStatusRepository vendasStatusRepository)
    {
        _vendasStatusRepository = vendasStatusRepository;
    }

    public async Task<List<VendasStatusDTO>> GetVendasStatusAsync(int vendaId)
    {
        var vendasStatus = await _vendasStatusRepository.GetVendasStatusAsync(vendaId);
        return vendasStatus.Adapt<List<VendasStatusDTO>>();
    }

    public async Task<int> Add(VendasStatusInsertDTO vendasStatusDto)
    {

        var relation = new VendasStatus(
            vendasStatusDto.VendaId,
            vendasStatusDto.Status
        );

        return await _vendasStatusRepository.AddAsync(relation);
    }
}
