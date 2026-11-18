using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;

namespace Loja.Application.Services;

public class CarrinhoProdutosService : ICarrinhoProdutosService
{
    private readonly ICarrinhoProdutosRepository _carrinhoProdutosRepository;

    public CarrinhoProdutosService(ICarrinhoProdutosRepository carrinhoProdutosRepository)
    {
        _carrinhoProdutosRepository = carrinhoProdutosRepository;
    }

    public async Task<List<CarrinhoProdutosDTO>> GetProdutosCarrinho(int carrinhoId)
    {
        var produtosCarrinho = await _carrinhoProdutosRepository.GetProdutosCarrinhoAsync(carrinhoId);
        return produtosCarrinho.Adapt<List<CarrinhoProdutosDTO>>();
    }

    public async Task<CarrinhoProdutosDTO?> GetRalacaoCarrinhoProduto(int produtoId, int carrinhoId)
    {
        var relation = await _carrinhoProdutosRepository.GetRalacaoCarrinhoProdutoAsync(produtoId, carrinhoId);
        return relation?.Adapt<CarrinhoProdutosDTO>();
    }

    public async Task<int> Add(int meuCarrinhoId, CarrinhoProdutosInsertDTO carrinhoProdutoDto)
    {

        var relation = new CarrinhoProdutos(
            meuCarrinhoId,
            carrinhoProdutoDto.ProdutoId,
            carrinhoProdutoDto.TamanhoId,
            carrinhoProdutoDto.Quantidade,
            DateTime.Now,
            carrinhoProdutoDto.GeneroId,
            carrinhoProdutoDto.CorId
        );

        return await _carrinhoProdutosRepository.AddAsync(relation);
    }

    public async Task<bool> Delete(int id)
    {
        return await _carrinhoProdutosRepository.DeleteAsync(id);
    }
}
