using linkedin_api.Data;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/analytics")]
public class AnalyticsController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet("profile")] public async Task<ActionResult> Profile() => Ok(new { views = await Db.ProfileViews.CountAsync(x => x.ViewedUserId == Me), searchAppearances = await Db.SearchHistories.CountAsync(), impressions = await Db.Posts.CountAsync(x => x.AuthorUserId == Me) * 10 });
    [HttpGet("posts")] public async Task<ActionResult> Posts() => Ok(await Db.Posts.Where(x => x.AuthorUserId == Me).Select(x => new { postId = x.Id, reactions = Db.Reactions.Count(r => r.PostId == x.Id), comments = Db.Comments.Count(c => c.PostId == x.Id), impressions = 100 + Db.Reactions.Count(r => r.PostId == x.Id) * 5 }).ToListAsync());
    [HttpGet("followers")] public async Task<ActionResult> Followers() => Ok(new { followers = await Db.Follows.CountAsync(x => x.FollowedUserId == Me), companies = await Db.CompanyFollows.CountAsync(x => x.UserId == Me) });
    [HttpGet("company/{id:int}")] public async Task<ActionResult> Company(int id) => Ok(new { companyId = id, followers = await Db.CompanyFollows.CountAsync(x => x.CompanyId == id), jobs = await Db.Jobs.CountAsync(x => x.CompanyId == id), posts = await Db.Posts.CountAsync(x => x.CompanyId == id) });
}
