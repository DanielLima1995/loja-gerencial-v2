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
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly IProdutoImagensService _produtoImagensService;
    private readonly ITagsProdutosService _tagsProdutosService;

    public ProdutosController(IProdutoService produtoService, IProdutoImagensService produtoImagensService, ITagsProdutosService tagsProdutosService)
    {
        _produtoService = produtoService;
        _produtoImagensService = produtoImagensService;
        _tagsProdutosService = tagsProdutosService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<ProdutoDTO>>> Get([FromQuery] ProdutoFilterDTO filtro)
    {
        var resultado = await _produtoService.GetProdutos(filtro);
        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> GetById(int id)
    {
        var produto = await _produtoService.GetById(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpGet("produtosRelacionados")]
    public async Task<ActionResult<List<ProdutoDTO>>> GetProdutosRelacionados([FromQuery] int? id)
    {
        var produtos = await _produtoService.GetProdutosRelacionados(id);
        return Ok(produtos);
    }

    [HttpGet("produtosSemLogin")]
    public async Task<ActionResult<List<ProdutoDTO>>> GetProdutosSemLogin([FromQuery] List<int>? produtosIds)
    {
        var produtos = await _produtoService.GetProdutosSemLogin(produtosIds ?? []);
        return Ok(produtos);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProdutoInsertDTO produtoInsertDto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var produtoDto = produtoInsertDto.Adapt<ProdutoDTO>();
        produtoDto.DataCadastro = DateTime.UtcNow;
        produtoDto.Ativo = true;
        produtoDto.UsuarioId = usuarioId;

        var novoId = await _produtoService.Add(produtoDto);

        if (novoId <= 0 || produtoDto is null)
        {
            return BadRequest(new { mensagem = "Erro ao adicionar o produto." });
        }

        return Ok(new
        {
            id = novoId,
            mensagem = "Produto adicionado com sucesso!"
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProdutoUpdateDTO dto)
    {
        await _produtoService.Update(id, dto);
        return Ok(new { mensagem = "Produto atualizado com sucesso!" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _produtoService.Remove(id);
        return Ok(new { mensagem = "Produto removido com sucesso!" });
    }
}
