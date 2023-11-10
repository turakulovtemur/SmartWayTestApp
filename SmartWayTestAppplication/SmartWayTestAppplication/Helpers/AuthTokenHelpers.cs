using SmartWayTestAppplication.Models;
using System.Text;

namespace SmartWayTestAppplication.Helpers
{
    public static class AuthTokenHelpers
    {
        public static Token Create(long userId, JwtTokenModel jwtToken, JwtConfiguration jwtConfiguration)
        {
            var authToken = new Token
            {
                UserId = userId,
                IssuedDate = jwtToken.IssuedDate,
                AccessExpiresDate = jwtToken.ExpiresDate
            };

            authToken.AccessToken = jwtToken.AccessToken;
            authToken.RefreshToken = RandomString(jwtConfiguration.RefreshLength);
            authToken.RefreshExpiresDate = jwtToken.IssuedDate.Add(jwtConfiguration.RefreshExpiration);

            return authToken;
        }
        private static string RandomString(int size, bool lowerCase = true)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }
}
