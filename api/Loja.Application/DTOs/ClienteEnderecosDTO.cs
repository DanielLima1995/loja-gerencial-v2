namespace Loja.Application.DTOs;

public class ClienteEnderecosDTO
{
    public int Id { get; set; }
    public string ClienteId { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade{ get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string? PontoReferencia { get; set; }
}
