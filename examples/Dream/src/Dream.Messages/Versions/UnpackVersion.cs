using System.ComponentModel.DataAnnotations;
using Totem;

namespace Dream.Versions
{
    [QueueName("version-files")]
    public class UnpackVersion : IQueueCommand
    {
        [Required] public Id VersionId { get; set; } = null!;
        [Required] public FilePath ZipPath { get; set; } = null!;
    }
}