using linkedin_api.Data;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/settings")]
public class SettingsController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet] public async Task<ActionResult> All() => Ok(await GetOrCreate());
    [HttpPut("privacy")] public async Task<ActionResult> Privacy(Dictionary<string, object?> body) => await UpdateJson(nameof(UserSettings.PrivacyJson), body);
    [HttpPut("notifications")] public async Task<ActionResult> Notifications(Dictionary<string, object?> body) => await UpdateJson(nameof(UserSettings.NotificationsJson), body);
    [HttpPut("job-seeking")] public async Task<ActionResult> JobSeeking(Dictionary<string, object?> body) => await UpdateJson(nameof(UserSettings.JobSeekingJson), body);
    [HttpPut("advertising")] public async Task<ActionResult> Advertising(Dictionary<string, object?> body) => await UpdateJson(nameof(UserSettings.AdvertisingJson), body);
    [HttpPut("data-privacy")] public async Task<ActionResult> DataPrivacy(Dictionary<string, object?> body) => await UpdateJson(nameof(UserSettings.DataPrivacyJson), body);
    [HttpPut("account")] public async Task<ActionResult> Account(Dictionary<string, object?> body) => await UpdateJson(nameof(UserSettings.AccountJson), body);
    [HttpPost("data-download")] public ActionResult DataDownload() => Ok(new { status = "queued", requestedAt = DateTime.UtcNow });
    [HttpDelete("account")] public async Task<ActionResult> Deactivate() { var s = await GetOrCreate(); s.Deactivated = true; var user = await Db.Users.FindAsync(Me); if (user is not null) user.IsActive = false; await Db.SaveChangesAsync(); return NoContent(); }
    [HttpDelete("account/permanent")] public async Task<ActionResult> DeletePermanent() { var user = await Db.Users.FindAsync(Me); if (user is null) return NotFound(); Db.Remove(user); await Db.SaveChangesAsync(); return NoContent(); }
    private async Task<UserSettings> GetOrCreate() { var settings = await Db.UserSettings.FirstOrDefaultAsync(x => x.UserId == Me); if (settings is not null) return settings; settings = new UserSettings { UserId = Me }; Db.UserSettings.Add(settings); await Db.SaveChangesAsync(); return settings; }
    private async Task<ActionResult> UpdateJson(string property, Dictionary<string, object?> body) { var settings = await GetOrCreate(); typeof(UserSettings).GetProperty(property)!.SetValue(settings, System.Text.Json.JsonSerializer.Serialize(body)); await Db.SaveChangesAsync(); return Ok(settings); }
}
