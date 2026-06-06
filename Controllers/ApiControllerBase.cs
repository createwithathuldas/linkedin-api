using linkedin_api.Data;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[ApiController]
public abstract class ApiControllerBase(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ControllerBase
{
    protected ApplicationDbContext Db { get; } = db;
    protected IApiEntityService Entities { get; } = entities;
    protected ICurrentUser CurrentUser { get; } = currentUser;

    protected int Me => CurrentUser.UserId;

    protected async Task<ActionResult<T>> Create<T>(Dictionary<string, object?> body, Action<T>? defaults = null) where T : EntityBase, new()
    {
        var entity = Entities.Create<T>(body);
        defaults?.Invoke(entity);
        Db.Set<T>().Add(entity);
        await Db.SaveChangesAsync();
        return Ok(entity);
    }

    protected async Task<ActionResult> Update<T>(int id, Dictionary<string, object?> body) where T : EntityBase
    {
        var entity = await Db.Set<T>().FindAsync(id);
        if (entity is null) return NotFound();
        Entities.Apply(entity, body);
        await Db.SaveChangesAsync();
        return Ok(entity);
    }

    protected async Task<ActionResult> Delete<T>(int id) where T : EntityBase
    {
        var entity = await Db.Set<T>().FindAsync(id);
        if (entity is null) return NotFound();
        Db.Remove(entity);
        await Db.SaveChangesAsync();
        return NoContent();
    }

    protected async Task<ActionResult> List<T>(IQueryable<T> query, int page = 1, int pageSize = 20) where T : class
        => Ok(await query.AsNoTracking().Page(page, pageSize).ToListAsync());
}
