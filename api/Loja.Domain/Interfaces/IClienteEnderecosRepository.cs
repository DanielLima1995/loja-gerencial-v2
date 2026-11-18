using Loja.Domain.Entities;
using Loja.Domain.Parameters;

namespace Loja.Domain.Interfaces;

public interface IClienteEnderecosRepository
{
    Task<List<ClienteEnderecos>> GetClienteEnderecosAsync(string clienteId);
    Task<ClienteEnderecos?> GetByIdAsync(int id, string clienteId);
    Task<int> AddAsync(ClienteEnderecos clienteEndereco);
    Task<bool> DeleteAsync(int id, string clienteId);
}