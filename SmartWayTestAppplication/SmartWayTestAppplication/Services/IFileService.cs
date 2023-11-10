using SmartWayTestAppplication.Dto.File;

namespace SmartWayTestAppplication.Services
{
    public interface IFileService
    {
        public Task<IEnumerable<FileDto>> GetAll(CancellationToken cancellation);
        public Task<FileDto> GetById(Guid Id, CancellationToken cancellation);
        public Task<FileDto> Create(FileDto model, CancellationToken cancellation);
        public Task<FileDto> Update(FileDto model, CancellationToken cancellation);
        public Task DeleteById(Guid Id, CancellationToken cancellation);
        public Task<string>GetFilePath(Guid FileId, CancellationToken cancellation);
        public Task<string[]> GetFilesPaths(Guid[] FileIds, CancellationToken cancellation);
    }
}
