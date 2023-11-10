using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartWayTestAppplication.Dto.File;
using SmartWayTestAppplication.Dto.User;
using SmartWayTestAppplication.Enums;
using SmartWayTestAppplication.Exceptions;
using SmartWayTestAppplication.Models;
using SmartWayTestAppplication.Services;

namespace SmartWayTestAppplication.Repositories
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        public readonly IPasswordHasher<User> _passwordHasher;
        public UserService(ApplicationContext context, UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Create(UserDto model, CancellationToken cancellation)
        {
            if (model is null)
            {
                throw new ArgumentNullException("User is null");
            }

            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                throw new Exception($"User {model.Email} already exist");
            }

            var user = new User
            {
                Id = model.Id,
                Login = model.Email,
                Password = model.Password,
                Name = model.UserName
            };

            await _context.Users.AddAsync(user, cancellation).ConfigureAwait(false);

            var hashedPassword = _passwordHasher.HashPassword(user, model.Password);
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.PasswordHash = hashedPassword;

            await _context.SaveChangesAsync(cancellation).ConfigureAwait(false);
            await _userManager.AddToRoleAsync(user, RoleType.User.ToString());

            return new UserDto(user);
        }

        public async Task DeleteById(long Id, CancellationToken cancellation)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == Id, cancellation);

            if (user is null)
            {
                throw new ArgumentNullException("User is null");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellation).ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserDto>> GetAll(CancellationToken cancellation)
        {
            return await _context.Users
                .AsNoTracking()
                .Select(x => new UserDto(x))
                .ToListAsync(cancellation)
                .ConfigureAwait(false);
        }

        public async Task<UserDto> GetById(long Id, CancellationToken cancellation)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == Id, cancellation)
                .ConfigureAwait(false);

            if (user is null)
            {
                throw new UserNotFoundException();
            }

            return new UserDto(user);
        }

        public async Task<UserDto> Update(UserDto model, CancellationToken cancellation)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == model.Id, cancellation)
                .ConfigureAwait(false);

            if (user is null)
            {
                throw new UserNotFoundException();
            }

            user.Login = model.Email;
            user.Password = model.Password;
            user.Name = model.UserName;

            await _context.SaveChangesAsync(cancellation);

            return new UserDto(user);
        }

        public async Task<IEnumerable<FileDto>> GetUserAllFiles(long Id, CancellationToken cancellation)
        {
            return await _context.Files
                 .AsNoTracking()
                 .Where(x => x.UserId == Id)
                 .Select(x => new FileDto(x))
                 .ToListAsync(cancellation)
                 .ConfigureAwait(false);
        }
    }
}
