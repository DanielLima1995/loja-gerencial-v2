using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class TiposUsuarios : Entity
{
    protected TiposUsuarios() { }

    public TiposUsuarios(string descricao, bool ativo = true)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(descricao), "A descrição é obrigatória.");
        Descricao = descricao;
        Ativo = ativo;
    }

    public string Descricao { get; private set; } = string.Empty;
    public bool Ativo { get; private set; } = true;
}
