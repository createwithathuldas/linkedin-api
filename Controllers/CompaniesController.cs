using linkedin_api.Data;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/companies")]
public class CompaniesController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpPost] public async Task<ActionResult<Company>> CreateCompany(Dictionary<string, object?> body) => await Create<Company>(body);
    [HttpGet("{id:int}")] public async Task<ActionResult> Company(int id) => Ok(await Db.Companies.FindAsync(id));
    [HttpPut("{id:int}")] public async Task<ActionResult> UpdateCompany(int id, Dictionary<string, object?> body) => await Update<Company>(id, body);
    [HttpPost("{id:int}/follow")] public async Task<ActionResult<CompanyFollow>> Follow(int id) => await Create<CompanyFollow>([], x => { x.CompanyId = id; x.UserId = Me; });
    [HttpDelete("{id:int}/follow")] public async Task<ActionResult> Unfollow(int id) { var f = await Db.CompanyFollows.FirstOrDefaultAsync(x => x.CompanyId == id && x.UserId == Me); if (f is null) return NotFound(); Db.Remove(f); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("{id:int}/followers")] public async Task<ActionResult> Followers(int id) => await List(Db.CompanyFollows.Where(x => x.CompanyId == id));
    [HttpGet("{id:int}/employees")] public async Task<ActionResult> Employees(int id) => await List(Db.Users.Where(x => Db.Experiences.Any(e => e.UserId == x.Id && e.Company == Db.Companies.Where(c => c.Id == id).Select(c => c.Name).FirstOrDefault())));
    [HttpPost("{id:int}/admins")] public async Task<ActionResult<CompanyAdmin>> AddAdmin(int id, Dictionary<string, object?> body) => await Create<CompanyAdmin>(body, x => x.CompanyId = id);
    [HttpDelete("{id:int}/admins/{userId:int}")] public async Task<ActionResult> RemoveAdmin(int id, int userId) { var admin = await Db.CompanyAdmins.FirstOrDefaultAsync(x => x.CompanyId == id && x.UserId == userId); if (admin is null) return NotFound(); Db.Remove(admin); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpPost("{id:int}/posts")] public async Task<ActionResult<Post>> AddPost(int id, Dictionary<string, object?> body) => await Create<Post>(body, x => { x.CompanyId = id; x.AuthorUserId = Me; });
    [HttpGet("{id:int}/posts")] public async Task<ActionResult> Posts(int id) => await List(Db.Posts.Where(x => x.CompanyId == id));
    [HttpGet("{id:int}/analytics")] public async Task<ActionResult> Analytics(int id) => Ok(new { companyId = id, followers = await Db.CompanyFollows.CountAsync(x => x.CompanyId == id), posts = await Db.Posts.CountAsync(x => x.CompanyId == id) });
    [HttpPost("{id:int}/products")] public async Task<ActionResult<CompanyProduct>> AddProduct(int id, Dictionary<string, object?> body) => await Create<CompanyProduct>(body, x => x.CompanyId = id);
    [HttpPut("{id:int}/products/{prodId:int}")] public async Task<ActionResult> UpdateProduct(int prodId, Dictionary<string, object?> body) => await Update<CompanyProduct>(prodId, body);
    [HttpDelete("{id:int}/products/{prodId:int}")] public async Task<ActionResult> DeleteProduct(int prodId) => await Delete<CompanyProduct>(prodId);
    [HttpGet("{id:int}/jobs")] public async Task<ActionResult> Jobs(int id) => await List(Db.Jobs.Where(x => x.CompanyId == id));
}
