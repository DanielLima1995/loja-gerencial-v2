using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class VendasProdutos : Entity
{
    protected VendasProdutos() { }

    public VendasProdutos(int vendaId, int produtoId, int quantidade, decimal valorUnitario, string tamanho, string genero, string cor)
    {
        DomainExceptionValidation.When(quantidade <= 0, "A quantidade deve ser maior que zero.");
        DomainExceptionValidation.When(valorUnitario < 0, "O valor unitário é inválido.");

        VendaId = vendaId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        Tamanho = tamanho;
        Genero = genero;
        Cor = cor;
    }
    public int VendaId { get; private set; }
    public Vendas? Venda { get; private set; }

    public int ProdutoId { get; private set; }
    public Produto? Produto { get; private set; }

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }

    public string Tamanho { get; private set; }
    public string Genero { get; private set; }
    public string Cor { get; private set; }

    public ICollection<VendasProdutosTags> Tags { get; private set; } = new List<VendasProdutosTags>();

    public void AdicionarTag(int tagId)
    {
        Tags.Add(new VendasProdutosTags(this.Id, tagId));
    }
}
