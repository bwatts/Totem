using Totem;

namespace Dream.Versions.Workflows;

public class DownloadVersionWorkflow : Workflow
{
    public static Id Route(VersionInstalled e) => e.VersionId;
    public static Id Route(VersionDownloaded e) => e.VersionId;

    VersionInstalled? _installed;

    public void Given(VersionInstalled e) => _installed = e;
    public void Given(VersionDownloaded _) => _installed = null;

    public void When(VersionInstalled _)
    {
        if(_installed is null)
        {
            return;
        }

        ThenEnqueue(new DownloadVersion
        {
            VersionId = _installed.VersionId,
            ZipUrl = _installed.ZipUrl
        });
    }
}
