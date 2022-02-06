using System;
using Totem;

namespace DreamUI.Installations;

public class InstallationStarted : IEvent
{
    public InstallationStarted(Id installationId) =>
        InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));

    public Id InstallationId { get; }
}
