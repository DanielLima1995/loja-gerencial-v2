using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using Loja.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Loja.Infrastructure.Services;

public class S3StorageProvider : IStorageProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _regionEndpoint;

    public S3StorageProvider(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));

        var storageSettings = configuration.GetSection("StorageSettings:S3");
        _bucketName = configuration["StorageSettings:BucketName"]
            ?? storageSettings["BucketName"]
            ?? throw new InvalidOperationException("BucketName não configurado");
        _regionEndpoint = configuration["StorageSettings:Region"]
            ?? storageSettings["RegionEndpoint"]
            ?? "us-east-1";
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        if (fileStream == null) 
            throw new ArgumentNullException(nameof(fileStream));
        if (string.IsNullOrWhiteSpace(fileName)) 
            throw new ArgumentException("FileName não pode estar vazio", nameof(fileName));

        var uniqueFileName = $"imagens/{Guid.NewGuid()}_{SanitizeFileName(fileName)}";

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = uniqueFileName,
            InputStream = fileStream,
            ContentType = contentType,
            Metadata =
            {
                ["x-amz-meta-uploaded"] = DateTime.UtcNow.ToString("O")
            }
        };

        await _s3Client.PutObjectAsync(putRequest);
        return uniqueFileName;
    }

    public async Task DeleteAsync(string fileKey)
    {
        if (string.IsNullOrWhiteSpace(fileKey))
            throw new ArgumentException("FileKey não pode estar vazio", nameof(fileKey));

        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileKey
        };

        await _s3Client.DeleteObjectAsync(deleteRequest);
    }

    public string GetPublicUrl(string fileKey)
    {
        if (string.IsNullOrWhiteSpace(fileKey))
            throw new ArgumentException("FileKey não pode estar vazio", nameof(fileKey));

        var escapedKey = string.Join("/", fileKey.Split('/').Select(Uri.EscapeDataString));
        return $"https://{_bucketName}.s3.{_regionEndpoint}.amazonaws.com/{escapedKey}";
    }

    private static string SanitizeFileName(string fileName)
    {
        return System.Net.WebUtility.UrlEncode(fileName).Replace("%", "_");
    }
}
