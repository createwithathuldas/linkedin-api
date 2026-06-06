using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/notifications")]
public class NotificationsController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet] public async Task<ActionResult> Notifications() => await List(Db.Notifications.Where(x => x.UserId == Me).OrderByDescending(x => x.CreatedAt));
    [HttpPut("{id:int}/read")] public async Task<ActionResult> Read(int id) { var n = await Db.Notifications.FindAsync(id); if (n is null) return NotFound(); n.Read = true; await Db.SaveChangesAsync(); return Ok(n); }
    [HttpPut("read-all")] public async Task<ActionResult> ReadAll() { await Db.Notifications.Where(x => x.UserId == Me && !x.Read).ExecuteUpdateAsync(x => x.SetProperty(n => n.Read, true)); return NoContent(); }
    [HttpDelete("{id:int}")] public async Task<ActionResult> DeleteNotification(int id) => await Delete<Notification>(id);
    [HttpGet("preferences")] public async Task<ActionResult> Preferences() => await List(Db.NotificationPreferences.Where(x => x.UserId == Me));
    [HttpPut("preferences")] public async Task<ActionResult<NotificationPreference>> UpdatePreferences(Dictionary<string, object?> body) => await Create<NotificationPreference>(body, x => x.UserId = Me);
    [HttpPost("push-token")] public async Task<ActionResult<PushToken>> PushToken(PushTokenRequest request) => await Create<PushToken>([], x => { x.UserId = Me; x.Token = request.Token; x.Platform = request.Platform; });
}
