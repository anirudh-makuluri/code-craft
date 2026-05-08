using api.Contracts.Crafts;
using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api.Controller;

[ApiController]
[Route("api/crafts")]
public class CodeCraftController(CodeCraftDbContext db) : ControllerBase
{
    [HttpGet("public")]
    public async Task<ActionResult<IEnumerable<CraftResponse>>> PublicCrafts()
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var crafts = await db.Crafts.Where(c => c.IsPublic).OrderByDescending(c => c.UpdatedAtUtc).ToListAsync();
        var response = new List<CraftResponse>();
        foreach (var craft in crafts) response.Add(await ToResponseAsync(craft, username));
        return Ok(response);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<CraftResponse>>> MyCrafts()
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var crafts = await db.Crafts.Where(c => c.CreatedByUsername == username).OrderByDescending(c => c.UpdatedAtUtc).ToListAsync();
        var response = new List<CraftResponse>();
        foreach (var craft in crafts) response.Add(await ToResponseAsync(craft, username));
        return Ok(response);
    }

    [HttpGet("{craftId}")]
    public async Task<ActionResult<CraftResponse>> GetByCraftId(string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();
        if (!craft.IsPublic && craft.CreatedByUsername != username) return Forbid();

        db.CraftViews.Add(new CraftView { CraftId = craft.Id, Username = username });
        await db.SaveChangesAsync();
        return Ok(await ToResponseAsync(craft, username));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CraftResponse>> Create([FromBody] CreateCraftRequest request)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (await db.Crafts.AnyAsync(c => c.CraftId == request.CraftId)) return Conflict(new { error = "craftId already exists" });

        var craft = new Craft
        {
            CraftId = request.CraftId,
            Name = request.Name,
            Js = request.Js,
            Css = request.Css,
            Html = request.Html,
            IsPublic = request.IsPublic,
            IsFork = request.IsFork,
            ParentCraftId = request.ParentCraftId,
            CreatedByUsername = username,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        db.Crafts.Add(craft);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByCraftId), new { craftId = craft.CraftId }, await ToResponseAsync(craft, username));
    }

    [Authorize]
    [HttpPut("{craftId}")]
    public async Task<ActionResult<CraftResponse>> Update(string craftId, [FromBody] SaveCraftRequest request)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();
        if (craft.CreatedByUsername != username) return Forbid();

        craft.Name = request.Name;
        craft.Js = request.Js;
        craft.Css = request.Css;
        craft.Html = request.Html;
        craft.IsPublic = request.IsPublic;
        craft.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(await ToResponseAsync(craft, username));
    }

    [Authorize]
    [HttpDelete("{craftId}")]
    public async Task<IActionResult> Delete(string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();
        if (craft.CreatedByUsername != username) return Forbid();

        db.Crafts.Remove(craft);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpPost("{craftId}/like")]
    public async Task<ActionResult<CraftResponse>> Like(string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();

        if (!await db.CraftLikes.AnyAsync(l => l.CraftId == craft.Id && l.Username == username))
        {
            db.CraftLikes.Add(new CraftLike { CraftId = craft.Id, Username = username });
            await db.SaveChangesAsync();
        }

        return Ok(await ToResponseAsync(craft, username));
    }

    [Authorize]
    [HttpDelete("{craftId}/like")]
    public async Task<ActionResult<CraftResponse>> Unlike(string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();

        var like = await db.CraftLikes.FirstOrDefaultAsync(l => l.CraftId == craft.Id && l.Username == username);
        if (like is not null)
        {
            db.CraftLikes.Remove(like);
            await db.SaveChangesAsync();
        }

        return Ok(await ToResponseAsync(craft, username));
    }

    [Authorize]
    [HttpPost("{craftId}/fork")]
    public async Task<ActionResult<CraftResponse>> Fork(string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var source = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (source is null) return NotFound();
        if (!source.IsPublic && source.CreatedByUsername != username) return Forbid();

        var fork = new Craft
        {
            CraftId = $"{craftId}-fork-{Guid.NewGuid().ToString("N")[..8]}",
            Name = $"{source.Name} (fork)",
            Js = source.Js,
            Css = source.Css,
            Html = source.Html,
            IsPublic = false,
            IsFork = true,
            ParentCraftId = source.CraftId,
            CreatedByUsername = username,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        db.Crafts.Add(fork);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByCraftId), new { craftId = fork.CraftId }, await ToResponseAsync(fork, username));
    }

    private async Task<CraftResponse> ToResponseAsync(Craft c, string? username)
    {
        var likes = await db.CraftLikes.CountAsync(l => l.CraftId == c.Id);
        var views = await db.CraftViews.CountAsync(v => v.CraftId == c.Id);
        var liked = username is not null && await db.CraftLikes.AnyAsync(l => l.CraftId == c.Id && l.Username == username);
        return new CraftResponse(c.CraftId, c.Name, c.CreatedByUsername, c.Js, c.Css, c.Html, c.IsPublic, c.IsFork, c.ParentCraftId, likes, views, liked, c.CreatedAtUtc, c.UpdatedAtUtc);
    }
}
