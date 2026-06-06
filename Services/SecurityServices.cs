using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace linkedin_api.Services;

public interface IPasswordHasher { string Hash(string password); bool Verify(string password, string hash); }
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public interface IJwtTokenService
{
    string CreateAccessToken(int userId, string email, string tier, DateTime expiresAt);
    string CreateRefreshToken();
    string HashToken(string token);
    ClaimsPrincipal? Validate(string token);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly string _secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "change-this-development-secret-at-least-32-characters";
    private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "linkedin-api";
    private readonly string _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "linkedin-clone";

    public string CreateAccessToken(int userId, string email, string tier, DateTime expiresAt)
    {
        var header = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new { alg = "HS256", typ = "JWT" }));
        var payload = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object>
        {
            ["sub"] = userId.ToString(),
            ["email"] = email,
            ["tier"] = tier,
            ["iss"] = _issuer,
            ["aud"] = _audience,
            ["exp"] = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
        }));
        var signature = Sign($"{header}.{payload}");
        return $"{header}.{payload}.{signature}";
    }

    public string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
    public string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    public ClaimsPrincipal? Validate(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3 || Sign($"{parts[0]}.{parts[1]}") != parts[2]) return null;
        using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(Base64UrlDecode(parts[1])));
        if (!doc.RootElement.TryGetProperty("exp", out var exp)) return null;
        if (DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64()) <= DateTimeOffset.UtcNow) return null;
        var claims = doc.RootElement.EnumerateObject().Select(p => new Claim(p.Name, p.Value.ToString()));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
    }

    private string Sign(string input)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
        return Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded += new string('=', (4 - padded.Length % 4) % 4);
        return Convert.FromBase64String(padded);
    }
}

public interface ICurrentUser { int UserId { get; } string? Email { get; } }
public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public int UserId => int.TryParse(accessor.HttpContext?.User.FindFirstValue("sub"), out var id) ? id : 1;
    public string? Email => accessor.HttpContext?.User.FindFirstValue("email");
}
