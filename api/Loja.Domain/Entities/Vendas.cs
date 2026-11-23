using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class Vendas : Entity
{
    public const string StatusInicial = "Em Processamento";
    public static IReadOnlyList<string> StatusEtapas { get; } = Array.AsReadOnly(new[]
    {
        StatusInicial,
        "Pagamento Aprovado",
        "Separação",
        "Enviado",
        "Em Trânsito",
        "Entregue"
    });

    protected Vendas() { }

    public Vendas(string usuarioId, int enderecoId, string status, string formaPagamento, decimal valorFinal, DateTime dataCompra)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(usuarioId), "O ID do usuário é obrigatório.");
        DomainExceptionValidation.When(enderecoId <= 0, "O endereço de entrega é obrigatório.");
        DomainExceptionValidation.When(valorFinal < 0, "O valor final da venda não pode ser negativo.");
        DomainExceptionValidation.When(!StatusEtapas.Contains(string.IsNullOrWhiteSpace(status) ? StatusInicial : status), "Status inválido.");

        UsuarioId = usuarioId;
        EnderecoId = enderecoId;
        Status = string.IsNullOrWhiteSpace(status) ? StatusInicial : status;
        FormaPagamento = string.IsNullOrWhiteSpace(formaPagamento) ? "Pix" : formaPagamento;
        ValorFinal = valorFinal;
        DataCompra = dataCompra == default ? DateTime.UtcNow : dataCompra;
        AdicionarStatus(Status);
    }

    public string Status { get; private set; } = StatusInicial;
    public string UsuarioId { get; private set; } = string.Empty;
    public ApplicationUser? Usuario { get; private set; }

    public int EnderecoId { get; private set; }
    public ClienteEnderecos? Endereco { get; private set; }

    public DateTime DataCompra { get; private set; }
    public string FormaPagamento { get; private set; } = "Pix";
    public decimal ValorFinal { get; private set; }

    public ICollection<VendasProdutos> Produtos { get; private set; } = new List<VendasProdutos>();
    public ICollection<VendasStatus> StatusHistorico { get; private set; } = new List<VendasStatus>();
    public void AtualizarStatus(string novoStatus)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(novoStatus), "O status é obrigatório.");

        var statusAtualIndex = StatusEtapas.ToList().IndexOf(Status);
        var novoStatusIndex = StatusEtapas.ToList().IndexOf(novoStatus);
        DomainExceptionValidation.When(novoStatusIndex <= statusAtualIndex, "O novo status deve ser posterior ao status atual.");

        Status = novoStatus;
    }

    public bool AvancarStatus()
    {
        var statusAtualIndex = StatusEtapas.ToList().IndexOf(Status);
        if (statusAtualIndex < 0 || statusAtualIndex >= StatusEtapas.Count - 1)
        {
            return false;
        }

        AtualizarStatus(StatusEtapas[statusAtualIndex + 1]);
        return true;
    }

  public void AdicionarProduto(int produtoId, int quantidade, decimal valorUnitario, string tamanho, string genero, string cor)
    {
        var item = new VendasProdutos(this.Id, produtoId, quantidade, valorUnitario, tamanho, genero, cor);
 
        Produtos.Add(item);
    }

    public void AdicionarStatus(string status)
    {
        var novoStatus = new VendasStatus(this, status);
        StatusHistorico.Add(novoStatus);
    }
}




