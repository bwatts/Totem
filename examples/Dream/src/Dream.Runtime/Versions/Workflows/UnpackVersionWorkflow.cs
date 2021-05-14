using Totem;

namespace Dream.Versions.Workflows
{
    public class UnpackVersionWorkflow : Workflow
    {
        public static Id Route(VersionDownloaded e) => e.VersionId;
        public static Id Route(VersionUnpacked e) => e.VersionId;

        VersionDownloaded? _downloaded;

        public UnpackVersionWorkflow()
        {
            Given<VersionDownloaded>(e => _downloaded = e);
            Given<VersionUnpacked>(e => _downloaded = null);
        }

        public void When(VersionDownloaded _)
        {
            if(_downloaded != null)
            {
                ThenEnqueue(new UnpackVersion
                {
                    VersionId = _downloaded.VersionId,
                    ZipPath = _downloaded.ZipPath
                });
            }
        }
    }
}