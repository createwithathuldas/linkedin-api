using linkedin_api.Data;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using linkedin_api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/search")]
public class SearchController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet]
    public async Task<ActionResult> Unified([FromQuery] string q = "", [FromQuery] SearchType type = SearchType.All)
    {
        await Record(q, type);
        var people = await PeopleQuery(q).Take(10).Select(x => new SearchResultDto(x.Id, "Person", x.FirstName + " " + x.LastName, x.Headline, x.AvatarUrl)).ToListAsync();
        var jobs = await JobQuery(q).Take(10).Select(x => new SearchResultDto(x.Id, "Job", x.Title, x.Location, null)).ToListAsync();
        var companies = await CompanyQuery(q).Take(10).Select(x => new SearchResultDto(x.Id, "Company", x.Name, x.Industry, x.LogoUrl)).ToListAsync();
        var posts = await PostQuery(q).Take(10).Select(x => new SearchResultDto(x.Id, "Post", x.Content, "Post", null)).ToListAsync();
        var groups = await GroupQuery(q).Take(10).Select(x => new SearchResultDto(x.Id, "Group", x.Name, x.Description, null)).ToListAsync();

        var combined = new List<SearchResultDto>();
        combined.AddRange(people);
        combined.AddRange(jobs);
        combined.AddRange(companies);
        combined.AddRange(posts);
        combined.AddRange(groups);

        return Ok(combined);
    }
    [HttpGet("people")] public async Task<ActionResult> People([FromQuery] string q = "") { await Record(q, SearchType.People); return Ok(await PeopleQuery(q).Select(x => new SearchResultDto(x.Id, "Person", x.FirstName + " " + x.LastName, x.Headline, x.AvatarUrl)).ToListAsync()); }
    [HttpGet("jobs")] public async Task<ActionResult> Jobs([FromQuery] string q = "") { await Record(q, SearchType.Jobs); return Ok(await JobQuery(q).Select(x => new SearchResultDto(x.Id, "Job", x.Title, x.Location, null)).ToListAsync()); }
    [HttpGet("companies")] public async Task<ActionResult> Companies([FromQuery] string q = "") { await Record(q, SearchType.Companies); return Ok(await CompanyQuery(q).Select(x => new SearchResultDto(x.Id, "Company", x.Name, x.Industry, x.LogoUrl)).ToListAsync()); }
    [HttpGet("posts")] public async Task<ActionResult> Posts([FromQuery] string q = "") { await Record(q, SearchType.Posts); return Ok(await PostQuery(q).Select(x => new SearchResultDto(x.Id, "Post", x.Content, "Post", null)).ToListAsync()); }
    [HttpGet("groups")] public async Task<ActionResult> Groups([FromQuery] string q = "") { await Record(q, SearchType.Groups); return Ok(await GroupQuery(q).Select(x => new SearchResultDto(x.Id, "Group", x.Name, x.Description, null)).ToListAsync()); }
    [HttpGet("typeahead")] public async Task<ActionResult> Typeahead([FromQuery] string q = "") => Ok(await Db.SearchHistories.Where(x => x.UserId == Me && x.Query.StartsWith(q)).Select(x => x.Query).Distinct().Take(10).ToListAsync());
    [HttpGet("recent")] public async Task<ActionResult> Recent() => await List(Db.SearchHistories.Where(x => x.UserId == Me).OrderByDescending(x => x.CreatedAt));
    [HttpDelete("recent")] public async Task<ActionResult> ClearRecent() { await Db.SearchHistories.Where(x => x.UserId == Me).ExecuteDeleteAsync(); return NoContent(); }
    [HttpPost("saved")] public async Task<ActionResult<SavedSearch>> Save(Dictionary<string, object?> body) => await Create<SavedSearch>(body, x => x.UserId = Me);
    [HttpGet("saved")] public async Task<ActionResult> Saved() => await List(Db.SavedSearches.Where(x => x.UserId == Me));
    [HttpDelete("saved/{id:int}")] public async Task<ActionResult> DeleteSaved(int id) => await Delete<SavedSearch>(id);

    private async Task Record(string q, SearchType type) { if (string.IsNullOrWhiteSpace(q)) return; Db.SearchHistories.Add(new SearchHistory { UserId = Me, Query = q, Type = type }); await Db.SaveChangesAsync(); }
    private IQueryable<User> PeopleQuery(string q) => Db.Users.Where(x => q == "" || x.FirstName.Contains(q) || x.LastName.Contains(q) || x.Headline.Contains(q));
    private IQueryable<Job> JobQuery(string q) => Db.Jobs.Where(x => q == "" || x.Title.Contains(q) || (x.Description ?? "").Contains(q));
    private IQueryable<Company> CompanyQuery(string q) => Db.Companies.Where(x => q == "" || x.Name.Contains(q) || (x.Industry ?? "").Contains(q));
    private IQueryable<Post> PostQuery(string q) => Db.Posts.Where(x => q == "" || (x.Content ?? "").Contains(q));
    private IQueryable<Group> GroupQuery(string q) => Db.Groups.Where(x => q == "" || x.Name.Contains(q) || (x.Description ?? "").Contains(q));
}
