namespace SmartWayTestAppplication.Models
{
    public class JwtTokenModel
    {
        public string AccessToken { get; init; } = null!;
        public DateTime IssuedDate { get; init; }
        public DateTime ExpiresDate { get; init; }
    }
}
