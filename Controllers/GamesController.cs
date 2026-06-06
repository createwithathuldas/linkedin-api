using linkedin_api.Data;
using linkedin_api.Enums;
using linkedin_api.Models;
using linkedin_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Controllers;

[Route("api/v1/games")]
public class GamesController(ApplicationDbContext db, IApiEntityService entities, ICurrentUser currentUser) : ApiControllerBase(db, entities, currentUser)
{
    [HttpGet] public ActionResult Games() => Ok(Enum.GetNames<GameType>());
    [HttpGet("{type}/daily")] public async Task<ActionResult> Daily(GameType type)
    {
        var date = DateTime.UtcNow.Date;
        var puzzle = await Db.DailyPuzzles.FirstOrDefaultAsync(x => x.Type == type && x.PuzzleDate == date);
        if (puzzle is null)
        {
            puzzle = new DailyPuzzle { Type = type, PuzzleDate = date, PuzzleJson = """{"prompt":"Daily professional puzzle","difficulty":"medium"}""" };
            Db.DailyPuzzles.Add(puzzle);
            await Db.SaveChangesAsync();
        }
        return Ok(puzzle);
    }
    [HttpPost("{type}/daily/attempt")] public async Task<ActionResult<PuzzleAttempt>> Attempt(GameType type, Dictionary<string, object?> body) { var puzzle = await Db.DailyPuzzles.FirstOrDefaultAsync(x => x.Type == type && x.PuzzleDate == DateTime.UtcNow.Date) ?? new DailyPuzzle { Type = type }; Db.DailyPuzzles.Update(puzzle); await Db.SaveChangesAsync(); return await Create<PuzzleAttempt>(body, x => { x.UserId = Me; x.Type = type; x.DailyPuzzleId = puzzle.Id; }); }
    [HttpGet("{type}/history")] public async Task<ActionResult> History(GameType type) => await List(Db.PuzzleAttempts.Where(x => x.UserId == Me && x.Type == type).OrderByDescending(x => x.CreatedAt));
    [HttpGet("{type}/streak")] public async Task<ActionResult> Streak(GameType type) => Ok(await Db.PuzzleStreaks.FirstOrDefaultAsync(x => x.UserId == Me && x.Type == type) ?? new PuzzleStreak { UserId = Me, Type = type });
    [HttpGet("leaderboard")] public async Task<ActionResult> Leaderboard() => Ok(await Db.PuzzleStreaks.GroupBy(x => x.UserId).Select(x => new { userId = x.Key, score = x.Sum(s => s.BestStreak) }).OrderByDescending(x => x.score).Take(50).ToListAsync());
    [HttpPost("{type}/daily/share")] public ActionResult Share(GameType type) => Ok(new { type, shareText = $"{type} solved on {DateTime.UtcNow:yyyy-MM-dd}" });
}
