namespace SmartWayTestAppplication.Dto.File
{
    public class FileDto
    {
        public FileDto() { }
        public FileDto(Models.FileModel file)
        {
            Id = file.Id;
            Path = file.Path;
            Name = file.Name;
            Extensions = file.Extensions;
            UserId = file.UserId;
        }
        public Guid Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Extensions { get; set; }
        public long UserId { get; set; }
    }
}
