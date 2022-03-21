namespace Outermind.Workspaces;

public class CreateMainBranch : IQueueCommand
{
    public CreateMainBranch(Id workspaceId, string workspaceLink)
    {
        WorkspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
        WorkspaceLink = workspaceLink ?? throw new ArgumentNullException(nameof(workspaceLink));
    }

    public Id WorkspaceId { get; }
    public string WorkspaceLink { get; }
}
