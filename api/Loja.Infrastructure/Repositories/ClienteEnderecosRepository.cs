using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Loja.Infrastructure.Repositories;

public class ClienteEnderecosRepository : IClienteEnderecosRepository
{
    private readonly ApplicationDbContext _clienteEnderecosContext;

    public ClienteEnderecosRepository(ApplicationDbContext context)
    {
        _clienteEnderecosContext = context;
    }

    public async Task<List<ClienteEnderecos>> GetClienteEnderecosAsync(string clienteId)
    {
        return await _clienteEnderecosContext.ClienteEnderecos
            .AsNoTracking()
            .Where(endereco => endereco.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<ClienteEnderecos?> GetByIdAsync(int id, string clienteId)
    {
        return await _clienteEnderecosContext.ClienteEnderecos
            .AsNoTracking()
            .FirstOrDefaultAsync(endereco => endereco.Id == id && endereco.ClienteId == clienteId);
    }

    public async Task<int> AddAsync(ClienteEnderecos clienteEndereco)
    {
        _clienteEnderecosContext.ClienteEnderecos.Add(clienteEndereco);
        await _clienteEnderecosContext.SaveChangesAsync();
        return clienteEndereco.Id;
    }

    public async Task<bool> DeleteAsync(int id, string clienteId)
    {
        var clienteEndereco = await _clienteEnderecosContext.ClienteEnderecos
            .FirstOrDefaultAsync(endereco => endereco.Id == id && endereco.ClienteId == clienteId);
        if (clienteEndereco == null) return false;
        _clienteEnderecosContext.ClienteEnderecos.Remove(clienteEndereco);
        await _clienteEnderecosContext.SaveChangesAsync();
        return true;
    }
}
