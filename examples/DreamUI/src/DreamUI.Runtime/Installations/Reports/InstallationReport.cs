using System;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace DreamUI.Installations.Reports
{
    public class InstallationReport : Report
    {
        public static Id Route(InstallationStarted e) => e.InstallationId;
        public static Id Route(InstallVersionSent e) => e.InstallationId;
        public static Id Route(InstallVersionFailed e) => e.InstallationId;

        readonly IInstallationRepository _repository;

        public InstallationReport(IInstallationRepository repository) =>
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Task WhenAsync(InstallationStarted e, CancellationToken cancellationToken) =>
            _repository.SaveAsync(new InstallationRecord { Id = e.InstallationId }, cancellationToken);

        public async Task WhenAsync(InstallVersionSent e, CancellationToken cancellationToken)
        {
            var record = await _repository.GetAsync(e.InstallationId, cancellationToken);

            if(record == null)
            {
                ThenError(RuntimeErrors.InstallationNotFound);
                return;
            }

            record.InstallVersionSent = true;

            await _repository.SaveAsync(record, cancellationToken);
        }

        public async Task WhenAsync(InstallVersionFailed e, CancellationToken cancellationToken)
        {
            var record = await _repository.GetAsync(e.InstallationId, cancellationToken);

            if(record == null)
            {
                ThenError(RuntimeErrors.InstallationNotFound);
                return;
            }

            record.InstallVersionError = e.Message;

            await _repository.SaveAsync(record, cancellationToken);
        }
    }
}