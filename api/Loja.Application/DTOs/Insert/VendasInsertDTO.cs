namespace Loja.Application.DTOs.Insert;

public class VendasInsertDTO
{
    public int EnderecoId { get; set; }
    public string FormaPagamento { get; set; } = "Pix";
}
