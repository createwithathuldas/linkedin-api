using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(ApplicationDbContext db, IPasswordHasher passwordHasher, IJwtTokenService tokens, IEmailService email, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        if (await db.Users.AnyAsync(x => x.Email == request.Email)) return Conflict("Email already registered.");
        var user = new User { Email = request.Email, PasswordHash = passwordHasher.Hash(request.Password), FirstName = request.FirstName, LastName = request.LastName, Headline = request.Headline ?? "", Location = request.Location ?? "", AccountTier = AccountTier.Free, IsEmailVerified = false };
        
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();
        user.OtpCode = otp;
        user.OtpExpiresAt = DateTime.UtcNow.AddMinutes(15);

        db.Users.Add(user);
        await db.SaveChangesAsync();
        db.UserSettings.Add(new UserSettings { UserId = user.Id });
        await db.SaveChangesAsync();

        await email.SendAsync(user.Email, "LinkedIn Clone - Verify Your Email", $"Your OTP code is: <strong>{otp}</strong>. It will expire in 15 minutes.", HttpContext.RequestAborted);

        return Ok(new { message = "Registration successful. Please verify your email with the OTP sent to your inbox.", email = user.Email });
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.IsActive);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash)) return Unauthorized();
        if (!user.IsEmailVerified) return BadRequest("Email not verified. Please verify your email first.");
        if (user.MfaEnabled && string.IsNullOrWhiteSpace(request.MfaCode)) return Unauthorized("MFA code required.");
        user.LastLoginAt = DateTime.UtcNow;
        return Ok(await IssueTokens(user));
    }

    [HttpPost("verify-otp")]
    public async Task<ActionResult<TokenResponse>> VerifyOtp(VerifyOtpRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null) return NotFound("User not found.");
        if (user.IsEmailVerified) return BadRequest("Email is already verified.");
        if (user.OtpCode != request.Otp || user.OtpExpiresAt < DateTime.UtcNow) return BadRequest("Invalid or expired OTP.");

        user.IsEmailVerified = true;
        user.OtpCode = null;
        user.OtpExpiresAt = null;
        await db.SaveChangesAsync();

        user.LastLoginAt = DateTime.UtcNow;
        return Ok(await IssueTokens(user));
    }

    [HttpPost("resend-otp")]
    public async Task<ActionResult> ResendOtp(ResendOtpRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null) return NotFound("User not found.");
        if (user.IsEmailVerified) return BadRequest("Email is already verified.");

        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();
        user.OtpCode = otp;
        user.OtpExpiresAt = DateTime.UtcNow.AddMinutes(15);
        await db.SaveChangesAsync();

        await email.SendAsync(user.Email, "LinkedIn Clone - Verify Your Email", $"Your OTP code is: <strong>{otp}</strong>. It will expire in 15 minutes.", HttpContext.RequestAborted);

        return Ok(new { message = "OTP resent successfully." });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshRequest request)
    {
        var hash = tokens.HashToken(request.RefreshToken);
        var user = await db.Users.FirstOrDefaultAsync(x => x.RefreshTokenHash == hash && x.RefreshTokenExpiresAt > DateTime.UtcNow);
        return user is null ? Unauthorized() : Ok(await IssueTokens(user));
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var user = await db.Users.FindAsync(currentUser.UserId);
        if (user is null) return NoContent();
        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAt = null;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("oauth/{provider}")]
    public async Task<ActionResult<TokenResponse>> OAuth(string provider, OAuthRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null)
        {
            user = new User { Email = request.Email, FirstName = request.FirstName ?? provider, LastName = request.LastName ?? "User", PasswordHash = passwordHasher.Hash(Guid.NewGuid().ToString("N")) };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        return Ok(await IssueTokens(user));
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await email.SendAsync(request.Email, "LinkedIn Clone password reset", "Use your reset token from the API response in development.", HttpContext.RequestAborted);
        return Ok(new { resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null) return NotFound();
        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("mfa/enable")]
    public async Task<ActionResult> EnableMfa()
    {
        var user = await db.Users.FindAsync(currentUser.UserId);
        if (user is null) return NotFound();
        user.MfaSecret = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        user.MfaEnabled = true;
        await db.SaveChangesAsync();
        return Ok(new { user.MfaSecret });
    }

    [HttpPost("mfa/verify")]
    public ActionResult VerifyMfa(MfaVerifyRequest request) => Ok(new { verified = !string.IsNullOrWhiteSpace(request.Code) });

    [HttpDelete("sessions")]
    public async Task<ActionResult> KillSessions() => await Logout();

    private async Task<TokenResponse> IssueTokens(User user)
    {
        var accessExpires = DateTime.UtcNow.AddMinutes(int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_MINUTES") ?? "60"));
        var refresh = tokens.CreateRefreshToken();
        user.RefreshTokenHash = tokens.HashToken(refresh);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(int.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_DAYS") ?? "30"));
        await db.SaveChangesAsync();
        var access = tokens.CreateAccessToken(user.Id, user.Email, user.AccountTier.ToString(), accessExpires);
        return new TokenResponse(access, refresh, accessExpires, new { user.Id, user.Email, user.FirstName, user.LastName, user.AccountTier });
    }
}
