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
    private readonly IStorageProvider _storageProvider;

    public ProdutoImagensService(IProdutoImagensRepository productImagensRepository, IStorageProvider storageProvider)
    {
        _produtoImagensRepository = productImagensRepository
            ?? throw new ArgumentNullException(nameof(productImagensRepository));
        _storageProvider = storageProvider
            ?? throw new ArgumentNullException(nameof(storageProvider));
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

    public async Task<int> AddFromStreamAsync(int produtoId, Stream fileStream, string fileName, string contentType, int referencia)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName não pode estar vazio", nameof(fileName));
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("ContentType não pode estar vazio", nameof(contentType));

        try
        {
            var s3Key = await _storageProvider.UploadAsync(fileStream, fileName, contentType);
            var publicUrl = _storageProvider.GetPublicUrl(s3Key);

            var imagem = new ProdutoImagens(produtoId, publicUrl, referencia);
            var imagemCriada = await _produtoImagensRepository.CreateAsync(imagem);
            return imagemCriada.Id;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao fazer upload da imagem: {ex.Message}", ex);
        }
    }

    public async Task Remove(int id)
    {
        var imagemEntity = await _produtoImagensRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Imagem com ID {id} não encontrada.");

        await _produtoImagensRepository.RemoveAsync(imagemEntity);
    }
}
