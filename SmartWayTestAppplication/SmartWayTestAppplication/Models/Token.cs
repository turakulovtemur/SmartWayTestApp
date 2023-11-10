namespace SmartWayTestAppplication.Models
{
    public sealed class Token
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }

        public DateTime IssuedDate { get; set; }

        public string? AccessToken { get; set; }

        public DateTime? AccessExpiresDate { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshExpiresDate { get; set; }

        public User User { get; set; } = null!;
    }
}
