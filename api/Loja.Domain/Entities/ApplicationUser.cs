using Microsoft.AspNetCore.Identity;

namespace Loja.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? NomeCompleto { get; set; }
    public string? Documento { get; set; }
    public int TipoUsuarioId { get; } = 2;
    public TiposUsuarios TipoUsuario { get; set; } = null!;
}
