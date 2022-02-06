using Totem;

namespace Dream.Versions.Reports;

public class VersionReport : Report<VersionRow>
{
    public static Id Route(VersionInstalled e) => e.VersionId;
    public static Id Route(VersionDownloaded e) => e.VersionId;
    public static Id Route(VersionUnpacked e) => e.VersionId;

    public void When(VersionInstalled e)
    {
        Row.ZipUrl = e.ZipUrl;
        Row.Status = VersionStatus.Downloading;
    }

    public void When(VersionDownloaded e)
    {
        Row.Status = VersionStatus.Unpacking;
        Row.ZipFile = new() { Path = e.ZipPath, ByteCount = e.ByteCount };
    }

    public void When(VersionUnpacked e)
    {
        Row.Status = VersionStatus.Ready;
        Row.ZipContent = new()
        {
            FileCount = e.FileCount,
            ByteCount = e.ByteCount,
            ExePath = e.ExePath
        };
    }
}
