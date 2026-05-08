using System.ComponentModel.DataAnnotations;

namespace api.Contracts.Auth;

public record RegisterRequest(
    [Required, MinLength(3), MaxLength(64)] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required, MinLength(2), MaxLength(128)] string Name
);

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

public record AuthResponse(string Token, UserResponse User);

public record UserResponse(string Username, string Email, string Name);
