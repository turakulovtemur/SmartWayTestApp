namespace SmartWayTestAppplication.Dto.User
{
    public class UserDto
    {
        public UserDto() { }
        public UserDto(Models.User user)
        {
            Id = user.Id;
            Email = user.Login;
            Password = user.Password;
            UserName = user.Name; 
        }
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
    }
}
