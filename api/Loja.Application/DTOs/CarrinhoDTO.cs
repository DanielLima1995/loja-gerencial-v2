namespace Loja.Application.DTOs;

public class CarrinhoDTO
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string Status { get; set; } = "Ativo";
    public List<CarrinhoProdutosDTO> Produtos { get; set; } = [];
}
