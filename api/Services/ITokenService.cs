using api.Entities;

namespace api.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
