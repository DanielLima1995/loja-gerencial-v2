using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class Produto : Entity
{
    protected Produto() { }

    public Produto(string titulo, string? descricaoBasica, string descricao, decimal valor, bool ativo,
        DateTime dataCadastro, string usuarioId)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(usuarioId),
            "O ID do usuário responsável pelo cadastro é obrigatório.");

        DomainExceptionValidation.When(dataCadastro == default,
            "Data de cadastro inválida.");

        DataCadastro = dataCadastro;
        UsuarioId = usuarioId;

        ValidateDomain(titulo, descricaoBasica, descricao, valor, ativo);
    }

    public string Titulo { get; private set; } = string.Empty;
    public string? DescricaoBasica { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }
    public string? UsuarioId { get; private set; }

    public ApplicationUser? Usuario { get; set; }
    public ICollection<ProdutoImagens> Imagens { get; private set; } = new List<ProdutoImagens>();
    public ICollection<TagsProdutos> TagsProdutos { get; private set; } = new List<TagsProdutos>();

    public void Update(string titulo, string? descricaoBasica, string descricao, decimal valor, bool ativo)
    {
        ValidateDomain(titulo, descricaoBasica, descricao, valor, ativo);
    }

    private void ValidateDomain(string titulo, string? descricaoBasica, string descricao, decimal valor, bool ativo)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(titulo),
            "Título inválido. O título é obrigatório.");

        DomainExceptionValidation.When(titulo.Length < 3,
            "O título deve ter no mínimo 3 caracteres.");

        DomainExceptionValidation.When(valor < 0,
            "Valor do preço inválido.");

        Titulo = titulo;
        DescricaoBasica = descricaoBasica;
        Descricao = descricao;
        Valor = valor;
        Ativo = ativo;
    }

    public void AdicionarImagem(string imagemUrl, int referencia)
    {
        Imagens.Add(new ProdutoImagens(this.Id, imagemUrl, referencia));
    }

    public void AdicionarTagProduto(int tagId)
    {
        TagsProdutos.Add(new TagsProdutos(this.Id, tagId));
    }
}
