using Microsoft.AspNetCore.Http;

namespace Loja.Application.DTOs.Insert;

public class ProdutoImagemUploadDTO
{
    public IFormFile File { get; set; } = null!;
    public int Referencia { get; set; }
}
