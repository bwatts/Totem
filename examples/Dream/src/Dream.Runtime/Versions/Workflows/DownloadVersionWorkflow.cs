using Totem;

namespace Dream.Versions.Workflows
{
    public class DownloadVersionWorkflow : Workflow
    {
        public static Id Route(VersionInstalled e) => e.VersionId;
        public static Id Route(VersionDownloaded e) => e.VersionId;

        VersionInstalled? _installed;

        public DownloadVersionWorkflow()
        {
            Given<VersionInstalled>(e => _installed = e);
            Given<VersionDownloaded>(e => _installed = null);
        }

        public void When(VersionInstalled _)
        {
            if(_installed != null)
            {
                ThenEnqueue(new DownloadVersion
                {
                    VersionId = _installed.VersionId,
                    ZipUrl = _installed.ZipUrl
                });
            }
        }
    }
}