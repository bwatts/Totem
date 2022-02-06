using Totem;

namespace DreamUI.Installations;

public class InstallationRow : IReportRow
{
    public Id Id { get; set; } = null!;
    public InstallationStatus Status { get; set; }
    public string? Error { get; set; }
}
