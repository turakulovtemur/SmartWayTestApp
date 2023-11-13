
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWayTestAppplication.Dto.File;
using SmartWayTestAppplication.Dto.User;
using SmartWayTestAppplication.Enums;
using SmartWayTestAppplication.Exceptions;
using SmartWayTestAppplication.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartWayTestAppplication.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        
        [SwaggerOperation("Получить пользователя по Id")]
        [HttpGet("{Id}")]
        public async Task<ActionResult<UserDto>> GetById(long Id, CancellationToken token)
        {
            try
            {
                var user = await _userService.GetById(Id, token);
                return Ok(user);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        
        [SwaggerOperation("Создать пользователя")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] RequestModel model, CancellationToken token)
        {
            var user = await _userService.Create(new UserDto
            {               
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password
            }, token);
            return Ok(user);
        }

        
        [SwaggerOperation("Обновить пользователя")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> EditUser([FromRoute]long Id, [FromBody] EditRequestModel model, CancellationToken token)
        {
            try
            {
                var user = await _userService.Update(new UserDto
                {
                    Id = Id,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password 
                }, token);
                return Ok(user);
            }
            catch (UserNotFoundException)
            {

                return NotFound();
            }
            catch (InvalidUserException)
            {
                return BadRequest();
            }
        }

       
        [SwaggerOperation("Получить всех пользователей")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken token)
        {
            var result = await _userService.GetAll(token);
            return Ok(result);
        }

        
        [SwaggerOperation("Удалить пользователя")]
        [HttpDelete]
        public async Task<ActionResult> DeleteUser(long Id, CancellationToken token)
        {
            await _userService.DeleteById(Id, token);
            return Ok();
        }

        
        [SwaggerOperation("Получить все файлы у пользователя")]
        [HttpGet("{Id}/files")]
        public async Task<ActionResult<IEnumerable<FileDto>>> GetUserAllFiles(long Id, CancellationToken token)
        {
            var result=await _userService.GetUserAllFiles(Id, token);
            return Ok(result);
        }
    }
}
