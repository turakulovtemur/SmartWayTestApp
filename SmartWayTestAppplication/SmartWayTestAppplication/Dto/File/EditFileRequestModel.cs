namespace SmartWayTestAppplication.Dto.File
{
    public class EditFileRequestModel
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Extensions { get; set; }
        public long UserId { get; set; }
    }
}
