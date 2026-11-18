namespace Loja.Domain.Entities;

public sealed class VendasProdutosTags : Entity
{
    protected VendasProdutosTags() { }

    public VendasProdutosTags(int vendaProdutoId, int tagId)
    {
        VendaProdutoId = vendaProdutoId;
        TagId = tagId;
    }

    public int VendaProdutoId { get; private set; }
    public VendasProdutos? VendaProduto { get; private set; }

    public int TagId { get; private set; }









    public Tags? Tag { get; private set; }
}
