using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWayTestAppplication.Models
{
    [Table("FileDownloadLink")]
    public class FileDownloadLink
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public Guid DownloadLinkId { get; set; }

        [ForeignKey("FileId")] public FileModel File { get; set; } = default!;
        [ForeignKey("DownloadLinkId")] public DownloadLink DownloadLink { get; set; } = default!;
    }
}
