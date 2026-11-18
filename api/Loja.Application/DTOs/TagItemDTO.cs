namespace Loja.Application.DTOs;

public class TagItemDTO
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Cor { get; set; }
}
