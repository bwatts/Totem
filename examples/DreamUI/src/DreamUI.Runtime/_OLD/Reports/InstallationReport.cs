using Totem;

namespace DreamUI.Installations.Reports;

public class InstallationReport : Report<InstallationRow>
{
    public static Id Route(InstallationStarted e) => e.InstallationId;
    public static Id Route(InstallVersionSent e) => e.InstallationId;
    public static Id Route(InstallVersionFailed e) => e.InstallationId;

    public void When(InstallationStarted _) =>
        Row.Status = InstallationStatus.Started;

    public void When(InstallVersionSent _) =>
        Row.Status = InstallationStatus.Sent;

    public void When(InstallVersionFailed e)
    {
        Row.Status = InstallationStatus.Error;
        Row.Error = e.Message;
    }
}
