namespace Loja.Domain.Entities;

public sealed class ProdutoImagens : Entity
{
    protected ProdutoImagens() { }

    public ProdutoImagens(int produtoId, string imagemUrl, int referencia)
    {
        ProdutoId = produtoId;
        ImagemUrl = imagemUrl;
        Referencia = referencia;
    }

    public int ProdutoId { get; private set; }
    public string ImagemUrl { get; private set; } = string.Empty;
    public int Referencia { get; private set; }
    public Produto? Produto { get; private set; }

    public void Update(int produtoId, string imagemUrl, int referencia)
    {
        ProdutoId = produtoId;
        ImagemUrl = imagemUrl;
        Referencia = referencia;
    }
}
