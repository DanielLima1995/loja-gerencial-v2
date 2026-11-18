using Microsoft.AspNetCore.Identity;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(string email, string password);
    Task<IdentityResult> RegisterAsync(string email, string password, string name, string documento);
    void RevokeToken(string token);
}