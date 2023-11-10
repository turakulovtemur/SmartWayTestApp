using Microsoft.IdentityModel.Tokens;

namespace SmartWayTestAppplication.Models
{
    public class JwtConfiguration
    {
        public string SecretKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public TimeSpan AccessExpiration { get; set; }
        public TimeSpan RefreshExpiration { get; set; }
        public short RefreshLength { get; set; }
        public SigningCredentials SigningCredentials { get; set; } = null!;
    }
}
