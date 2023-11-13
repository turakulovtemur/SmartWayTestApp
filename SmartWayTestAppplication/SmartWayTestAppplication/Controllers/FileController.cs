using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWayTestAppplication.Dto.File;
using SmartWayTestAppplication.Exceptions;
using SmartWayTestAppplication.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.IO.Compression;

namespace SmartWayTestAppplication.Controllers
{    
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IDownLoadLinkService _downLoadLinkService;
        public FileController(IFileService fileService, IDownLoadLinkService downLoadLinkService)
        {
            _fileService = fileService;
            _downLoadLinkService = downLoadLinkService;
        }

        
        [SwaggerOperation("Получить файл по Id")]
        [HttpGet("{Id}")]
        public async Task<ActionResult<FileDto>> GetById(Guid Id, CancellationToken token)
        {
            try
            {
                var result = await _fileService.GetById(Id, token);
                return Ok(result);
            }
            catch (FilesNotFoundException)
            {
                return NotFound();
            }
        }

        
        [SwaggerOperation("Создать файл")]
        [HttpPost]
        public async Task<ActionResult<FileDto>> CreateFile([FromBody] CreateFileRequestModel model, CancellationToken token)
        {
            var result = await _fileService.Create(new FileDto
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Extensions = model.Extensions,
                Path = model.Path,
                UserId = model.UserId
            }, token);
            return Ok(result);
        }

        
        [SwaggerOperation("Обновить файл")]
        [HttpPut]
        public async Task<ActionResult<FileDto>> EditFile(Guid Id, [FromBody] EditFileRequestModel model, CancellationToken token)
        {
            try
            {
                var result = await _fileService.Update(new FileDto
                {
                    Id = Id,
                    Name = model.Name,
                    Extensions = model.Extensions,
                    Path = model.Path,
                    UserId = model.UserId
                }, token);
                return Ok(result);
            }
            catch (FilesNotFoundException)
            {

                return NotFound();
            }
            catch (InvalidFileException)
            {
                return BadRequest();
            }
        }

        
        [SwaggerOperation("Получить все файлы")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileDto>>> GetAll(CancellationToken token)
        {
            var result = await _fileService.GetAll(token);
            return Ok(result);
        }

        
        [SwaggerOperation("Удалить файл")]
        [HttpDelete]
        public async Task<ActionResult> DeleteFile(Guid Id, CancellationToken token)
        {
            await _fileService.DeleteById(Id, token);
            return Ok();
        }

        
        [SwaggerOperation("Скачать файл по Id")]
        [HttpGet("download/{Id}")]
        public FileContentResult DownloadFileArchive(Guid Id, CancellationToken token)
        {
            var result = _fileService.GetFilePath(Id, token);
            string[] filePaths = new string[1];
            filePaths[0] = result.Result;

            string archiveName = "files.zip";
            string temporaryPath = Path.Combine(Path.GetTempPath(), archiveName);

            using (FileStream zipToCreate = new FileStream(temporaryPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    foreach (string filePath in filePaths)
                    {
                        string fileName = Path.GetFileName(filePath);
                        archive.CreateEntryFromFile(filePath, fileName);
                    }
                }
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(temporaryPath);
            System.IO.File.Delete(temporaryPath);

            return File(fileBytes, "application/zip", archiveName);
        }

        
        [SwaggerOperation("Скачать группу файлов по Id")]
        [HttpPost("download")]
        public FileContentResult DownloadFileArchive(Guid[] Ids, CancellationToken token)
        {
            var filePaths = _fileService.GetFilesPaths(Ids, token).Result;

            string archiveName = "files.zip";
            string temporaryPath = Path.Combine(Path.GetTempPath(), archiveName);

            using (FileStream zipToCreate = new FileStream(temporaryPath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    foreach (string filePath in filePaths)
                    {
                        string fileName = Path.GetFileName(filePath);
                        archive.CreateEntryFromFile(filePath, fileName);
                    }
                }
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(temporaryPath);
            System.IO.File.Delete(temporaryPath);

            return File(fileBytes, "application/zip", archiveName);
        }

        
        [SwaggerOperation("Скачать файлы по ссылке")]
        [HttpGet("link/{id}")]
        public async Task<IActionResult> DownloadByLink(string id, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(id, out var linkId))
            {
                return BadRequest();
            }
            string archiveName = "files.zip";
            var bytes= await _downLoadLinkService.GetFileToDownload(linkId, cancellationToken);
            return File(bytes, "application/zip", archiveName);
        }

        
        [SwaggerOperation("Сформировать ссылку для скачивания файлов")]
        [HttpGet("link")]
        public async Task<ActionResult> CreateDownloadLink([FromQuery]Guid[] fileIds,[FromQuery] long userId, CancellationToken token)
        {
            try
            {
                var route= await _downLoadLinkService.CreateLink(fileIds, userId, token);

                var link = $"{Request.Host}/{route}"; 

                return Ok(link);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
