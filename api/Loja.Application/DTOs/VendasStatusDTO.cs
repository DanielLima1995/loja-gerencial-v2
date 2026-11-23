namespace Loja.Application.DTOs;

public class VendasStatusDTO
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public string Status { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
