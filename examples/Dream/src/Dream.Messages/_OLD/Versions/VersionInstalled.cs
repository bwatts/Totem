using System;
using Totem;

namespace Dream.Versions;

public class VersionInstalled : IEvent
{
    public VersionInstalled(Id versionId, Uri zipUrl)
    {
        VersionId = versionId ?? throw new ArgumentNullException(nameof(versionId));
        ZipUrl = zipUrl ?? throw new ArgumentNullException(nameof(zipUrl));
    }

    public Id VersionId { get; }
    public Uri ZipUrl { get; }
}
