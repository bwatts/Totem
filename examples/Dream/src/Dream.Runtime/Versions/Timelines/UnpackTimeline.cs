using System;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions.Timelines
{
    public class UnpackTimeline : Timeline
    {
        public static Id DeriveId(Id versionId) =>
            versionId?.DeriveId(nameof(UnpackTimeline)) ?? throw new ArgumentNullException(nameof(versionId));

        bool _unpacked;

        public UnpackTimeline(Id id) : base(id)
        {
            Given<VersionUnpacked>(e => _unpacked = true);
        }

        public async Task WhenAsync(UnpackVersion command, IVersionService service, CancellationToken cancellationToken)
        {
            if(_unpacked)
            {
                return;
            }

            var result = await service.UnpackAsync(command.VersionId, command.ZipPath, cancellationToken);

            Then(new VersionUnpacked(command.VersionId, command.ZipPath, result.FileCount, result.ByteCount, result.ExePath));
        }
    }
}