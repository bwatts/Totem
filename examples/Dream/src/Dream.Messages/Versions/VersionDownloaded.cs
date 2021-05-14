using System;
using Totem;

namespace Dream.Versions
{
    public class VersionDownloaded : IEvent
    {
        public VersionDownloaded(Id versionId, Uri zipUrl, FilePath zipPath, long byteCount)
        {
            VersionId = versionId ?? throw new ArgumentNullException(nameof(versionId));
            ZipUrl = zipUrl ?? throw new ArgumentNullException(nameof(zipUrl));
            ZipPath = zipPath ?? throw new ArgumentNullException(nameof(zipPath));
            ByteCount = byteCount >= 0 ? byteCount : throw new ArgumentOutOfRangeException(nameof(byteCount));
        }

        public Id VersionId { get; }
        public Uri ZipUrl { get; }
        public FilePath ZipPath { get; }
        public long ByteCount { get; }
    }
}