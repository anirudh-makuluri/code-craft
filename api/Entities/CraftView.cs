namespace api.Entities;

public class CraftView
{
    public int Id { get; set; }
    public int CraftId { get; set; }
    public Craft? Craft { get; set; }

    public string? Username { get; set; }
    public DateTime ViewedAtUtc { get; set; } = DateTime.UtcNow;
}
