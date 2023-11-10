using SmartWayTestAppplication.Dto.File;
using SmartWayTestAppplication.Dto.User;

namespace SmartWayTestAppplication.Services
{
    public interface IUserService
    {
        public Task<UserDto> Create(UserDto model, CancellationToken cancellation);
        public Task DeleteById(long Id, CancellationToken cancellation);
        public Task<IEnumerable<UserDto>> GetAll(CancellationToken cancellation);
        public Task<UserDto> GetById(long Id, CancellationToken cancellation);
        public Task<UserDto> Update(UserDto model, CancellationToken cancellation);
        public Task<IEnumerable<FileDto>> GetUserAllFiles(long Id, CancellationToken cancellation);
    }
}
