using Microsoft.EntityFrameworkCore;
using SmartWayTestAppplication.Dto.File;
using SmartWayTestAppplication.Models;
using System.Linq;
using System.Text;
using FileNotFoundException = SmartWayTestAppplication.Exceptions.FilesNotFoundException;

namespace SmartWayTestAppplication.Services
{
    public class FileService : IFileService
    {
        private readonly ApplicationContext _context;
        public FileService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<FileDto> Create(FileDto model, CancellationToken cancellation)
        {
            if (model == null)
            {
                throw new ArgumentNullException("File is null");
            }

            var file = new FileModel
            {
                Id = model.Id,
                Name = model.Name,
                Path = model.Path,
                Extensions = model.Extensions,
                UserId = model.UserId
            };

            await _context.Files.AddAsync(file, cancellation).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellation).ConfigureAwait(false);

            return new FileDto(file);
        }

        public async Task DeleteById(Guid Id, CancellationToken cancellation)
        {
            var file = await _context.Files
                 .FirstOrDefaultAsync(x => x.Id == Id, cancellation);

            if (file is null)
            {
                throw new ArgumentNullException("File is null");
            }

            _context.Files.Remove(file);
            await _context.SaveChangesAsync(cancellation).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FileDto>> GetAll(CancellationToken cancellation)
        {
            return await _context.Files
                 .AsNoTracking()
                 .Select(x => new FileDto(x))
                 .ToListAsync(cancellation)
                 .ConfigureAwait(false);
        }

        public async Task<FileDto> GetById(Guid Id, CancellationToken cancellation)
        {
            var file = await _context.Files
                .FirstOrDefaultAsync(x => x.Id == Id, cancellation)
                .ConfigureAwait(false);

            if (file is null)
            {
                throw new FileNotFoundException();
            }

            return new FileDto(file);
        }

        public async Task<FileDto> Update(FileDto model, CancellationToken cancellation)
        {
            var files = await _context.Files
                .FirstOrDefaultAsync(x => x.Id == model.Id, cancellation)
                .ConfigureAwait(false);

            if (files is null)
            {
                throw new FileNotFoundException();
            }

            files.Name = model.Name;
            files.Path = model.Path;
            files.Extensions = model.Extensions;
            files.UserId = model.UserId;

            await _context.SaveChangesAsync(cancellation);

            return new FileDto(files);
        }

        public async Task<string> GetFilePath(Guid FileId, CancellationToken cancellation)
        {
            var file = await _context.Files
               .FirstOrDefaultAsync(x => x.Id == FileId, cancellation)
               .ConfigureAwait(false);
            var path = $"{file.Path}\\{file.Name}{file.Extensions}";
            return path;
        }

        public async Task<string[]> GetFilesPaths(Guid[] FileIds, CancellationToken cancellation)
        {
            var files= await _context.Files
                 .AsNoTracking()
                 .Where(x => FileIds.Contains(x.Id))
                 .Select(x => new FileDto(x))
                 .ToListAsync(cancellation)
                 .ConfigureAwait(false);
             
            var result=new List<string>();

            foreach (var file in files)
            {
                result.Add($"{file.Path}\\{file.Name}{file.Extensions}");
            }
            return result.ToArray();
        }
    }
}
