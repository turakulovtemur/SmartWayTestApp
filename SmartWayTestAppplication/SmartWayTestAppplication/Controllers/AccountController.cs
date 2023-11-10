using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartWayTestAppplication.Dto.Account;
using SmartWayTestAppplication.Dto.User;
using SmartWayTestAppplication.Helpers;
using SmartWayTestAppplication.Models;
using SmartWayTestAppplication.Repositories;
using SmartWayTestAppplication.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace SmartWayTestAppplication.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IJwtTokenManager _jwtTokenManager;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<JwtConfiguration> _jwtConfiguration;
        private readonly IUserService _userService;
        public AccountController(ApplicationContext context,
            IJwtTokenManager jwtTokenManager,
            UserManager<User> userManager,
            IOptions<JwtConfiguration> jwtConfiguration,
            IUserService userService)
        {
            _context = context;
            _jwtTokenManager = jwtTokenManager;
            _userManager = userManager;
            _jwtConfiguration = jwtConfiguration;
            _userService = userService;
        }

        [SwaggerOperation("Войти в систему")]
        [HttpPost("token")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestModel model, CancellationToken ct)
        {
            var user=await _context.Users
                .FirstOrDefaultAsync(x => x.Login == model.Login, ct).ConfigureAwait(false);

            if (user is null)
            {
                return BadRequest(new { errorText = "User not found" });
            }

            var isValid= await _userManager.CheckPasswordAsync(user, model.Password);

            if (!isValid)
            {
                return BadRequest(new { errorText = "Invalid password" });
            }

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var now = DateTime.UtcNow;
            var jwt = await _jwtTokenManager.GenerateJwtToken(user.Id, roles, null).ConfigureAwait(false);
            var authToken = AuthTokenHelpers.Create(user.Id, jwt, _jwtConfiguration.Value);

            try
            {
                await _context.Tokens.AddAsync(authToken, ct).ConfigureAwait(false);
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            var response = new TokenResponse
            {
                AccessToken = authToken.AccessToken!,
                RefreshToken = authToken.RefreshToken,
                IssuedDate = authToken.IssuedDate,
                AccessExpiresDate = authToken.AccessExpiresDate,
                RefreshExpiresDate = authToken.RefreshExpiresDate
            };

            return Ok(response);
        }

        [SwaggerOperation("Регистрация")]
        [HttpPost("registr")]
        public async Task<IActionResult> Register(RequestModel model, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(model.UserName)&& 
                string.IsNullOrEmpty(model.Email)&& 
                string.IsNullOrEmpty(model.Password))
            {
                return BadRequest();
            }

            var userDto = await _userService.Create(new UserDto
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password
            }, ct);

            var user = new User {Id=userDto.Id,UserName=userDto.UserName,Password=userDto.Password };


            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var now = DateTime.UtcNow;
            var jwt = await _jwtTokenManager.GenerateJwtToken(user.Id, roles, null).ConfigureAwait(false);
            var authToken = AuthTokenHelpers.Create(user.Id, jwt, _jwtConfiguration.Value);

            try
            {
                await _context.Tokens.AddAsync(authToken, ct).ConfigureAwait(false);
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            var response = new TokenResponse
            {
                AccessToken = authToken.AccessToken!,
                RefreshToken = authToken.RefreshToken,
                IssuedDate = authToken.IssuedDate,
                AccessExpiresDate = authToken.AccessExpiresDate,
                RefreshExpiresDate = authToken.RefreshExpiresDate
            };

            return Ok(response);
        }
    }
}
