using System.ComponentModel.DataAnnotations;
using Totem;

namespace DreamUI.Installations;

public class SendInstallVersion : IQueueCommand
{
    [Required]
    public Id InstallationId { get; set; } = null!;
}
