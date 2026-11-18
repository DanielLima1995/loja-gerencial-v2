using Loja.Domain.Validation;

namespace Loja.Domain.Entities;

public sealed class ClienteEnderecos : Entity
{
    protected ClienteEnderecos() { }

    public ClienteEnderecos(string clienteId, string cep, string logradouro, string bairro,
        string cidade, string uf, string numero, string? complemento, string? pontoReferencia)
    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(clienteId), "O ID do cliente é obrigatório.");

        ClienteId = clienteId;

        ValidateDomain(cep, logradouro, bairro, cidade, uf, numero, complemento, pontoReferencia);
    }

    public string ClienteId { get; private set; } = string.Empty;
    public ApplicationUser? Cliente { get; set; }


    public string Cep { get; private set; } = string.Empty;
    public string Logradouro { get; private set; } = string.Empty;
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Uf { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string? Complemento { get; private set; }
    public string? PontoReferencia { get; private set; }
    public void Update(string cep, string logradouro, string bairro,
        string cidade, string uf, string numero, string? complemento, string? pontoReferencia)

    {
        ValidateDomain(cep, logradouro, bairro, cidade, uf, numero, complemento, pontoReferencia);
    }




    private void ValidateDomain(string cep, string logradouro, string bairro,
        string cidade, string uf, string numero, string? complemento, string? pontoReferencia)



    {
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(cep), "O CEP é obrigatório.");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(logradouro), "O logradouro é obrigatório.");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(bairro), "O bairro é obrigatório.");


        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(cidade), "A cidade é obrigatória.");
        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(uf), "A UF é obrigatória.");


        DomainExceptionValidation.When(string.IsNullOrWhiteSpace(numero), "O número é obrigatório.");



        Cep = cep;
        Logradouro = logradouro;


        Bairro = bairro;
        Cidade = cidade;


        Uf = uf;


        Numero = numero;
        Complemento = complemento;


        PontoReferencia = pontoReferencia;
    }
}