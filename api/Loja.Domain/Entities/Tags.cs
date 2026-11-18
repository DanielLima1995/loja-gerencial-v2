namespace Loja.Domain.Entities;

public sealed class Tags : Entity
{
    protected Tags() { }

    public Tags(string tipo, string descricao, string? cor)
    {
        Tipo = tipo;
        Descricao = descricao;
        Cor = cor;
    }

    public string Tipo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public string? Cor { get; private set; }
    public ICollection<TagsProdutos> TagsProdutos { get; private set; } = new List<TagsProdutos>();
}
