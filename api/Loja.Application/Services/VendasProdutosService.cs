using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;
using Loja.Application.DTOs.Pagination;

namespace Loja.Application.Services;

public class VendasProdutosService : IVendasProdutosService
{
    private readonly IVendasProdutosRepository _vendasProdutosRepository;

    public VendasProdutosService(IVendasProdutosRepository vendasProdutosRepository)
    {
        _vendasProdutosRepository = vendasProdutosRepository;
    }

    public async Task<List<VendasProdutosDTO>> GetProdutos(int vendaId)
    {
        var produtos = await _vendasProdutosRepository.GetProdutosAsync(vendaId);
        return produtos.Adapt<List<VendasProdutosDTO>>();
    }

    public async Task<VendasProdutosDTO?> GetVendaById(int vendaId, int produtoId)
    {
        var vendaProduto = await _vendasProdutosRepository.GetVendaProdutoByIdAsync(vendaId, produtoId);
        return vendaProduto?.Adapt<VendasProdutosDTO>();
    }

    public async Task<int> Add(int vendaId, VendasProdutosInsertDTO vendasInsertDto)
    {
        var vendaProduto = new VendasProdutos(
            vendaId,
            vendasInsertDto.ProdutoId,
            vendasInsertDto.Quantidade,
            vendasInsertDto.ValorUnitario,
            vendasInsertDto.Tamanho,
            vendasInsertDto.Genero,
            vendasInsertDto.Cor
        );

        return await _vendasProdutosRepository.AddAsync(vendaProduto);
    }
}