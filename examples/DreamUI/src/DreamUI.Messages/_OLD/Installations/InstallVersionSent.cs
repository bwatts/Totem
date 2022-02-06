using System;
using Totem;

namespace DreamUI.Installations;

public class InstallVersionSent : IEvent
{
    public InstallVersionSent(Id installationId) =>
        InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));

    public Id InstallationId { get; }
}
