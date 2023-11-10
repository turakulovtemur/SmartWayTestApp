using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartWayTestAppplication.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartWayTestAppplication.Services
{
    public class JwtTokenManager : IJwtTokenManager
    {
        private readonly JwtConfiguration _jwtConfiguration;

        public JwtTokenManager(IOptions<JwtConfiguration> jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration.Value;

            _jwtConfiguration.SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfiguration.SecretKey)),
                SecurityAlgorithms.HmacSha512Signature, SecurityAlgorithms.Sha512Digest);
        }
        public Task<JwtTokenModel> GenerateJwtToken(long userId, IEnumerable<string>? roles, IDictionary<string, string>? userClaims)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Iat, Guid.NewGuid().ToString(), ClaimValueTypes.Integer32)
            };
            if (userClaims != null) claims.AddRange(userClaims.Select(i => new Claim(i.Key, i.Value)));

            if (roles != null) claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtToken = new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims,
                utcNow,
                utcNow.Add(_jwtConfiguration.AccessExpiration),
                _jwtConfiguration.SigningCredentials);

            var encodedJwtToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return Task.FromResult(new JwtTokenModel
            {
                AccessToken = encodedJwtToken,
                IssuedDate = utcNow,
                ExpiresDate = utcNow.Add(_jwtConfiguration.AccessExpiration)
            });
        }
    }
}
