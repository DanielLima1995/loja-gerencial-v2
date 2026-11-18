using System.Security.Claims;
using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Pagination;
using Loja.Application.DTOs.Update;
using Loja.Application.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loja.API.Controllers;

[ApiController]
[Authorize(Roles = "Cliente")]
[Route("api/[controller]")]
public class CarrinhoController : ControllerBase
{
    private readonly ICarrinhoService _carrinhoService;
    private readonly ICarrinhoProdutosService _carrinhoProdutosService;
    private readonly ITagsService _tagsService;

    public CarrinhoController(ICarrinhoService carrinhoService, ICarrinhoProdutosService carrinhoProdutosService, ITagsService tagsService)
    {
        _carrinhoService = carrinhoService;
        _carrinhoProdutosService = carrinhoProdutosService;
        _tagsService = tagsService;
    }

    [HttpGet]
    public async Task<ActionResult<CarrinhoDTO>> GetMeuCarrinho()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var resultado = await _carrinhoService.GetCarrinhoAsync(usuarioId);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarrinhoInsertDTO carrinhoInsertDto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        List<TagItemDTO> tags = new List<TagItemDTO>();
        tags = await _tagsService.GetAll();

        //verifica se o usuario já possui um carrinho de compras ativo, inativa o carrinho existente antes de criar um novo
        var carrinhoExistente = await _carrinhoService.GetCarrinhoAsync(usuarioId);
        
        if (carrinhoExistente != null)
        {
            await _carrinhoService.DeleteAsync(carrinhoExistente.Id, usuarioId);
        }

        var novoId = await _carrinhoService.AddAsync(usuarioId);

        if (novoId <= 0)
        {
            return BadRequest(new { mensagem = "Erro ao criar o carrinho." });
        }
        else
        {
            foreach(CarrinhoProdutosInsertDTO carrinhoProduto in carrinhoInsertDto.Produtos)
            {
                await _carrinhoProdutosService.Add(novoId, carrinhoProduto);
            }
        }

        return Ok(new
        {
            id = novoId,
            mensagem = "Carrinho criado com sucesso!"
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] List<CarrinhoProdutosInsertDTO> dto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var atualizado = await _carrinhoService.UpdateAsync(id, usuarioId, dto);
        if (!atualizado) return NotFound();

        return Ok(new { mensagem = "Carrinho atualizado com sucesso!" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var removido = await _carrinhoService.DeleteAsync(id, usuarioId);
        if (!removido) return NotFound();

        return Ok(new { mensagem = "Carrinho removido com sucesso!" });
    }
}
