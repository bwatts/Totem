using System;
using System.Threading;
using System.Threading.Tasks;
using Totem;
using Totem.Files;

namespace Dream.Versions;

public interface IVersionService
{
    Task<FileItem> DownloadAsync(Uri zipUrl, CancellationToken cancellationToken);
    Task<UnpackResult> UnpackAsync(Id versionId, FilePath zipPath, CancellationToken cancellationToken);
}
