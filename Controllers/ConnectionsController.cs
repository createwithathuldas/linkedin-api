using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/connections")]
public class ConnectionsController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpPost("request")] public async Task<ActionResult<ConnectionRequest>> SendRequest(ConnectionRequestDto request) => await Create<ConnectionRequest>([], x => { x.RequesterUserId = Me; x.AddresseeUserId = request.UserId; x.Message = request.Message; });
    [HttpPost("request/{id:int}/accept")] public async Task<ActionResult> Accept(int id) { var req = await Db.ConnectionRequests.FindAsync(id); if (req is null) return NotFound(); req.Status = ConnectionRequestStatus.Accepted; Db.Connections.AddRange(new Connection { UserId = req.RequesterUserId, ConnectedUserId = req.AddresseeUserId }, new Connection { UserId = req.AddresseeUserId, ConnectedUserId = req.RequesterUserId }); await Db.SaveChangesAsync(); return Ok(req); }
    [HttpDelete("request/{id:int}")] public async Task<ActionResult> DeleteRequest(int id) => await Delete<ConnectionRequest>(id);
    [HttpDelete("{userId:int}")] public async Task<ActionResult> Disconnect(int userId) { var rows = await Db.Connections.Where(x => (x.UserId == Me && x.ConnectedUserId == userId) || (x.UserId == userId && x.ConnectedUserId == Me)).ToListAsync(); Db.RemoveRange(rows); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet] public async Task<ActionResult> Mine() => await List(Db.Connections.Where(x => x.UserId == Me));
    [HttpGet("received")] public async Task<ActionResult> Received() => await List(Db.ConnectionRequests.Where(x => x.AddresseeUserId == Me && x.Status == ConnectionRequestStatus.Pending));
    [HttpGet("sent")] public async Task<ActionResult> Sent() => await List(Db.ConnectionRequests.Where(x => x.RequesterUserId == Me && x.Status == ConnectionRequestStatus.Pending));
    [HttpGet("suggestions")] public async Task<ActionResult> Suggestions() => await List(Db.ConnectionSuggestions.Where(x => x.UserId == Me && !x.Dismissed).OrderByDescending(x => x.Score));
    [HttpGet("suggestions/{category}")] public async Task<ActionResult> SuggestionsByCategory(SuggestionCategory category) => await List(Db.ConnectionSuggestions.Where(x => x.UserId == Me && x.Category == category && !x.Dismissed));
    [HttpPut("settings")] public async Task<ActionResult> UpdateSettings(Dictionary<string, object?> body) { var settings = await Db.UserSettings.FirstOrDefaultAsync(x => x.UserId == Me) ?? new UserSettings { UserId = Me }; settings.PrivacyJson = System.Text.Json.JsonSerializer.Serialize(body); Db.UserSettings.Update(settings); await Db.SaveChangesAsync(); return Ok(settings); }
    [HttpGet("settings")] public async Task<ActionResult> Settings() => Ok(await Db.UserSettings.FirstOrDefaultAsync(x => x.UserId == Me));
    [HttpPost("suggestions/{userId:int}/dismiss")] public async Task<ActionResult> Dismiss(int userId) { var s = await Db.ConnectionSuggestions.FirstOrDefaultAsync(x => x.UserId == Me && x.SuggestedUserId == userId); if (s is null) return NotFound(); s.Dismissed = true; await Db.SaveChangesAsync(); return Ok(s); }
}

[ApiController]
[Route("api/v1/follow")]
public class FollowController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpPost("{userId:int}")] public async Task<ActionResult<Follow>> Follow(int userId) => await Create<Follow>([], x => { x.FollowerUserId = Me; x.FollowedUserId = userId; });
    [HttpDelete("{userId:int}")] public async Task<ActionResult> Unfollow(int userId) { var follow = await Db.Follows.FirstOrDefaultAsync(x => x.FollowerUserId == Me && x.FollowedUserId == userId); if (follow is null) return NotFound(); Db.Follows.Remove(follow); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("followers")] public async Task<ActionResult> Followers() => await List(Db.Follows.Where(x => x.FollowedUserId == Me));
    [HttpGet("following")] public async Task<ActionResult> Following() => await List(Db.Follows.Where(x => x.FollowerUserId == Me));
}
