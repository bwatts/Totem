using Totem;

namespace Dream.Versions.Workflows;

public class UnpackVersionWorkflow : Workflow
{
    public static Id Route(VersionDownloaded e) => e.VersionId;
    public static Id Route(VersionUnpacked e) => e.VersionId;

    VersionDownloaded? _downloaded;

    public void Given(VersionDownloaded e) => _downloaded = e;
    public void Given(VersionUnpacked _) => _downloaded = null;

    public void When(VersionDownloaded _)
    {
        if(_downloaded is null)
        {
            return;
        }

        ThenEnqueue(new UnpackVersion
        {
            VersionId = _downloaded.VersionId,
            ZipPath = _downloaded.ZipPath
        });
    }
}
