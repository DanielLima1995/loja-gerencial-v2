namespace Loja.Application.DTOs.Insert;

public class VendasProdutosInsertDTO
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public string Tamanho { get; set; }
    public string Genero { get; set; }
    public string Cor { get; set; }
}
