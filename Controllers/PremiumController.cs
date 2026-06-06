using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/premium")]
public class PremiumController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet("plans")] public ActionResult Plans() => Ok(Enum.GetNames<AccountTier>().Where(x => x != nameof(AccountTier.Free)));
    [HttpPost("subscribe")] public async Task<ActionResult> Subscribe(PremiumSubscribeRequest request) { Db.PremiumSubscriptions.Add(new PremiumSubscription { UserId = Me, Tier = request.Tier, ExpiresAt = DateTime.UtcNow.AddMonths(1) }); await SetTier(request.Tier); return Ok(await Db.PremiumSubscriptions.Where(x => x.UserId == Me).OrderByDescending(x => x.Id).FirstAsync()); }
    [HttpPut("upgrade")] public async Task<ActionResult> Upgrade(PremiumSubscribeRequest request) { await SetTier(request.Tier); return Ok(new { tier = request.Tier }); }
    [HttpDelete("cancel")] public async Task<ActionResult> Cancel() { var sub = await Db.PremiumSubscriptions.Where(x => x.UserId == Me && x.Status == SubscriptionStatus.Active).OrderByDescending(x => x.Id).FirstOrDefaultAsync(); if (sub is not null) sub.Status = SubscriptionStatus.Cancelled; await SetTier(AccountTier.Free); return NoContent(); }
    [HttpGet("status")] public async Task<ActionResult> Status() => Ok(await Db.Users.Where(x => x.Id == Me).Select(x => new { x.AccountTier, subscription = Db.PremiumSubscriptions.Where(s => s.UserId == Me).OrderByDescending(s => s.Id).FirstOrDefault() }).FirstOrDefaultAsync());
    [HttpGet("inmail-credits")] public async Task<ActionResult> InMailCredits() { var usage = await Db.InMailUsages.Where(x => x.UserId == Me).OrderByDescending(x => x.Id).FirstOrDefaultAsync(); return Ok(new { credits = usage?.CreditsGranted ?? 0, used = usage?.CreditsUsed ?? 0, remaining = Math.Max(0, (usage?.CreditsGranted ?? 0) - (usage?.CreditsUsed ?? 0)) }); }
    [HttpGet("features")] public async Task<ActionResult> Features() => Ok(await Db.PremiumFeatureGates.AsNoTracking().ToListAsync());
    private async Task SetTier(AccountTier tier) { var user = await Db.Users.FindAsync(Me); if (user is not null) user.AccountTier = tier; await Db.SaveChangesAsync(); }
}
