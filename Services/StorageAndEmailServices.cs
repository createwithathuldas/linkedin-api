using System.Net;
using System.Net.Mail;
using linkedin_api.DTOs;

namespace linkedin_api.Services;

public interface IMediaStorageService { Task<FileUploadResponse> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken); }
public class LocalMediaStorageService(IWebHostEnvironment env) : IMediaStorageService
{
    public async Task<FileUploadResponse> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken)
    {
        var safeFolder = string.Join("", folder.Where(c => char.IsLetterOrDigit(c) || c is '-' or '_')).ToLowerInvariant();
        var root = Path.Combine(env.ContentRootPath, "SimpleStorage", safeFolder);
        Directory.CreateDirectory(root);
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var path = Path.Combine(root, fileName);
        await using var stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);
        var baseUrl = Environment.GetEnvironmentVariable("MEDIA_BASE_URL") ?? "/media";
        return new FileUploadResponse($"{baseUrl.TrimEnd('/')}/{safeFolder}/{fileName}", file.FileName, file.Length);
    }
}

public interface IEmailService { Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken); }
public class BrevoEmailService : IEmailService
{
    private static readonly HttpClient _httpClient = new();

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey)) return;
        var fromEmail = Environment.GetEnvironmentVariable("BREVO_FROM_EMAIL") ?? "no-reply@linkedin-clone.local";
        var fromName = Environment.GetEnvironmentVariable("BREVO_FROM_NAME") ?? "LinkedIn Clone";

        var payload = new
        {
            sender = new { name = fromName, email = fromEmail },
            to = new[] { new { email = to } },
            subject = subject,
            htmlContent = body
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("api-key", apiKey);
        request.Headers.Add("accept", "application/json");

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Brevo Email Service Failed with status {response.StatusCode}: {errorContent}");
                throw new HttpRequestException($"Brevo SMTP API returned status {response.StatusCode}: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Brevo Email Service Error: {ex.Message}");
            throw;
        }
    }
}
