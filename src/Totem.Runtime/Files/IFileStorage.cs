using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Files;

public interface IFileStorage
{
    Task<bool> ExistsAsync(FilePath path, CancellationToken cancellationToken);
    Task<Stream> GetAsync(FilePath path, CancellationToken cancellationToken);
    IAsyncEnumerable<FileItem> ListAsync(IFileQuery query, CancellationToken cancellationToken);
    Task<FileItem> PutAsync(FilePath path, Stream data, CancellationToken cancellationToken);
    Task RemoveAsync(FilePath path, CancellationToken cancellationToken);
}
