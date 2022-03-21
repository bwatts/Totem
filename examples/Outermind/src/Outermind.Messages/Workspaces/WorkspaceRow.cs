namespace Outermind.Workspaces;

public class WorkspaceRow : IReportRow
{
    public Id Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Link { get; set; } = null!;
    public List<Branch> Branches { get; } = new();

    public class Branch
    {
        public Id Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Link { get; set; } = null!;
    }
}
