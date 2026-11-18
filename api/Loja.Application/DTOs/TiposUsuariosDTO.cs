namespace Loja.Application.DTOs;

public class TiposUsuariosDTO
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}
