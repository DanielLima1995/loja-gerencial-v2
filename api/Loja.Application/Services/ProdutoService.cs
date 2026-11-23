using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Pagination;
using Loja.Application.DTOs.Update;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Mapster;

namespace Loja.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _productRepository;

    public ProdutoService(IProdutoRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<PagedList<ProdutoDTO>> GetProdutos(ProdutoFilterDTO filtro)
    {
        var (items, totalCount) = await _productRepository.GetProdutosAsync(filtro);
        var produtosDto = items.Select(MapProduto).ToList();

        return new PagedList<ProdutoDTO>(produtosDto, totalCount, filtro.Page, filtro.PerPage);
    }

    public async Task<ProdutoDTO?> GetById(int id)
    {
        var produto = await _productRepository.GetByIdAsync(id);
        return produto is null ? null : MapProduto(produto);
    }

    public async Task<List<ProdutoDTO>> GetProdutosRelacionados(int? produtoId)
    {
        var produtosRelacionados = await _productRepository.GetProdutosRelacionadosAsync(produtoId);
        return produtosRelacionados.Select(MapProduto).ToList();
    }

    public async Task<List<ProdutoDTO>> GetProdutosSemLogin(List<int> produtosIds)
    {
        var produtos = await _productRepository.GetProdutosSemLoginAsync(produtosIds);
        return produtos.Select(MapProduto).ToList();
    }

    private static ProdutoDTO MapProduto(Produto produto)
    {
        var produtoDto = produto.Adapt<ProdutoDTO>();
        produtoDto.TagIds = produto.TagsProdutos.Select(tp => tp.TagId).ToList();
        produtoDto.Tags = produto.TagsProdutos
            .Where(tp => tp.Tag is not null)
            .Select(tp => tp.Tag!.Adapt<TagItemDTO>())
            .ToList();

        return produtoDto;
    }

    public async Task<int> Add(ProdutoDTO produtoDto)
    {
        var produto = new Produto(
            produtoDto.Titulo,
            produtoDto.DescricaoBasica,
            produtoDto.Descricao,
            produtoDto.Valor,
            produtoDto.Ativo,
            produtoDto.DataCadastro,
            produtoDto.UsuarioId ?? string.Empty
        );

        foreach (var tagId in produtoDto.TagIds.Distinct())
            produto.AdicionarTagProduto(tagId);

        var produtoCriado = await _productRepository.CreateAsync(produto);
        return produtoCriado.Id;
    }

    public async Task Update(int id, ProdutoUpdateDTO dto)
    {
        var produto = await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto {id} não encontrado.");

        produto.Update(dto.Titulo, dto.DescricaoBasica, dto.Descricao, dto.Valor, dto.Ativo);

        var tagsAtuaisIds = produto.TagsProdutos.Select(tp => tp.TagId).ToList();
        var tagsParaRemover = produto.TagsProdutos
            .Where(tp => tagsAtuaisIds.Except(dto.TagIds).Contains(tp.TagId))
            .ToList();

        foreach (var tag in tagsParaRemover)
            produto.TagsProdutos.Remove(tag);

        foreach (var novaTagId in dto.TagIds.Except(tagsAtuaisIds))
            produto.AdicionarTagProduto(novaTagId);

        var imagensNoBanco = produto.Imagens.ToDictionary(i => i.ImagemUrl);
        var urlsExistentes = imagensNoBanco.Keys.ToHashSet();

        var imagensRecebidas = dto.Imagens
            .GroupBy(i => i.ImagemUrl)
            .ToDictionary(g => g.Key, g => g.First());
        var urlsEnviadas = imagensRecebidas.Keys.ToHashSet();

        foreach (var url in urlsExistentes.Except(urlsEnviadas))
            produto.Imagens.Remove(imagensNoBanco[url]);

        foreach (var url in urlsEnviadas.Except(urlsExistentes))
        {
            var novaImg = imagensRecebidas[url];
            produto.AdicionarImagem(novaImg.ImagemUrl, novaImg.Referencia);
        }

        foreach (var url in urlsExistentes.Intersect(urlsEnviadas))
        {
            var entidadeAtual = imagensNoBanco[url];
            var dtoAtual = imagensRecebidas[url];
            if (entidadeAtual.Referencia != dtoAtual.Referencia)
                entidadeAtual.Update(produto.Id, entidadeAtual.ImagemUrl, dtoAtual.Referencia);
        }

        await _productRepository.UpdateAsync(produto);
    }

    public async Task Remove(int id)
    {
        var produto = await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Produto {id} não encontrado.");

        await _productRepository.RemoveAsync(produto);
    }
}
