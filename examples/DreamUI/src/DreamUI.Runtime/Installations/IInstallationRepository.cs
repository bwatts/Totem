using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace DreamUI.Installations
{
    public interface IInstallationRepository
    {
        Task<InstallationRecord?> GetAsync(Id installationId, CancellationToken cancellationToken);
        Task SaveAsync(InstallationRecord record, CancellationToken cancellationToken);
        Task RemoveAsync(Id installationId, CancellationToken cancellationToken);
    }
}