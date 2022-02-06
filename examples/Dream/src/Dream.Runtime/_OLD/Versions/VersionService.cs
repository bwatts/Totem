using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem;
using Totem.Files;

namespace Dream.Versions;

public class VersionService : IVersionService
{
    public const string DownloadsRoot = "version-downloads";
    public const string ContentRoot = "version-content";
    public const string ExeName = "EventStore.ClusterNode.exe";

    readonly ILogger _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly IFileStorage _fileStorage;

    public VersionService(ILogger<VersionService> logger, IHttpClientFactory httpClientFactory, IFileStorage fileStorage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
    }

    public async Task<FileItem> DownloadAsync(Uri zipUrl, CancellationToken cancellationToken)
    {
        if(zipUrl is null)
            throw new ArgumentNullException(nameof(zipUrl));

        _logger.LogTrace("[version] Download file {ZipUrl}", zipUrl);

        using var client = _httpClientFactory.CreateClient();
        using var data = await client.GetStreamAsync(zipUrl, cancellationToken);

        var path = new FilePath(DownloadsRoot, Path.GetFileName(zipUrl.AbsolutePath));

        _logger.LogTrace("[version] PUT zip file {ZipPath}", path);

        var item = await _fileStorage.PutAsync(path, data, cancellationToken);

        _logger.LogTrace("[version] PUT complete of zip file {ZipPath} (bytes: {ByteCount})", path, item.ByteCount);

        return item;
    }

    public async Task<UnpackResult> UnpackAsync(Id versionId, FilePath zipPath, CancellationToken cancellationToken)
    {
        if(versionId is null)
            throw new ArgumentNullException(nameof(versionId));

        if(zipPath is null)
            throw new ArgumentNullException(nameof(zipPath));

        _logger.LogTrace("[version] Unpack zip file {ZipPath}", zipPath);

        using var zipStream = await _fileStorage.GetAsync(zipPath, cancellationToken);
        using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        var byteCount = 0L;
        var exePath = default(FilePath);

        foreach(var entry in zipArchive.Entries)
        {
            var key = entry.FullName.Trim(FilePath.Separator);

            using var entryStream = entry.Open();

            var file = await _fileStorage.PutAsync(ContentRoot, key, entryStream, cancellationToken);

            byteCount += file.ByteCount;

            if(Path.GetFileName(entry.FullName) == ExeName)
            {
                exePath = new FilePath(ContentRoot, key);
            }
        }

        if(exePath is null)
            throw new InvalidOperationException($"Expected exe file {ExeName} in zip file");

        _logger.LogTrace("[version] Unpack {ZipPath} complete (files: {FileCount}, bytes: {ByteCount})", zipPath, zipArchive.Entries.Count, byteCount);

        return new UnpackResult(byteCount, zipArchive.Entries.Count, exePath);
    }
}
