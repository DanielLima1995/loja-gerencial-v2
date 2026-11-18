using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Mapster;

namespace Loja.API.Controllers;

[ApiController]
[Authorize(Roles = "Cliente")]
[Route("api/[controller]")]
public class ClienteEnderecosController : ControllerBase
{
    private readonly IClienteEnderecosService _clienteEnderecos;

    public ClienteEnderecosController(IClienteEnderecosService clienteEnderecos)
    {
        _clienteEnderecos = clienteEnderecos;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClienteEnderecosDTO>>> Get()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var clienteEnderecos = await _clienteEnderecos.GetClienteEnderecosAsync(usuarioId);
        return Ok(clienteEnderecos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteEnderecosDTO>> GetById(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var clienteEndereco = await _clienteEnderecos.GetByIdAsync(id, usuarioId);
        if (clienteEndereco == null) return NotFound();
        return Ok(clienteEndereco);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClienteEnderecosInsertDTO clienteEnderecosInsertDto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var novoId = await _clienteEnderecos.AddAsync(usuarioId, clienteEnderecosInsertDto);

        return Ok(new
        {
            id = novoId,
            mensagem = "Endereço criado com sucesso!"
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var resultado = await _clienteEnderecos.DeleteAsync(id, usuarioId);
        if (!resultado) return NotFound();
        return Ok(new { mensagem = "Endereço removido com sucesso!" });
    }
}
