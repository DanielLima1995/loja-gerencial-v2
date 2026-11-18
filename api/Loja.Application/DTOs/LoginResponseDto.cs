public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public string TipoUsuario { get; set; } = string.Empty;
    public DateTime DataExpiracaoToken { get; set; }
}
