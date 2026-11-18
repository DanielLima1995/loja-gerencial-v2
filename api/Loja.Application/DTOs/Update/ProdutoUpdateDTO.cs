using Loja.Application.DTOs.Insert;

namespace Loja.Application.DTOs.Update;

public class ProdutoUpdateDTO
{
    public string Titulo { get; set; } = string.Empty;
    public string? DescricaoBasica { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public bool Ativo { get; set; }
    public List<ProdutoImagemInsertDTO> Imagens { get; set; } = [];
    public List<int> TagIds { get; set; } = [];
}
