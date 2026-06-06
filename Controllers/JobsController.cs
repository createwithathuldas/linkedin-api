using linkedin_api.Data;
using linkedin_api.DTOs;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1")]
public class JobsController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet("jobs")] public async Task<ActionResult> Jobs([FromQuery] string? q) => await List(Db.Jobs.Where(x => q == null || x.Title.Contains(q) || (x.Description ?? "").Contains(q)));
    [HttpPost("jobs")] public async Task<ActionResult<Job>> CreateJob(Dictionary<string, object?> body) => await Create<Job>(body, x => x.PosterUserId = Me);
    [HttpGet("jobs/{id:int}")] public async Task<ActionResult> Job(int id) => Ok(await Db.Jobs.FindAsync(id));
    [HttpPut("jobs/{id:int}")] public async Task<ActionResult> UpdateJob(int id, Dictionary<string, object?> body) => await Update<Job>(id, body);
    [HttpDelete("jobs/{id:int}")] public async Task<ActionResult> CloseJob(int id) { var job = await Db.Jobs.FindAsync(id); if (job is null) return NotFound(); job.IsOpen = false; await Db.SaveChangesAsync(); return NoContent(); }
    [HttpPost("jobs/{id:int}/apply")] public async Task<ActionResult<JobApplication>> Apply(int id, Dictionary<string, object?> body) => await Create<JobApplication>(body, x => { x.JobId = id; x.ApplicantUserId = Me; });
    [HttpGet("jobs/{id:int}/applicants")] public async Task<ActionResult> Applicants(int id) => await List(Db.JobApplications.Where(x => x.JobId == id));
    [HttpPut("jobs/{id:int}/applicants/{appId:int}/stage")] public async Task<ActionResult> Stage(int appId, StageRequest request) => await Update<JobApplication>(appId, new() { ["Stage"] = request.Stage });
    [HttpPost("jobs/{id:int}/save")] public async Task<ActionResult<SavedItem>> SaveJob(int id) => await Create<SavedItem>([], x => { x.UserId = Me; x.ItemId = id; x.Type = SavedItemType.Job; });
    [HttpDelete("jobs/{id:int}/save")] public async Task<ActionResult> UnsaveJob(int id) { var item = await Db.SavedItems.FirstOrDefaultAsync(x => x.UserId == Me && x.ItemId == id && x.Type == SavedItemType.Job); if (item is null) return NotFound(); Db.Remove(item); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("jobs/saved")] public async Task<ActionResult> SavedJobs() => await List(Db.SavedItems.Where(x => x.UserId == Me && x.Type == SavedItemType.Job));
    [HttpGet("jobs/recommended")] public async Task<ActionResult> Recommended() => await List(Db.JobRecommendations.Where(x => x.UserId == Me).OrderByDescending(x => x.Score));
    [HttpGet("jobs/recommended/categories")] public ActionResult RecommendedCategories() => Ok(new { jobTypes = Enum.GetNames<JobType>(), workplaces = Enum.GetNames<WorkplaceType>(), levels = Enum.GetNames<ExperienceLevel>() });
    [HttpGet("jobs/applied")] public async Task<ActionResult> Applied() => await List(Db.JobApplications.Where(x => x.ApplicantUserId == Me));
    [HttpPost("job-alerts")] public async Task<ActionResult<JobAlert>> CreateAlert(Dictionary<string, object?> body) => await Create<JobAlert>(body, x => x.UserId = Me);
    [HttpGet("job-alerts")] public async Task<ActionResult> Alerts() => await List(Db.JobAlerts.Where(x => x.UserId == Me));
    [HttpPut("job-alerts/{id:int}")] public async Task<ActionResult> UpdateAlert(int id, Dictionary<string, object?> body) => await Update<JobAlert>(id, body);
    [HttpDelete("job-alerts/{id:int}")] public async Task<ActionResult> DeleteAlert(int id) => await Delete<JobAlert>(id);
    [HttpGet("jobs/categories")] public ActionResult Categories() => Ok(new { jobTypes = Enum.GetNames<JobType>(), workplaces = Enum.GetNames<WorkplaceType>(), levels = Enum.GetNames<ExperienceLevel>() });
}
