using linkedin_api.Enums;

namespace linkedin_api.DTOs;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string? Headline, string? Location);
public record LoginRequest(string Email, string Password, string? MfaCode);
public record RefreshRequest(string RefreshToken);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record OAuthRequest(string ProviderUserId, string Email, string? FirstName, string? LastName);
public record MfaVerifyRequest(string Code);
public record VerifyOtpRequest(string Email, string Otp);
public record ResendOtpRequest(string Email);
public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, object User);
public record PremiumSubscribeRequest(AccountTier Tier);
public record ConnectionRequestDto(int UserId, string? Message);
public record ReactionRequest(ReactionType Type);
public record TextRequest(string? Text);
public record ShareRequest(int? RecipientUserId, int? GroupId, int? CompanyId, string? Message);
public record InviteRequest(int UserId, string? Message);
public record StageRequest(ApplicationStage Stage);
public record PushTokenRequest(string Token, string? Platform);
public record FileUploadResponse(string Url, string FileName, long Size);
public record SearchResultDto(int Id, string Type, string? Title, string? Subtitle, string? ImageUrl);
