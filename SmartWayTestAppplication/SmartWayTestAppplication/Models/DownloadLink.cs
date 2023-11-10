using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWayTestAppplication.Models
{
    [Table("DownloadLink")]
    public class DownloadLink
    {
        [Column("Id")][Key] public Guid Id { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("Link")] public string Link { get; set; } = default!;
        public List<FileDownloadLink> FileDownloadLink { get; set; } = default!;
    }
}
