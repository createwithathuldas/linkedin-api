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
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var user = Environment.GetEnvironmentVariable("BREVO_SMTP_USERNAME");
        var pass = Environment.GetEnvironmentVariable("BREVO_SMTP_PASSWORD");
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass)) return;
        using var client = new SmtpClient(Environment.GetEnvironmentVariable("BREVO_SMTP_HOST") ?? "smtp-relay.brevo.com", int.Parse(Environment.GetEnvironmentVariable("BREVO_SMTP_PORT") ?? "587"))
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(user, pass)
        };
        var fromEmail = Environment.GetEnvironmentVariable("BREVO_FROM_EMAIL") ?? "no-reply@linkedin-clone.local";
        var fromName = Environment.GetEnvironmentVariable("BREVO_FROM_NAME") ?? "LinkedIn Clone";
        using var message = new MailMessage(new MailAddress(fromEmail, fromName), new MailAddress(to)) { Subject = subject, Body = body };
        await client.SendMailAsync(message, cancellationToken);
    }
}
