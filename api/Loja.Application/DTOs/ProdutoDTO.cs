namespace Loja.Application.DTOs;

public class ProdutoDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? DescricaoBasica { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public string? UsuarioId { get; set; }
    public List<ProdutoImagensDTO> Imagens { get; set; } = [];
    public List<int> TagIds { get; set; } = [];
    public List<TagItemDTO> Tags { get; set; } = [];
}
