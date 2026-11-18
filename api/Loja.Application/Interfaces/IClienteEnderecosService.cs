using Loja.Application.DTOs;
using Loja.Application.DTOs.Insert;

namespace Loja.Application.Interfaces;

public interface IClienteEnderecosService
{
    Task<List<ClienteEnderecosDTO>> GetClienteEnderecosAsync(string usuarioId);
    Task<ClienteEnderecosDTO?> GetByIdAsync(int id, string usuarioId);
    Task<int> AddAsync(string usuarioId, ClienteEnderecosInsertDTO clienteEnderecoDto);
    Task<bool> DeleteAsync(int id, string usuarioId);
}
