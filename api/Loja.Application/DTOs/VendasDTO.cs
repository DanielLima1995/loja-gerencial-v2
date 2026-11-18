namespace Loja.Application.DTOs;

public class VendasDTO
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public int EnderecoId { get; set; }
    public DateTime DataCompra { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public decimal ValorFinal { get; set; }
    public ClienteEnderecosDTO? Endereco { get; set; }
    public List<VendasProdutosDTO> Produtos { get; set; } = [];
}
