using System;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions.Projections
{
    public class VersionReport : Report
    {
        public static Id Route(VersionInstalled e) => e.VersionId;
        public static Id Route(VersionDownloaded e) => e.VersionId;
        public static Id Route(VersionUnpacked e) => e.VersionId;

        readonly IVersionRepository _repository;

        public VersionReport(IVersionRepository repository) =>
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Task WhenAsync(VersionInstalled e, CancellationToken cancellationToken) =>
            _repository.SaveAsync(new VersionRecord
            {
                Id = e.VersionId,
                ZipUrl = e.ZipUrl,
                Status = VersionStatus.Downloading
            }, cancellationToken);

        public async Task WhenAsync(VersionDownloaded e, CancellationToken cancellationToken)
        {
            var record = await _repository.GetAsync(e.VersionId, cancellationToken);

            if(record == null)
            {
                ThenError(RuntimeErrors.VersionNotFound);
                return;
            }

            record.Status = VersionStatus.Unpacking;
            record.ZipFile = new() { Path = e.ZipPath, ByteCount = e.ByteCount };

            await _repository.SaveAsync(record, cancellationToken);
        }

        public async Task WhenAsync(VersionUnpacked e, CancellationToken cancellationToken)
        {
            var record = await _repository.GetAsync(e.VersionId, cancellationToken);

            if(record == null)
            {
                ThenError(RuntimeErrors.VersionNotFound);
                return;
            }

            record.Status = VersionStatus.Ready;
            record.ZipContent = new()
            {
                FileCount = e.FileCount,
                ByteCount = e.ByteCount,
                ExePath = e.ExePath
            };

            await _repository.SaveAsync(record, cancellationToken);
        }
    }
}