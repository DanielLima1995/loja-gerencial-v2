namespace Loja.Application.DTOs;

public class VendasProdutosDTO
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public string Tamanho { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public ProdutoDTO? Produto { get; set; }
}
