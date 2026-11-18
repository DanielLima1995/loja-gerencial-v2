using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface ICarrinhoRepository
{
    Task<Carrinho?> GetCarrinhoAsync(string usuarioId);
    Task<Carrinho?> GetCarrinhoAsync(int id, string usuarioId);
    Task<int> AddAsync(Carrinho carrinho);
    Task<bool> DeleteAsync(int id, string usuarioId);
}