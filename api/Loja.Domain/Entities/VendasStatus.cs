using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class VendasStatus : Entity
{
    protected VendasStatus() { }

    public static IReadOnlyList<string> StatusEtapas { get; } = Array.AsReadOnly(new[]
    {
        "Em Processamento",
        "Pagamento Aprovado",
        "Separação",
        "Enviado",
        "Em Trânsito",
        "Entregue"
    });

    public VendasStatus(int vendaId, string status)
    {
        DomainExceptionValidation.When(vendaId <= 0, "O ID da venda é obrigatório.");
        DomainExceptionValidation.When(!StatusEtapas.Contains(string.IsNullOrWhiteSpace(status) ? "Em Processamento" : status), "Status inválido.");

        VendaId = vendaId;
        Status = string.IsNullOrWhiteSpace(status) ? "Em Processamento" : status;
        DataAtualizacao = DateTime.UtcNow;
    }

    public VendasStatus(Vendas venda, string status)
    {
        DomainExceptionValidation.When(!StatusEtapas.Contains(string.IsNullOrWhiteSpace(status) ? "Em Processamento" : status), "Status inválido.");

        Venda = venda;
        Status = string.IsNullOrWhiteSpace(status) ? "Em Processamento" : status;
        DataAtualizacao = DateTime.UtcNow;
    }

    public DateTime DataAtualizacao { get; private set; }
    public int VendaId { get; private set; }
    public string Status { get; private set; }
    public Vendas? Venda { get; private set; }

}