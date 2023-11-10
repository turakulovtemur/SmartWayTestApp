namespace SmartWayTestAppplication.Dto.File
{
    public class CreateFileRequestModel
    {
        public string Path { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Extensions { get; set; } = default!;
        public long UserId { get; set; } = default!;
    }
}
