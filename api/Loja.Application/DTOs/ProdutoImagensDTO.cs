namespace Loja.Application.DTOs;

public class ProdutoImagensDTO
{
    public int Id { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;
    public int Referencia { get; set; }
    public int ProdutoId { get; set; }
}
