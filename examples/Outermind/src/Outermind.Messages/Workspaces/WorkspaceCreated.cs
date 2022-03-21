namespace Outermind.Workspaces;

public class WorkspaceCreated : IEvent
{
    public WorkspaceCreated(Id workspaceId, string name, string link)
    {
        WorkspaceId = workspaceId ?? throw new ArgumentNullException(nameof(workspaceId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Link = link ?? throw new ArgumentNullException(nameof(link));
    }

    public Id WorkspaceId { get; }
    public string Name { get; }
    public string Link { get; }
}
