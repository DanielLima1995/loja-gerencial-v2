namespace Loja.Application.DTOs;

public class TagGroupDTO
{
    public string Tipo { get; set; } = string.Empty;
    public List<TagItemDTO> Tags { get; set; } = [];
}
