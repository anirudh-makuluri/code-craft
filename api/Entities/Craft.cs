using System.ComponentModel.DataAnnotations;

namespace api.Entities;

public class Craft
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string CraftId { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(64)]
    public string CreatedByUsername { get; set; } = string.Empty;

    public User? CreatedByUser { get; set; }

    public string Js { get; set; } = string.Empty;
    public string Css { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;
    public bool IsFork { get; set; }

    public string? ParentCraftId { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<CraftLike> Likes { get; set; } = new List<CraftLike>();
    public ICollection<CraftView> Views { get; set; } = new List<CraftView>();
}
