namespace Loja.API.Messaging;

public sealed record VendaStatusMessage(int VendaId, string ExpectedStatus);
