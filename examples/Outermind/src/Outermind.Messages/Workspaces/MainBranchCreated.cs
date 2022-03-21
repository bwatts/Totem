namespace Outermind.Workspaces;

public class MainBranchCreated : IEvent
{
    public MainBranchCreated(Id workspaceId, Id branchId, string name, string link)
    {
        WorkspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
        BranchId = branchId ?? throw new ArgumentNullException(nameof(branchId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Link = link ?? throw new ArgumentNullException(nameof(link));
    }

    public Id WorkspaceId { get; }
    public Id BranchId { get; }
    public string Name { get; }
    public string Link { get; }
}
