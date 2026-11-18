using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;
using Loja.Application.DTOs.Update;

namespace Loja.Application.Interfaces;

public interface ICarrinhoService
{
    Task<CarrinhoDTO> GetCarrinhoAsync(string usuarioId);
    Task<int> AddAsync(string usuarioId);
    Task<bool> UpdateAsync(int id, string usuarioId, List<CarrinhoProdutosInsertDTO> dto);
    Task<bool> DeleteAsync(int id, string usuarioId);
}
