using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Mapster;

namespace Loja.Application.Services;

public class ProdutoImagensService : IProdutoImagensService
{
    private readonly IProdutoImagensRepository _produtoImagensRepository;

    public ProdutoImagensService(IProdutoImagensRepository productImagensRepository)
    {
        _produtoImagensRepository = productImagensRepository
            ?? throw new ArgumentNullException(nameof(productImagensRepository));
    }

    public async Task<ProdutoImagensDTO?> GetByIdAsync(int id)
    {
        var imagemEntity = await _produtoImagensRepository.GetByIdAsync(id);
        return imagemEntity?.Adapt<ProdutoImagensDTO>();
    }

    public async Task<int> Add(int produtoId, ProdutoImagemInsertDTO produtoImagensDTO)
    {
        var imagem = new ProdutoImagens(
            produtoId,
            produtoImagensDTO.ImagemUrl,
            produtoImagensDTO.Referencia
        );

        var imagemCriada = await _produtoImagensRepository.CreateAsync(imagem);
        return imagemCriada.Id;
    }

    public async Task Remove(int id)
    {
        var imagemEntity = await _produtoImagensRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Imagem com ID {id} não encontrada.");

        await _produtoImagensRepository.RemoveAsync(imagemEntity);
    }
}
