using System.Runtime.CompilerServices;

namespace Totem.Files;

public class DiskStorage : IFileStorage
{
    public const string BaseDirectoryConfigurationKey = "Totem:Files:DiskStorage:BaseDirectory";

    readonly IDiskStorageSettings _settings;

    public DiskStorage(IDiskStorageSettings settings) =>
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

    public Task<bool> ExistsAsync(FilePath path, CancellationToken cancellationToken)
    {
        if(path is null)
            throw new ArgumentNullException(nameof(path));

        return Task.FromResult(File.Exists(GetDiskPath(path)));
    }

    public Task<Stream> GetAsync(FilePath path, CancellationToken cancellationToken)
    {
        if(path is null)
            throw new ArgumentNullException(nameof(path));

        var diskPath = GetDiskPath(path);

        return Task.FromResult<Stream>(File.OpenRead(diskPath));
    }

    public async IAsyncEnumerable<FileItem> ListAsync(IFileQuery query, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if(query is null)
            throw new ArgumentNullException(nameof(query));

        await Task.Yield();

        var directoryPath = query.Prefix is not null
            ? Path.Combine(_settings.BaseDirectory, query.Root, query.Prefix)
            : Path.Combine(_settings.BaseDirectory, query.Root);

        foreach(var file in
            from file in new DirectoryInfo(directoryPath).GetFiles("*", SearchOption.AllDirectories)
            let key = GetKey(query.Root, file.FullName)
            where query.IncludeKey(key)
            select new FileItem(new FilePath(query.Root, key), file.Length))
        {
            yield return file;
        }
    }

    public async Task<FileItem> PutAsync(FilePath path, Stream data, CancellationToken cancellationToken)
    {
        if(path is null)
            throw new ArgumentNullException(nameof(path));

        if(data is null)
            throw new ArgumentNullException(nameof(data));

        var diskPath = GetDiskPath(path);
        var directoryPath = Path.GetDirectoryName(diskPath)!;

        Directory.CreateDirectory(directoryPath);

        using var file = File.Open(diskPath, FileMode.Create);

        await data.CopyToAsync(file, cancellationToken);
        await file.FlushAsync(cancellationToken);

        return new FileItem(path, new FileInfo(diskPath).Length);
    }

    public Task RemoveAsync(FilePath path, CancellationToken cancellationToken)
    {
        if(path is null)
            throw new ArgumentNullException(nameof(path));

        File.Delete(GetDiskPath(path));

        return Task.CompletedTask;
    }

    string GetDiskPath(FilePath path) =>
        Path.Combine(_settings.BaseDirectory, path.Root, NormalizeSeparators(path.Key));

    string GetKey(string root, string fileName) =>
        Path.GetRelativePath(Path.Combine(_settings.BaseDirectory, root), fileName);

    static string NormalizeSeparators(string path) =>
        path.Replace("/", "\\").Trim('\\');
}
