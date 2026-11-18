using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var loginResult = await _authService.LoginAsync(dto.Email, dto.Password);

        if (loginResult == null)
            return Unauthorized(new { Mensagem = "Credenciais inválidas." });

        return Ok(loginResult);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(dto.Email, dto.Password, dto.Name, dto.Documento);

        if (result.Succeeded)
            return Ok(new { Mensagem = "Utilizador criado com sucesso!" });

        return BadRequest(result.Errors);
    }

    [Authorize]
    [HttpPost("revoke")]
    public IActionResult RevokeToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { Mensagem = "Token não encontrado." });

        _authService.RevokeToken(token);

        return Ok(new { Mensagem = "Logout efetuado. Token revogado com sucesso." });
    }
}