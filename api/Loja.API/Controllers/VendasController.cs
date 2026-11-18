using System.Security.Claims;
using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Pagination;
using Loja.Application.DTOs.Update;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Validation;
using Loja.API.Messaging;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loja.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly IVendasService _vendasService;
    private readonly ICarrinhoService _carrinhoService;
    private readonly IProdutoService _produtoService;
    private readonly ITagsService _tagsService;
    private readonly IVendasProdutosService _vendasProdutosService;
    private readonly IVendasStatusQueue _vendasStatusQueue;

    public VendasController(
        IVendasService vendasService,
        ICarrinhoService carrinhoService,
        ITagsService tagsService,
        IProdutoService produtoService,
        IVendasProdutosService vendasProdutosService,
        IVendasStatusQueue vendasStatusQueue)
    {
        _vendasService = vendasService;
        _carrinhoService = carrinhoService;
        _tagsService = tagsService;
        _produtoService = produtoService;
        _vendasProdutosService = vendasProdutosService;
        _vendasStatusQueue = vendasStatusQueue;
    }

    [Authorize(Roles = "Admin,Cliente")]
    [HttpGet]
    public async Task<ActionResult<PagedList<VendasDTO>>> Get()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var administrador = User.IsInRole("Admin");
        var resultado = await _vendasService.GetVendas(usuarioId, administrador);
        return Ok(resultado);
    }

    [Authorize(Roles = "Admin,Cliente")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VendasDTO>> GetById(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var venda = await _vendasService.GetVendaById(id, usuarioId, User.IsInRole("Admin"));
        if (venda == null) return NotFound();
        return Ok(venda);
    }

    [HttpPost]
    public async Task<IActionResult> Create(VendasInsertDTO vendasInsertDto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        //busca carrinho do usuário

        var carrinho = await _carrinhoService.GetCarrinhoAsync(usuarioId);
        if (carrinho == null)
        {
            return BadRequest(new { mensagem = "Carrinho de compras não encontrado." });
        }

        var tagsBD = await _tagsService.GetAll();

        List<VendasProdutosInsertDTO> vendasProdutosDTOs = new List<VendasProdutosInsertDTO>();

        //resgata os produtos do carrinho e calcula o valor total da venda
        foreach (var item in carrinho.Produtos)
        {
            var produtoBD = await _produtoService.GetById(item.ProdutoId);
            if(produtoBD is not null){

                var tamanhoNome = tagsBD.FirstOrDefault(t => t.Id == item.TamanhoId)?.Descricao;
                var generoNome = tagsBD.FirstOrDefault(t => t.Id == item.GeneroId)?.Descricao;
                var corNome = tagsBD.FirstOrDefault(t => t.Id == item.CorId)?.Descricao;

                if(string.IsNullOrEmpty(tamanhoNome) || string.IsNullOrEmpty(generoNome) || string.IsNullOrEmpty(corNome))
                {
                    return BadRequest(new { mensagem = $"Alguma tag (tamanho, gênero ou cor) não foi encontrada para o produto com ID {item.ProdutoId}." });
                }

                vendasProdutosDTOs.Add(new VendasProdutosInsertDTO
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    ValorUnitario = produtoBD.Valor,
                    Tamanho = tamanhoNome,
                    Genero = generoNome,
                    Cor = corNome
                });
            }
            else
            {
                return BadRequest(new { mensagem = $"Produto com ID {item.ProdutoId} não encontrado." });
            }
        }

        var valorTotal = vendasProdutosDTOs.Sum(vp => vp.Quantidade * vp.ValorUnitario);

        var novoId = await _vendasService.Add(usuarioId, valorTotal, vendasInsertDto);

        if (novoId <= 0 || vendasInsertDto is null)
        {
            return BadRequest(new { mensagem = "Erro ao adicionar a venda." });
        }
        else
        {
            foreach(VendasProdutosInsertDTO vendaProduto in vendasProdutosDTOs)
            {
                await _vendasProdutosService.Add(novoId, vendaProduto);
            }
        }

        await _vendasStatusQueue.PublishAsync(novoId, Vendas.StatusInicial, HttpContext.RequestAborted);

        return Ok(new
        {
            id = novoId,
            mensagem = "Venda realizada com sucesso!"
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] VendasStatusUpdateDTO dto)
    {
        try
        {
            var venda = await _vendasService.Update(id, dto);
            if (venda == null) return NotFound(new { mensagem = "Venda não encontrada." });

            return Ok(new { mensagem = "Venda atualizada com sucesso!", venda.Status });
        }
        catch (DomainExceptionValidation exception)
        {
            return BadRequest(new { mensagem = exception.Message });
        }
    }

}
