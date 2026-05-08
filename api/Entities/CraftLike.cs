namespace api.Entities;

public class CraftLike
{
    public int Id { get; set; }
    public int CraftId { get; set; }
    public Craft? Craft { get; set; }

    public string Username { get; set; } = string.Empty;
    public User? User { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
