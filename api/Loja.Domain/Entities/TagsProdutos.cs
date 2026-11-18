namespace Loja.Domain.Entities;

public sealed class TagsProdutos : Entity
{
    protected TagsProdutos() { }

    public TagsProdutos(int produtoId, int tagId)
    {
        ProdutoId = produtoId;
        TagId = tagId;
    }

    public int ProdutoId { get; private set; }
    public Produto? Produto { get; private set; }

    public int TagId { get; private set; }
    public Tags? Tag { get; private set; }
}
