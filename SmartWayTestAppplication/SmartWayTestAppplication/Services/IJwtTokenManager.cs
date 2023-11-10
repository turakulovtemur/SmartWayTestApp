using SmartWayTestAppplication.Models;

namespace SmartWayTestAppplication.Services
{
    public interface IJwtTokenManager
    {
        Task<JwtTokenModel> GenerateJwtToken( long userId, IEnumerable<string>? roles, IDictionary<string, string>? userClaims);
    }
}
