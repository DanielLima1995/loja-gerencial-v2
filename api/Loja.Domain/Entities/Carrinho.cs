using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class Carrinho : Entity
{
    protected Carrinho() { }

    public Carrinho(string usuarioId, string status = "Ativo")
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(usuarioId), "O ID do usuário é obrigatório.");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(status), "O status é obrigatório.");

        UsuarioId = usuarioId;
        Status = status;
    }

    public string UsuarioId { get; private set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }
    public string Status { get; private set; } = "Ativo";

    public ICollection<CarrinhoProdutos> Produtos { get; private set; } = new List<CarrinhoProdutos>();

    public void AlterarStatus(string novoStatus)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(novoStatus), "O status é obrigatório.");
        Status = novoStatus;
    }

    public void AdicionarItem(int produtoId, int tamanhoId, int quantidade, int generoId, int corId)
    {
        Produtos.Add(new CarrinhoProdutos(this.Id, produtoId, tamanhoId, quantidade, DateTime.UtcNow, generoId, corId));
    }
}
