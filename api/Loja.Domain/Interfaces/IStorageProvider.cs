namespace Loja.Domain.Interfaces;

public interface IStorageProvider
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string fileKey);
    string GetPublicUrl(string fileKey);
}
