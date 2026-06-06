using linkedin_api.Data;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/users")]
public class UsersController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser, IMediaStorageService media) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet("{userId:int}")]
    public async Task<ActionResult> GetProfile(int userId) => Ok(new
    {
        user = await Db.Users.FindAsync(userId),
        experience = await Db.Experiences.Where(x => x.UserId == userId).ToListAsync(),
        education = await Db.Educations.Where(x => x.UserId == userId).ToListAsync(),
        skills = await Db.Skills.Where(x => x.UserId == userId).ToListAsync(),
        certifications = await Db.Certifications.Where(x => x.UserId == userId).ToListAsync(),
        volunteer = await Db.VolunteerExperiences.Where(x => x.UserId == userId).ToListAsync(),
        honors = await Db.HonorAwards.Where(x => x.UserId == userId).ToListAsync(),
        publications = await Db.Publications.Where(x => x.UserId == userId).ToListAsync(),
        patents = await Db.Patents.Where(x => x.UserId == userId).ToListAsync(),
        courses = await Db.Courses.Where(x => x.UserId == userId).ToListAsync(),
        projects = await Db.Projects.Where(x => x.UserId == userId).ToListAsync(),
        languages = await Db.Languages.Where(x => x.UserId == userId).ToListAsync(),
        recommendations = await Db.Recommendations.Where(x => x.UserId == userId).ToListAsync(),
        featured = await Db.FeaturedItems.Where(x => x.UserId == userId).ToListAsync()
    });

    [HttpPut("{userId:int}")] public async Task<ActionResult> UpdateUser(int userId, Dictionary<string, object?> body) => await Update<User>(userId, body);
    [HttpPost("{userId:int}/avatar")] public async Task<ActionResult> Avatar(int userId, IFormFile file) => await UploadUserMedia(userId, file, "avatars", true);
    [HttpPost("{userId:int}/banner")] public async Task<ActionResult> Banner(int userId, IFormFile file) => await UploadUserMedia(userId, file, "banners", false);
    [HttpPut("{userId:int}/open-to-work")] public async Task<ActionResult> OpenToWork(int userId, Dictionary<string, object?> body) { var user = await Db.Users.FindAsync(userId); if (user is null) return NotFound(); user.OpenToWork = body.TryGetValue("openToWork", out var v) && bool.TryParse(v?.ToString(), out var b) ? b : !user.OpenToWork; user.OpenToWorkConfigJson = System.Text.Json.JsonSerializer.Serialize(body); await Db.SaveChangesAsync(); return Ok(user); }
    [HttpPut("{userId:int}/privacy")] public async Task<ActionResult> Privacy(int userId, Dictionary<string, object?> body) => await Update<User>(userId, body);
    [HttpGet("{userId:int}/profile-views")] public async Task<ActionResult> ProfileViews(int userId) => Ok(await Db.ProfileViews.Where(x => x.ViewedUserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync());

    [HttpPost("{userId:int}/experience")] public async Task<ActionResult<Experience>> AddExperience(int userId, Dictionary<string, object?> body) => await Create<Experience>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/experience/{id:int}")] public async Task<ActionResult> UpdateExperience(int id, Dictionary<string, object?> body) => await Update<Experience>(id, body);
    [HttpDelete("{userId:int}/experience/{id:int}")] public async Task<ActionResult> DeleteExperience(int id) => await Delete<Experience>(id);
    [HttpPost("{userId:int}/education")] public async Task<ActionResult<Education>> AddEducation(int userId, Dictionary<string, object?> body) => await Create<Education>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/education/{id:int}")] public async Task<ActionResult> UpdateEducation(int id, Dictionary<string, object?> body) => await Update<Education>(id, body);
    [HttpDelete("{userId:int}/education/{id:int}")] public async Task<ActionResult> DeleteEducation(int id) => await Delete<Education>(id);
    [HttpPost("{userId:int}/skills")] public async Task<ActionResult<Skill>> AddSkill(int userId, Dictionary<string, object?> body) => await Create<Skill>(body, x => x.UserId = userId);
    [HttpDelete("{userId:int}/skills/{id:int}")] public async Task<ActionResult> DeleteSkill(int id) => await Delete<Skill>(id);
    [HttpPost("{userId:int}/skills/{id:int}/endorse")] public async Task<ActionResult<Endorsement>> Endorse(int userId, int id) { var skill = await Db.Skills.FindAsync(id); if (skill is null) return NotFound(); skill.EndorsementCount++; return await Create<Endorsement>([], x => { x.SkillId = id; x.UserId = userId; x.EndorserUserId = Me; }); }
    [HttpDelete("{userId:int}/skills/{id:int}/endorse")] public async Task<ActionResult> RemoveEndorse(int id) { var e = await Db.Endorsements.FirstOrDefaultAsync(x => x.SkillId == id && x.EndorserUserId == Me); if (e is null) return NotFound(); Db.Endorsements.Remove(e); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpPost("{userId:int}/certifications")] public async Task<ActionResult<Certification>> AddCertification(int userId, Dictionary<string, object?> body) => await Create<Certification>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/certifications/{id:int}")] public async Task<ActionResult> UpdateCertification(int id, Dictionary<string, object?> body) => await Update<Certification>(id, body);
    [HttpDelete("{userId:int}/certifications/{id:int}")] public async Task<ActionResult> DeleteCertification(int id) => await Delete<Certification>(id);
    [HttpPost("{userId:int}/volunteer")] public async Task<ActionResult<VolunteerExperience>> AddVolunteer(int userId, Dictionary<string, object?> body) => await Create<VolunteerExperience>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/volunteer/{id:int}")] public async Task<ActionResult> UpdateVolunteer(int id, Dictionary<string, object?> body) => await Update<VolunteerExperience>(id, body);
    [HttpDelete("{userId:int}/volunteer/{id:int}")] public async Task<ActionResult> DeleteVolunteer(int id) => await Delete<VolunteerExperience>(id);
    [HttpPost("{userId:int}/honors")] public async Task<ActionResult<HonorAward>> AddHonor(int userId, Dictionary<string, object?> body) => await Create<HonorAward>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/honors/{id:int}")] public async Task<ActionResult> UpdateHonor(int id, Dictionary<string, object?> body) => await Update<HonorAward>(id, body);
    [HttpDelete("{userId:int}/honors/{id:int}")] public async Task<ActionResult> DeleteHonor(int id) => await Delete<HonorAward>(id);
    [HttpPost("{userId:int}/publications")] public async Task<ActionResult<Publication>> AddPublication(int userId, Dictionary<string, object?> body) => await Create<Publication>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/publications/{id:int}")] public async Task<ActionResult> UpdatePublication(int id, Dictionary<string, object?> body) => await Update<Publication>(id, body);
    [HttpDelete("{userId:int}/publications/{id:int}")] public async Task<ActionResult> DeletePublication(int id) => await Delete<Publication>(id);
    [HttpPost("{userId:int}/patents")] public async Task<ActionResult<Patent>> AddPatent(int userId, Dictionary<string, object?> body) => await Create<Patent>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/patents/{id:int}")] public async Task<ActionResult> UpdatePatent(int id, Dictionary<string, object?> body) => await Update<Patent>(id, body);
    [HttpDelete("{userId:int}/patents/{id:int}")] public async Task<ActionResult> DeletePatent(int id) => await Delete<Patent>(id);
    [HttpPost("{userId:int}/courses")] public async Task<ActionResult<Course>> AddCourse(int userId, Dictionary<string, object?> body) => await Create<Course>(body, x => x.UserId = userId);
    [HttpDelete("{userId:int}/courses/{id:int}")] public async Task<ActionResult> DeleteCourse(int id) => await Delete<Course>(id);
    [HttpPost("{userId:int}/projects")] public async Task<ActionResult<Project>> AddProject(int userId, Dictionary<string, object?> body) => await Create<Project>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/projects/{id:int}")] public async Task<ActionResult> UpdateProject(int id, Dictionary<string, object?> body) => await Update<Project>(id, body);
    [HttpDelete("{userId:int}/projects/{id:int}")] public async Task<ActionResult> DeleteProject(int id) => await Delete<Project>(id);
    [HttpPost("{userId:int}/languages")] public async Task<ActionResult<Language>> AddLanguage(int userId, Dictionary<string, object?> body) => await Create<Language>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/languages/{id:int}")] public async Task<ActionResult> UpdateLanguage(int id, Dictionary<string, object?> body) => await Update<Language>(id, body);
    [HttpDelete("{userId:int}/languages/{id:int}")] public async Task<ActionResult> DeleteLanguage(int id) => await Delete<Language>(id);
    [HttpPost("{userId:int}/recommendations")] public async Task<ActionResult<Recommendation>> AddRecommendation(int userId, Dictionary<string, object?> body) => await Create<Recommendation>(body, x => { x.UserId = userId; x.RecommenderUserId = Me; });
    [HttpPut("{userId:int}/recommendations/{id:int}")] public async Task<ActionResult> UpdateRecommendation(int id, Dictionary<string, object?> body) => await Update<Recommendation>(id, body);
    [HttpDelete("{userId:int}/recommendations/{id:int}")] public async Task<ActionResult> DeleteRecommendation(int id) => await Delete<Recommendation>(id);
    [HttpPost("{userId:int}/featured")] public async Task<ActionResult<FeaturedItem>> AddFeatured(int userId, Dictionary<string, object?> body) => await Create<FeaturedItem>(body, x => x.UserId = userId);
    [HttpPut("{userId:int}/featured/{id:int}")] public async Task<ActionResult> UpdateFeatured(int id, Dictionary<string, object?> body) => await Update<FeaturedItem>(id, body);
    [HttpDelete("{userId:int}/featured/{id:int}")] public async Task<ActionResult> DeleteFeatured(int id) => await Delete<FeaturedItem>(id);

    [HttpPost("{userId:int}/block")] public async Task<ActionResult<Block>> Block(int userId) => await Create<Block>([], x => { x.BlockerUserId = Me; x.BlockedUserId = userId; });
    [HttpDelete("{userId:int}/block")] public async Task<ActionResult> Unblock(int userId) { var block = await Db.Blocks.FirstOrDefaultAsync(x => x.BlockerUserId == Me && x.BlockedUserId == userId); if (block is null) return NotFound(); Db.Remove(block); await Db.SaveChangesAsync(); return NoContent(); }
    [HttpGet("{userId:int}/degree")] public async Task<ActionResult> Degree(int userId) { var direct = await Db.Connections.AnyAsync(x => x.UserId == Me && x.ConnectedUserId == userId); return Ok(new { userId, degree = direct ? 1 : 2 }); }

    private async Task<ActionResult> UploadUserMedia(int userId, IFormFile file, string folder, bool avatar)
    {
        var user = await Db.Users.FindAsync(userId);
        if (user is null) return NotFound();
        var saved = await media.SaveAsync(file, folder, HttpContext.RequestAborted);
        if (avatar) user.AvatarUrl = saved.Url; else user.BannerUrl = saved.Url;
        await Db.SaveChangesAsync();
        return Ok(saved);
    }
}
