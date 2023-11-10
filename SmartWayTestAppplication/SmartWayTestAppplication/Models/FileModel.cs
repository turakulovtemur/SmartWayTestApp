using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWayTestAppplication.Models
{
    [Table("File")]
    public class FileModel
    {
        [Column("Id")][Key] public Guid Id { get; set; }
        [Column("Path")] public string Path { get; set; } = default!;
        [Column("Name")] public string Name { get; set; } = default!;
        [Column("Extensions")] public string Extensions { get; set; } = default!;
        public long UserId { get; set; }

        [ForeignKey("UserId")] public User User { get; set; } = default!;
        public List<FileDownloadLink> DownloadLink { get; set; } = default!;
    }
}
