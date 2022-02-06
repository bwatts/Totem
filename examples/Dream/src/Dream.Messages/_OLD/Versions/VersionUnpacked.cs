using System;
using Totem;

namespace Dream.Versions;

public class VersionUnpacked : IEvent
{
    public VersionUnpacked(Id versionId, FilePath zipPath, int fileCount, long byteCount, FilePath exePath)
    {
        VersionId = versionId ?? throw new ArgumentNullException(nameof(versionId));
        ZipPath = zipPath ?? throw new ArgumentNullException(nameof(zipPath));
        FileCount = fileCount >= 0 ? fileCount : throw new ArgumentOutOfRangeException(nameof(fileCount));
        ByteCount = byteCount >= 0 ? byteCount : throw new ArgumentOutOfRangeException(nameof(byteCount));
        ExePath = exePath ?? throw new ArgumentNullException(nameof(exePath));
    }

    public Id VersionId { get; }
    public FilePath ZipPath { get; }
    public int FileCount { get; }
    public long ByteCount { get; }
    public FilePath ExePath { get; }
}
