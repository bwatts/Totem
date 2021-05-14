using System;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions.Timelines
{
    public class DownloadTimeline : Timeline
    {
        public static Id DeriveId(Id versionId) =>
            versionId?.DeriveId(nameof(DownloadTimeline)) ?? throw new ArgumentNullException(nameof(versionId));

        bool _downloaded;

        public DownloadTimeline(Id id) : base(id)
        {
            Given<VersionDownloaded>(e => _downloaded = true);
        }

        public async Task WhenAsync(DownloadVersion command, IVersionService service, CancellationToken cancellationToken)
        {
            if(_downloaded)
            {
                return;
            }

            var file = await service.DownloadAsync(command.ZipUrl, cancellationToken);

            Then(new VersionDownloaded(command.VersionId, command.ZipUrl, file.Path, file.ByteCount));
        }
    }
}