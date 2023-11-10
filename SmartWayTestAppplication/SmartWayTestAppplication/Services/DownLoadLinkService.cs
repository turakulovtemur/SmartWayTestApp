using Microsoft.EntityFrameworkCore;
using SmartWayTestAppplication.Dto.File;
using SmartWayTestAppplication.Models;
using System.IO.Compression;

namespace SmartWayTestAppplication.Services
{
    public class DownLoadLinkService : IDownLoadLinkService
    {
        private readonly ApplicationContext _context;
        public DownLoadLinkService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GetFileToDownload(Guid linkId, CancellationToken token)
        {
            var fileIds = await _context.FilesDownloadLinks
                 .Where(x => x.DownloadLinkId == linkId)
                 .Select(x => x.FileId)
                 .ToListAsync(token)
                 .ConfigureAwait(false);

            var files = await _context.Files
                 .AsNoTracking()
                 .Where(x => fileIds.Contains(x.Id))
                 .Select(x => new FileDto(x))
                 .ToListAsync(token)
                 .ConfigureAwait(false);

            var result = new List<string>();

            foreach (var file in files)
            {
                result.Add($"{file.Path}\\{file.Name}{file.Extensions}");
            }

            string archiveName = "files.zip";
            string temporaryPath = Path.Combine(Path.GetTempPath(), archiveName);

            using (FileStream zipToCreate = new FileStream(temporaryPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    foreach (string filePath in result)
                    {
                        string fileName = Path.GetFileName(filePath);
                        archive.CreateEntryFromFile(filePath, fileName);
                    }
                }
            }

            byte[] fileBytes = File.ReadAllBytes(temporaryPath);
            File.Delete(temporaryPath);

            return fileBytes;
        }

        public async Task<string> CreateLink(Guid[] fileIds, long userId, CancellationToken token)
        {
            var files = await _context.Files
                        .Where(x => fileIds.Contains(x.Id) && x.UserId == userId).ToListAsync();

            if (fileIds.Length != files.Count)
            {
                throw new FileNotFoundException();
            }
            var id = Guid.NewGuid();
            var downloadLink = new DownloadLink
            {
                Id = id,
                Link = $"api/file/link/{id.ToString("N")}",
                IsActive = true
            };
            await _context.DownloadLinks.AddAsync(downloadLink);

            foreach (var fileId in fileIds)
            {
                await _context.FilesDownloadLinks.AddAsync(new FileDownloadLink
                {
                    Id = Guid.NewGuid(),
                    DownloadLinkId = id,
                    FileId = fileId
                });
            }
            await _context.SaveChangesAsync();

            return downloadLink.Link;
        }
    }
}
