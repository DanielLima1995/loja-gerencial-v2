using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Pagination;
using Loja.Application.DTOs.Update;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;

namespace Loja.Application.Services;

public class VendasService : IVendasService
{
    private readonly IVendasRepository _vendasRepository;

    public VendasService(IVendasRepository vendasRepository)
    {
        _vendasRepository = vendasRepository;
    }

    public async Task<PagedList<VendasDTO>> GetVendas(string usuarioId, bool administrador)
    {
        var vendas = await _vendasRepository.GetVendasAsync(usuarioId, administrador);
        var vendasDto = vendas.Adapt<List<VendasDTO>>();
        var perPage = Math.Max(vendasDto.Count, 1);

        return new PagedList<VendasDTO>(vendasDto, vendasDto.Count, 1, perPage);
    }

    public async Task<VendasDTO?> GetVendaById(int id, string usuarioId, bool administrador)
    {
        var venda = await _vendasRepository.GetVendaByIdAsync(id, usuarioId, administrador);
        return venda?.Adapt<VendasDTO>();
    }

    public async Task<int> Add(string usuarioId, decimal valorTotal, VendasInsertDTO vendasInsertDto)
    {
        var venda = new Vendas(
            usuarioId,
            vendasInsertDto.EnderecoId,
            Vendas.StatusInicial,
            vendasInsertDto.FormaPagamento,
            valorTotal,
            DateTime.Now
        );

        return await _vendasRepository.AddAsync(venda);
    }

    public async Task<VendasDTO?> Update(int id, VendasStatusUpdateDTO status)
    {
        var venda = await _vendasRepository.UpdateStatusAsync(id, status.Status);
        return venda?.Adapt<VendasDTO>();
    }

    public async Task<VendasDTO?> AdvanceStatus(int id, string expectedStatus)
    {
        var venda = await _vendasRepository.AdvanceStatusAsync(id, expectedStatus);
        return venda?.Adapt<VendasDTO>();
    }
}