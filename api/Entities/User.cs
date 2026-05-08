using System.ComponentModel.DataAnnotations;

namespace api.Entities;

public class User
{
    [Key]
    [MaxLength(64)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Craft> Crafts { get; set; } = new List<Craft>();
}
