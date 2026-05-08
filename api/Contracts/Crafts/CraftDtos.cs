using System.ComponentModel.DataAnnotations;

namespace api.Contracts.Crafts;

public record CraftResponse(
    string CraftId,
    string Name,
    string CreatedBy,
    string Js,
    string Css,
    string Html,
    bool IsPublic,
    bool IsFork,
    string? ParentCraftId,
    int LikesCount,
    int ViewsCount,
    bool IsLikedByCurrentUser,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);

public record SaveCraftRequest(
    [Required, MinLength(4), MaxLength(120)] string Name,
    [Required] string Js,
    [Required] string Css,
    [Required] string Html,
    bool IsPublic
);

public record CreateCraftRequest(
    [Required, MinLength(3), MaxLength(64)] string CraftId,
    [Required, MinLength(4), MaxLength(120)] string Name,
    [Required] string Js,
    [Required] string Css,
    [Required] string Html,
    bool IsPublic,
    bool IsFork,
    string? ParentCraftId
);
