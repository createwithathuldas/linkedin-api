using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1")]
public class PostsController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser, IMediaStorageService media) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet("feed")]
    public async Task<ActionResult> Feed()
    {
        var posts = await Db.Posts.Include(x => x.Author).Where(x => !x.IsDraft).OrderByDescending(x => x.CreatedAt).Take(20).AsNoTracking().ToListAsync();
        foreach (var p in posts)
        {
            p.ReactionCount = await Db.Reactions.CountAsync(r => r.PostId == p.Id);
            p.CommentCount = await Db.Comments.CountAsync(c => c.PostId == p.Id);
        }
        return Ok(posts);
    }
    [HttpPost("posts")] public async Task<ActionResult<Post>> CreatePost(Dictionary<string, object?> body) => await Create<Post>(body, x => x.AuthorUserId = Me);
    [HttpPost("posts/media")] public async Task<ActionResult> UploadMedia(IFormFile file) { var saved = await media.SaveAsync(file, "posts", HttpContext.RequestAborted); return Ok(saved); }
    [HttpGet("posts/{id:int}")]
    public async Task<ActionResult> GetPost(int id)
    {
        var post = await Db.Posts.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
        if (post is null) return NotFound();
        post.ReactionCount = await Db.Reactions.CountAsync(r => r.PostId == id);
        post.CommentCount = await Db.Comments.CountAsync(c => c.PostId == id);
        return Ok(post);
    }
    [HttpPut("posts/{id:int}")] public async Task<ActionResult> UpdatePost(int id, Dictionary<string, object?> body) => await Update<Post>(id, body);
    [HttpDelete("posts/{id:int}")] public async Task<ActionResult> DeletePost(int id) => await Delete<Post>(id);
    [HttpPost("posts/{id:int}/react")]
    public async Task<ActionResult> ReactPost(int id, ReactionRequest request)
    {
        var existing = await Db.Reactions.FirstOrDefaultAsync(x => x.PostId == id && x.UserId == Me);
        if (existing is not null)
        {
            if (existing.Type == request.Type)
            {
                Db.Reactions.Remove(existing);
                await Db.SaveChangesAsync();
                return NoContent();
            }
            existing.Type = request.Type;
            existing.UpdatedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return Ok(existing);
        }
        var r = new Reaction { PostId = id, UserId = Me, Type = request.Type, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        Db.Reactions.Add(r);
        await Db.SaveChangesAsync();
        return Ok(r);
    }
    [HttpDelete("posts/{id:int}/react")] public async Task<ActionResult> RemovePostReaction(int id) { var r = await Db.Reactions.FirstOrDefaultAsync(x => x.PostId == id && x.UserId == Me); if (r is null) return NotFound(); Db.Remove(r); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("posts/{id:int}/reactions")] public async Task<ActionResult> Reactions(int id) => await List(Db.Reactions.Where(x => x.PostId == id));
    [HttpPost("posts/{id:int}/comments")] public async Task<ActionResult<Comment>> AddComment(int id, TextRequest request) => await Create<Comment>([], x => { x.PostId = id; x.UserId = Me; x.Text = request.Text ?? ""; });
    [HttpGet("posts/{id:int}/comments")] public async Task<ActionResult> Comments(int id) => await List(Db.Comments.Include(x => x.User).Where(x => x.PostId == id && x.ParentCommentId == null));
    [HttpPut("comments/{id:int}")] public async Task<ActionResult> UpdateComment(int id, TextRequest request) => await Update<Comment>(id, new() { ["Text"] = request.Text });
    [HttpDelete("comments/{id:int}")] public async Task<ActionResult> DeleteComment(int id) => await Delete<Comment>(id);
    [HttpPost("comments/{id:int}/react")]
    public async Task<ActionResult> ReactComment(int id, ReactionRequest request)
    {
        var existing = await Db.Reactions.FirstOrDefaultAsync(x => x.CommentId == id && x.UserId == Me);
        if (existing is not null)
        {
            if (existing.Type == request.Type)
            {
                Db.Reactions.Remove(existing);
                await Db.SaveChangesAsync();
                return NoContent();
            }
            existing.Type = request.Type;
            existing.UpdatedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return Ok(existing);
        }
        var r = new Reaction { CommentId = id, UserId = Me, Type = request.Type, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        Db.Reactions.Add(r);
        await Db.SaveChangesAsync();
        return Ok(r);
    }
    [HttpPost("comments/{id:int}/replies")] public async Task<ActionResult<Comment>> Reply(int id, TextRequest request) { var parent = await Db.Comments.FindAsync(id); return await Create<Comment>([], x => { x.PostId = parent?.PostId ?? 0; x.ParentCommentId = id; x.UserId = Me; x.Text = request.Text ?? ""; }); }
    [HttpPost("posts/{id:int}/repost")] public async Task<ActionResult<Post>> Repost(int id, Dictionary<string, object?> body) => await Create<Post>(body, x => { x.AuthorUserId = Me; x.RepostOfPostId = id; });
    [HttpPost("posts/{id:int}/share")] public ActionResult Share(int id, ShareRequest request) => Ok(new { postId = id, request });
    [HttpPost("posts/{id:int}/send")] public async Task<ActionResult<Message>> SendPost(int id, ShareRequest request) => await Create<Message>([], x => { x.SenderUserId = Me; x.Text = request.Message ?? $"Shared post {id}"; });
    [HttpPost("posts/{id:int}/pin")] public async Task<ActionResult> Pin(int id) { var post = await Db.Posts.FindAsync(id); if (post is null) return NotFound(); post.IsPinned = true; await Db.SaveChangesAsync(); return Ok(post); }
    [HttpPost("posts/{id:int}/save")] public async Task<ActionResult<SavedItem>> SavePost(int id) => await Create<SavedItem>([], x => { x.UserId = Me; x.ItemId = id; x.Type = Enums.SavedItemType.Post; });
    [HttpDelete("posts/{id:int}/save")] public async Task<ActionResult> UnsavePost(int id) { var item = await Db.SavedItems.FirstOrDefaultAsync(x => x.UserId == Me && x.ItemId == id && x.Type == Enums.SavedItemType.Post); if (item is null) return NotFound(); Db.Remove(item); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("posts/saved")] public async Task<ActionResult> SavedPosts() => await List(Db.SavedItems.Where(x => x.UserId == Me && x.Type == Enums.SavedItemType.Post));
    [HttpGet("posts/saved/collections")] public async Task<ActionResult> Collections() => Ok(await Db.SavedItems.Where(x => x.UserId == Me).Select(x => x.CollectionName).Distinct().ToListAsync());
    [HttpPost("posts/{id:int}/report")] public async Task<ActionResult<PostReport>> Report(int id, Dictionary<string, object?> body) => await Create<PostReport>(body, x => { x.PostId = id; x.ReporterUserId = Me; });
    [HttpPost("articles")] public async Task<ActionResult<Post>> CreateArticle(Dictionary<string, object?> body) => await Create<Post>(body, x => { x.AuthorUserId = Me; x.Type = Enums.PostType.Article; });
    [HttpPut("articles/{id:int}")] public async Task<ActionResult> UpdateArticle(int id, Dictionary<string, object?> body) => await Update<Post>(id, body);
    [HttpGet("articles/{id:int}")] public async Task<ActionResult> Article(int id) => Ok(await Db.Posts.FindAsync(id));
    [HttpPost("posts/draft")] public async Task<ActionResult<Post>> Draft(Dictionary<string, object?> body) => await Create<Post>(body, x => { x.AuthorUserId = Me; x.IsDraft = true; });
    [HttpGet("posts/drafts")] public async Task<ActionResult> Drafts() => await List(Db.Posts.Where(x => x.AuthorUserId == Me && x.IsDraft));
    [HttpGet("hashtags/{tag}/posts")] public async Task<ActionResult> HashtagPosts(string tag) => await List(Db.Hashtags.Where(x => x.Tag == tag && x.PostId.HasValue));
    [HttpPost("hashtags/{tag}/follow")] public async Task<ActionResult<Hashtag>> FollowHashtag(string tag) => await Create<Hashtag>([], x => { x.Tag = tag; x.FollowerUserId = Me; });
    [HttpGet("posts/templates")] public ActionResult Templates() => Ok(Enum.GetNames<Enums.TemplateType>());
}
