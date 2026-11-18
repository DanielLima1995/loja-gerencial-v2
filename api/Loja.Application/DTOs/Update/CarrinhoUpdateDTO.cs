using Loja.Application.DTOs.Insert;

namespace Loja.Application.DTOs.Update;

public class CarrinhoUpdateDTO
{
    public string Status { get; set; } = "Ativo";
    public List<CarrinhoProdutosInsertDTO> Produtos { get; set; } = [];
}
