using Microsoft.AspNetCore.Identity;

namespace SmartWayTestAppplication.Models
{
    public class User:IdentityUser<long>
    { 
        public string Name { get; set; } = default!;
        public string Login { get; set; } = default!;
        public string Password { get; set; } = default!; 
        public List<FileModel> File { get; set; } = default!;
        public IEnumerable<Token> Tokens { get; set; } = new List<Token>(); 
    }
}
