namespace Loja.Application.DTOs.Insert;

public class TagItemInsertDTO
{
    public string Descricao { get; set; } = string.Empty;
    public string? Cor { get; set; }
    public string Tipo { get; set; } = string.Empty;
}
