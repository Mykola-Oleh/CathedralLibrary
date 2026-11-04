using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
namespace CathedralLibrary.Services;
public sealed class S3CoverStorage(IConfiguration configuration)
{
    public bool Enabled => !string.IsNullOrWhiteSpace(configuration["Storage:S3:Bucket"])
        && !string.IsNullOrWhiteSpace(configuration["Storage:S3:Region"])
        && !string.IsNullOrWhiteSpace(configuration["Storage:CloudFrontBaseUrl"]);

    public async Task<string> UploadAsync(IFormFile file, CancellationToken ct)
    {
        if (!Enabled) throw new InvalidOperationException("Налаштуйте Storage:S3:Bucket, Region та CloudFrontBaseUrl");
        if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Розмір зображення має бути від 1 байта до 5 МБ");
        await using var stream = file.OpenReadStream();
        byte[] signature = new byte[12];
        var read = await stream.ReadAsync(signature.AsMemory(), ct);
        stream.Position = 0;
        var png = read >= 8 && signature.AsSpan(0, 8).SequenceEqual(new byte[] { 137,80,78,71,13,10,26,10 });
        var jpg = read >= 3 && signature[0] == 0xFF && signature[1] == 0xD8 && signature[2] == 0xFF;
        var webp = read >= 12 && System.Text.Encoding.ASCII.GetString(signature, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(signature, 8, 4) == "WEBP";
        if (!png && !jpg && !webp) throw new ArgumentException("Дозволено лише PNG, JPEG або WebP");
        var ext = png ? "png" : jpg ? "jpg" : "webp";
        var mime = png ? "image/png" : jpg ? "image/jpeg" : "image/webp";
        var key = $"covers/{Guid.NewGuid():N}.{ext}";
        using var client = new AmazonS3Client(RegionEndpoint.GetBySystemName(configuration["Storage:S3:Region"]));
        await client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = configuration["Storage:S3:Bucket"],
            Key = key,
            InputStream = stream,
            ContentType = mime,
            AutoCloseStream = false
        }, ct);
        return configuration["Storage:CloudFrontBaseUrl"]!.TrimEnd('/') + "/" + key;
    }
}
