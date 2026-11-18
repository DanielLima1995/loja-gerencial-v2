using Loja.Application.DTOs;
using Loja.Application.DTOs.Filters;
using Loja.Application.DTOs.Insert;
using Loja.Application.Interfaces;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Domain.Parameters;
using Mapster;

namespace Loja.Application.Services;

public class ClienteEnderecosService : IClienteEnderecosService
{
    private readonly IClienteEnderecosRepository _clienteEnderecosRepository;

    public ClienteEnderecosService(IClienteEnderecosRepository clienteEnderecosRepository)
    {
        _clienteEnderecosRepository = clienteEnderecosRepository;
    }

    public async Task<List<ClienteEnderecosDTO>> GetClienteEnderecosAsync(string usuarioId)
    {
        var clienteEnderecos = await _clienteEnderecosRepository.GetClienteEnderecosAsync(usuarioId);
        var clienteEnderecosDTO = clienteEnderecos.Adapt<List<ClienteEnderecosDTO>>();
        return clienteEnderecosDTO;
    }

    public async Task<ClienteEnderecosDTO?> GetByIdAsync(int id, string usuarioId)
    {
        var clienteEndereco = await _clienteEnderecosRepository.GetByIdAsync(id, usuarioId);
        return clienteEndereco?.Adapt<ClienteEnderecosDTO>();
    }
    public async Task<int> AddAsync(string clienteID, ClienteEnderecosInsertDTO clienteEnderecoDto)
    {

        var clienteEndereco = new ClienteEnderecos(
            clienteID,
            clienteEnderecoDto.Cep,
            clienteEnderecoDto.Logradouro,
            clienteEnderecoDto.Bairro,
            clienteEnderecoDto.Cidade,
            clienteEnderecoDto.Uf,
            clienteEnderecoDto.Numero,
            clienteEnderecoDto.Complemento,
            clienteEnderecoDto.PontoReferencia
        );

        return await _clienteEnderecosRepository.AddAsync(clienteEndereco);
    }

    public async Task<bool> DeleteAsync(int id, string usuarioId)
    {
        return await _clienteEnderecosRepository.DeleteAsync(id, usuarioId);
    }
}
