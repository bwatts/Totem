using System;
using Totem;

namespace DreamUI.Installations;

public class InstallVersionFailed : IEvent
{
    public InstallVersionFailed(Id installationId, string message)
    {
        InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public Id InstallationId { get; }
    public string Message { get; }
}
