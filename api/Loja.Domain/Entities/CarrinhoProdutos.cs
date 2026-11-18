using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class CarrinhoProdutos : Entity
{
    protected CarrinhoProdutos() { }

    public CarrinhoProdutos(int carrinhoId, int produtoId, int tamanhoId, int quantidade, DateTime dataInclusao, int generoId, int corId)
    {
        DomainExceptionValidation.When(quantidade <= 0, "A quantidade deve ser maior que zero.");

        CarrinhoId = carrinhoId;
        ProdutoId = produtoId;
        TamanhoId = tamanhoId;
        Quantidade = quantidade;
        DataInclusao = dataInclusao == default ? DateTime.UtcNow : dataInclusao;
        GeneroId = generoId;
        CorId = corId;

    }

    public int CarrinhoId { get; private set; }
    public Carrinho? Carrinho { get; private set; }

    public int ProdutoId { get; private set; }
    public Produto? Produto { get; private set; }

    
    public int TamanhoId { get; private set; }
    public Tags? Tamanho { get; private set; }

    public int GeneroId { get; private set; }
    public Tags? Genero { get; private set; }
    public int CorId { get; private set; }
    public Tags? Cor { get; private set; }


    public int Quantidade { get; private set; }
    public DateTime DataInclusao { get; private set; }


    public void AtualizarQuantidade(int novaQuantidade)
    {
        DomainExceptionValidation.When(novaQuantidade <= 0, "A quantidade deve ser maior que zero.");
        Quantidade = novaQuantidade;
    }
}
