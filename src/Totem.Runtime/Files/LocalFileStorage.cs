using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Files
{
    public class LocalFileStorage : IFileStorage
    {
        readonly ILocalFileStorageSettings _settings;

        public LocalFileStorage(ILocalFileStorageSettings settings) =>
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        public Task<bool> ExistsAsync(FilePath path, CancellationToken cancellationToken)
        {
            if(path is null)
                throw new ArgumentNullException(nameof(path));

            return Task.FromResult(File.Exists(GetLocalPath(path)));
        }

        public Task<Stream> GetAsync(FilePath path, CancellationToken cancellationToken)
        {
            if(path is null)
                throw new ArgumentNullException(nameof(path));

            var localPath = GetLocalPath(path);

            return Task.FromResult<Stream>(File.OpenRead(localPath));
        }

        public async IAsyncEnumerable<FileItem> ListAsync(IFileQuery query, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            await Task.Yield();

            var directoryPath = query.Prefix != null
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

            if(data == null)
                throw new ArgumentNullException(nameof(data));

            var localPath = GetLocalPath(path);
            var directoryPath = Path.GetDirectoryName(localPath)!;

            Directory.CreateDirectory(directoryPath);

            using var localFileStream = File.Open(localPath, FileMode.Create);

            await data.CopyToAsync(localFileStream, cancellationToken);
            await localFileStream.FlushAsync(cancellationToken);

            return new FileItem(path, new FileInfo(localPath).Length);
        }

        public Task RemoveAsync(FilePath path, CancellationToken cancellationToken)
        {
            if(path is null)
                throw new ArgumentNullException(nameof(path));

            File.Delete(GetLocalPath(path));

            return Task.CompletedTask;
        }

        string GetLocalPath(FilePath path) =>
            Path.Combine(_settings.BaseDirectory, path.Root, NormalizeSeparators(path.Key));

        string GetKey(string root, string fileName) =>
            Path.GetRelativePath(Path.Combine(_settings.BaseDirectory, root), fileName);

        static string NormalizeSeparators(string path) =>
            path.Replace("/", "\\").Trim('\\');
    }
}