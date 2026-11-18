namespace Loja.Application.DTOs;

public class CarrinhoProdutosDTO
{
    public int Id { get; set; }
    public int CarrinhoId { get; set; }
    public int ProdutoId { get; set; }
    public int TamanhoId { get; set; }
    public int Quantidade { get; set; }
    public DateTime DataInclusao { get; set; }
    public int GeneroId { get; set; }
    public int CorId { get; set; }
    public ProdutoDTO? Produto { get; set; }
    public TagItemDTO? Tamanho { get; set; }
    public TagItemDTO? Genero { get; set; }
    public TagItemDTO? Cor { get; set; }
}
