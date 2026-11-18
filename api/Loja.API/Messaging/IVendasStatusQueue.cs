namespace Loja.API.Messaging;

public interface IVendasStatusQueue
{
    Task PublishAsync(int vendaId, string expectedStatus, CancellationToken cancellationToken = default);
}
