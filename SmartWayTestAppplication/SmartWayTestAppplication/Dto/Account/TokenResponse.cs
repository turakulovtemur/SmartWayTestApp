namespace SmartWayTestAppplication.Dto.Account
{
    public class TokenResponse
    {
        public string AccessToken { get; init; } = null!;
        public DateTime IssuedDate { get; set; }
        public DateTime? AccessExpiresDate { get; set; }
        public string? RefreshToken { get; init; } = null!;
        public DateTime? RefreshExpiresDate { get; set; }
    }
}
