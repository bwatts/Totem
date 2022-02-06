using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions.Topics;

public class DownloadTopic : Topic
{
    public static Id Route(DownloadVersion command) => command.VersionId;

    readonly IVersionService _service;
    bool _downloaded;

    public DownloadTopic(IVersionService service) =>
        _service = service;

    public void Given(VersionDownloaded _) =>
        _downloaded = true;

    public async Task When(DownloadVersion command, CancellationToken cancellationToken)
    {
        if(_downloaded)
        {
            return;
        }

        var file = await _service.DownloadAsync(command.ZipUrl, cancellationToken);

        Then(new VersionDownloaded(command.VersionId, command.ZipUrl, file.Path, file.ByteCount));
    }
}
