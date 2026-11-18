using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Update;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;

namespace Loja.Application.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly ICarrinhoProdutosRepository _carrinhoProdutosRepository;

    public CarrinhoService(ICarrinhoRepository carrinhoRepository, ICarrinhoProdutosRepository carrinhoProdutosRepository)
    {
        _carrinhoRepository = carrinhoRepository;

        _carrinhoProdutosRepository = carrinhoProdutosRepository;
    }

    public async Task<CarrinhoDTO> GetCarrinhoAsync(string usuarioId)
    {
        var carrinho = await _carrinhoRepository.GetCarrinhoAsync(usuarioId);
        return carrinho.Adapt<CarrinhoDTO>();
    }

    public async Task<int> AddAsync(string usuarioId)
    {

        var carrinho = new Carrinho(
            usuarioId
        );

        return await _carrinhoRepository.AddAsync(carrinho);
    }

    public async Task<bool> UpdateAsync(int id, string usuarioId, List<CarrinhoProdutosInsertDTO> dto)
    {
        var carrinhoBD = await _carrinhoRepository.GetCarrinhoAsync(id, usuarioId);
        
        if(carrinhoBD == null)
        {
            return false;
        }

        var idCarrinho = carrinhoBD.Produtos.Select(x => x.Id).ToList();

        //verifica se algum produto do BD não está na lista de produtos do DTO, se sim, remove do carrinho

        foreach (var produto in carrinhoBD.Produtos)
        {
            if (!dto.Any(x => x.ProdutoId == produto.ProdutoId && x.TamanhoId == produto.TamanhoId && x.Quantidade == produto.Quantidade))
            {
                await _carrinhoProdutosRepository.DeleteAsync(produto.Id);
            }
        }

        //verifica se algum produto do DTO não está na lista de produtos do BD, se sim, adiciona ao carrinho

        foreach (CarrinhoProdutosInsertDTO produto in dto)
        {
            if (!carrinhoBD.Produtos.Any(x => x.ProdutoId == produto.ProdutoId && x.TamanhoId == produto.TamanhoId && x.Quantidade == produto.Quantidade))
            {
                var carrinhoProduto = new CarrinhoProdutos(
                    carrinhoBD.Id,
                    produto.ProdutoId,
                    produto.TamanhoId,
                    produto.Quantidade,
                    DateTime.Now,
                    produto.GeneroId,
                    produto.CorId
                );
                await _carrinhoProdutosRepository.AddAsync(carrinhoProduto);
            }
            else
            {
                var carrinhoProdutoBD = carrinhoBD.Produtos.FirstOrDefault(x => x.ProdutoId == produto.ProdutoId && x.TamanhoId == produto.TamanhoId && x.Quantidade == produto.Quantidade);
                if (carrinhoProdutoBD != null)
                {
                    var carrinhoProduto = new CarrinhoProdutos(
                        carrinhoBD.Id,
                        produto.ProdutoId,
                        produto.TamanhoId,
                        produto.Quantidade,
                        DateTime.Now,
                        produto.GeneroId,
                        produto.CorId
                    );

                    await _carrinhoProdutosRepository.UpdateAsync(carrinhoProduto);
                }
            }
        }

        return true;
    }

    public async Task<bool> DeleteAsync(int id, string usuarioId)
    {
        return await _carrinhoRepository.DeleteAsync(id, usuarioId);
    }
}
