using System;
using System.ComponentModel.DataAnnotations;
using Totem;

namespace Dream.Versions
{
    [QueueName("version-files")]
    public class DownloadVersion : IQueueCommand
    {
        [Required] public Id VersionId { get; set; } = null!;
        [Required] public Uri ZipUrl { get; set; } = null!;
    }
}