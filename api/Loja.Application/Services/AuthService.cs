using Loja.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<LoginResponseDto?> LoginAsync(string email, string password)
    {
        // 1. Verifica se o utilizador existe
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        // 2. Verifica se a password está correta
        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid) return null;

        // 3. Gera o token e devolve os dados do login
        var tipoUsuario = await _userManager.Users
            .Where(u => u.Id == user.Id)
            .Select(u => u.TipoUsuario.Descricao)
            .SingleAsync();

        var expiration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationMinutes"]!));
        var token = GenerateJwtToken(user, tipoUsuario, expiration);

        return new LoginResponseDto
        {
            Token = token,
            Email = user.Email!,
            NomeUsuario = user.NomeCompleto ?? user.UserName!,
            TipoUsuario = tipoUsuario,
            DataExpiracaoToken = expiration
        };
    }

    public async Task<IdentityResult> RegisterAsync(string email, string password, string name, string documento)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            NomeCompleto = name,
            Documento = documento
        };

        // O Identity trata do hash da password automaticamente
        return await _userManager.CreateAsync(user, password);
    }

    public void RevokeToken(string token)
    {
        var expiracaoMinutos = double.Parse(_configuration["JwtSettings:ExpirationMinutes"]!);

        // Adiciona o token ao cache (blocklist) pelo tempo exato de vida restante
        _cache.Set(token, "revogado", TimeSpan.FromMinutes(expiracaoMinutos));
    }

    private string GenerateJwtToken(ApplicationUser user, string tipoUsuario, DateTime expiration)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, tipoUsuario),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}