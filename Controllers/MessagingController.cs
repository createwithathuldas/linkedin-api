using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1")]
public class MessagingController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet("conversations")] public async Task<ActionResult> Conversations() => await List(Db.ConversationMembers.Where(x => x.UserId == Me).Join(Db.Conversations, m => m.ConversationId, c => c.Id, (m, c) => c));
    [HttpPost("conversations")] public async Task<ActionResult<Conversation>> Start(Dictionary<string, object?> body) => await CreateConversation(body, ConversationType.Direct);
    [HttpGet("conversations/{id:int}/messages")] public async Task<ActionResult> Messages(int id) => await List(Db.Messages.Where(x => x.ConversationId == id && !x.Deleted).OrderBy(x => x.CreatedAt));
    [HttpPost("conversations/{id:int}/messages")] public async Task<ActionResult<Message>> Send(int id, Dictionary<string, object?> body) => await Create<Message>(body, x => { x.ConversationId = id; x.SenderUserId = Me; });
    [HttpPut("messages/{id:int}")] public async Task<ActionResult> UpdateMessage(int id, Dictionary<string, object?> body) => await Update<Message>(id, body);
    [HttpDelete("messages/{id:int}")] public async Task<ActionResult> DeleteMessage(int id) { var msg = await Db.Messages.FindAsync(id); if (msg is null) return NotFound(); msg.Deleted = true; await Db.SaveChangesAsync(); return NoContent(); }
    [HttpPost("messages/{id:int}/react")] public async Task<ActionResult<MessageReaction>> React(int id, ReactionRequest request) => await Create<MessageReaction>([], x => { x.MessageId = id; x.UserId = Me; x.Type = request.Type; });
    [HttpPut("conversations/{id:int}/read")] public async Task<ActionResult> Read(int id) { var member = await Db.ConversationMembers.FirstOrDefaultAsync(x => x.ConversationId == id && x.UserId == Me); if (member is null) return NotFound(); member.LastReadAt = DateTime.UtcNow; await Db.SaveChangesAsync(); return Ok(member); }
    [HttpPut("conversations/{id:int}/mute")] public async Task<ActionResult> Mute(int id) => await ToggleConversation(id, c => c.IsMuted = !c.IsMuted);
    [HttpPut("conversations/{id:int}/archive")] public async Task<ActionResult> Archive(int id) => await ToggleConversation(id, c => c.IsArchived = !c.IsArchived);
    [HttpDelete("conversations/{id:int}")] public async Task<ActionResult> DeleteConversation(int id) { var member = await Db.ConversationMembers.FirstOrDefaultAsync(x => x.ConversationId == id && x.UserId == Me); if (member is null) return NotFound(); member.Deleted = true; await Db.SaveChangesAsync(); return NoContent(); }
    [HttpPost("conversations/group")] public async Task<ActionResult<Conversation>> Group(Dictionary<string, object?> body) => await CreateConversation(body, ConversationType.Group);
    [HttpPost("conversations/{id:int}/members")] public async Task<ActionResult<ConversationMember>> AddMember(int id, InviteRequest request) => await Create<ConversationMember>([], x => { x.ConversationId = id; x.UserId = request.UserId; });
    [HttpDelete("conversations/{id:int}/members/{userId:int}")] public async Task<ActionResult> RemoveMember(int id, int userId) { var member = await Db.ConversationMembers.FirstOrDefaultAsync(x => x.ConversationId == id && x.UserId == userId); if (member is null) return NotFound(); Db.Remove(member); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpPost("inmail")] public async Task<ActionResult<Message>> InMail(Dictionary<string, object?> body) => await Create<Message>(body, x => { x.SenderUserId = Me; });
    [HttpGet("inmail/credits")] public async Task<ActionResult> InMailCredits() { var usage = await Db.InMailUsages.Where(x => x.UserId == Me).OrderByDescending(x => x.Id).FirstOrDefaultAsync(); return Ok(new { remaining = Math.Max(0, (usage?.CreditsGranted ?? 0) - (usage?.CreditsUsed ?? 0)) }); }

    private async Task<ActionResult<Conversation>> CreateConversation(Dictionary<string, object?> body, ConversationType type)
    {
        var conversation = Entities.Create<Conversation>(body);
        conversation.Type = type;
        Db.Conversations.Add(conversation);
        await Db.SaveChangesAsync();
        Db.ConversationMembers.Add(new ConversationMember { ConversationId = conversation.Id, UserId = Me });
        await Db.SaveChangesAsync();
        return Ok(conversation);
    }
    private async Task<ActionResult> ToggleConversation(int id, Action<Conversation> toggle) { var c = await Db.Conversations.FindAsync(id); if (c is null) return NotFound(); toggle(c); await Db.SaveChangesAsync(); return Ok(c); }
}
