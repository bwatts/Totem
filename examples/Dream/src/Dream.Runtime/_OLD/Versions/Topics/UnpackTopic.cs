using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions.Topics;

public class UnpackTopic : Topic
{
    public static Id Route(UnpackVersion command) => command.VersionId;

    readonly IVersionService _service;
    bool _unpacked;

    public UnpackTopic(IVersionService service) =>
        _service = service;

    public void Given(VersionUnpacked _) =>
        _unpacked = true;

    public async Task When(UnpackVersion command, CancellationToken cancellationToken)
    {
        if(_unpacked)
        {
            return;
        }

        var result = await _service.UnpackAsync(command.VersionId, command.ZipPath, cancellationToken);

        Then(new VersionUnpacked(command.VersionId, command.ZipPath, result.FileCount, result.ByteCount, result.ExePath));
    }
}
