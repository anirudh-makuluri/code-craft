using api.Contracts.Crafts;
using api.Data;
using api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api.Controller;

[ApiController]
[Route("api")]
public class LegacyApiController(CodeCraftDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();
        if (!craft.IsPublic && craft.CreatedByUsername != username) return Forbid();

        var likes = await db.CraftLikes.Where(l => l.CraftId == craft.Id).Select(l => l.Username).ToListAsync();
        return Ok(new
        {
            craft.CraftId,
            craft.Name,
            CreatedBy = craft.CreatedByUsername,
            craft.Js,
            craft.Css,
            craft.Html,
            craft.IsPublic,
            craft.IsFork,
            LikesCount = likes.Count,
            ViewsCount = await db.CraftViews.CountAsync(v => v.CraftId == craft.Id),
            LikedBy = string.Join(",", likes)
        });
    }

    [HttpGet("user")]
    public async Task<IActionResult> UserCrafts([FromQuery] string username)
    {
        var current = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var crafts = await db.Crafts.Where(c => c.CreatedByUsername == username && (c.IsPublic || c.CreatedByUsername == current)).ToListAsync();
        var result = new List<object>();

        foreach (var craft in crafts)
        {
            var likes = await db.CraftLikes.Where(l => l.CraftId == craft.Id).Select(l => l.Username).ToListAsync();
            result.Add(new
            {
                craft.CraftId,
                craft.Name,
                CreatedBy = craft.CreatedByUsername,
                craft.Js,
                craft.Css,
                craft.Html,
                craft.IsPublic,
                craft.IsFork,
                LikesCount = likes.Count,
                ViewsCount = await db.CraftViews.CountAsync(v => v.CraftId == craft.Id),
                LikedBy = string.Join(",", likes)
            });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] CreateCraftRequest request)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existing = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == request.CraftId);

        if (existing is null)
        {
            db.Crafts.Add(new Craft
            {
                CraftId = request.CraftId,
                Name = request.Name,
                Js = request.Js,
                Css = request.Css,
                Html = request.Html,
                IsPublic = request.IsPublic,
                IsFork = request.IsFork,
                ParentCraftId = request.ParentCraftId,
                CreatedByUsername = username
            });
        }
        else
        {
            if (existing.CreatedByUsername != username) return Forbid();
            existing.Name = request.Name;
            existing.Js = request.Js;
            existing.Css = request.Css;
            existing.Html = request.Html;
            existing.IsPublic = request.IsPublic;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [Authorize]
    [HttpPatch("like")]
    public async Task<IActionResult> Like([FromQuery] string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();

        var like = await db.CraftLikes.FirstOrDefaultAsync(l => l.CraftId == craft.Id && l.Username == username);
        if (like is null) db.CraftLikes.Add(new CraftLike { CraftId = craft.Id, Username = username });
        else db.CraftLikes.Remove(like);

        await db.SaveChangesAsync();
        return await Get(craftId);
    }

    [HttpPatch("view")]
    public async Task<IActionResult> ViewIncrement([FromQuery] string craftId)
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var craft = await db.Crafts.FirstOrDefaultAsync(c => c.CraftId == craftId);
        if (craft is null) return NotFound();

        db.CraftViews.Add(new CraftView { CraftId = craft.Id, Username = username });
        await db.SaveChangesAsync();
        return Ok();
    }
}
