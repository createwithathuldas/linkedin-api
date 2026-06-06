using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/groups")]
public class GroupsController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpPost] public async Task<ActionResult<Group>> CreateGroup(Dictionary<string, object?> body) => await Create<Group>(body, x => x.OwnerUserId = Me);
    [HttpGet("{id:int}")] public async Task<ActionResult> Group(int id) => Ok(await Db.Groups.FindAsync(id));
    [HttpPut("{id:int}")] public async Task<ActionResult> UpdateGroup(int id, Dictionary<string, object?> body) => await Update<Group>(id, body);
    [HttpDelete("{id:int}")] public async Task<ActionResult> DeleteGroup(int id) => await Delete<Group>(id);
    [HttpPost("{id:int}/join")] public async Task<ActionResult<GroupMember>> Join(int id) => await Create<GroupMember>([], x => { x.GroupId = id; x.UserId = Me; });
    [HttpDelete("{id:int}/leave")] public async Task<ActionResult> Leave(int id) { var m = await Db.GroupMembers.FirstOrDefaultAsync(x => x.GroupId == id && x.UserId == Me); if (m is null) return NotFound(); Db.Remove(m); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("{id:int}/members")] public async Task<ActionResult> Members(int id) => await List(Db.GroupMembers.Where(x => x.GroupId == id));
    [HttpPut("{id:int}/members/{userId:int}/role")] public async Task<ActionResult> Role(int id, int userId, Dictionary<string, object?> body) { var m = await Db.GroupMembers.FirstOrDefaultAsync(x => x.GroupId == id && x.UserId == userId); if (m is null) return NotFound(); if (body.TryGetValue("role", out var role)) m.Role = Enum.Parse<GroupRole>(role?.ToString() ?? "Member", true); await Db.SaveChangesAsync(); return Ok(m); }
    [HttpDelete("{id:int}/members/{userId:int}")] public async Task<ActionResult> RemoveMember(int id, int userId) { var m = await Db.GroupMembers.FirstOrDefaultAsync(x => x.GroupId == id && x.UserId == userId); if (m is null) return NotFound(); Db.Remove(m); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("{id:int}/posts")] public async Task<ActionResult> Posts(int id) => await List(Db.Posts.Where(x => x.GroupId == id));
    [HttpPost("{id:int}/posts")] public async Task<ActionResult<Post>> AddPost(int id, Dictionary<string, object?> body) => await Create<Post>(body, x => { x.GroupId = id; x.AuthorUserId = Me; });
    [HttpGet("my")] public async Task<ActionResult> MyGroups() => await List(Db.GroupMembers.Where(x => x.UserId == Me).Join(Db.Groups, m => m.GroupId, g => g.Id, (m, g) => g));
    [HttpGet("suggested")] public async Task<ActionResult> Suggested() => await List(Db.Groups.OrderByDescending(x => x.CreatedAt));
    [HttpPost("{id:int}/invite")] public ActionResult Invite(int id, InviteRequest request) => Ok(new { groupId = id, request.UserId, request.Message });
}
