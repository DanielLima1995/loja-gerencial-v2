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
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagsService _tagsService;

    public TagsController(ITagsService tagsService)
    {
        _tagsService = tagsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TagGroupDTO>>> Get([FromQuery] TagsFilterDTO filtro)
    {
        var tags = await _tagsService.GetTagsAgrupadasAsync(filtro);
        return Ok(tags);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TagItemDTO>> GetById(int id)
    {
        var tag = await _tagsService.GetById(id);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagItemInsertDTO tagsInsertDto)
    {
        var novoId = await _tagsService.Add(tagsInsertDto);

        return Ok(new
        {
            id = novoId,
            mensagem = "Tag criada com sucesso!"
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var resultado = await _tagsService.Delete(id);
        if (!resultado) return NotFound();
        return Ok(new { mensagem = "Tag removida com sucesso!" });
    }
}
