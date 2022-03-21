namespace Outermind.Workspaces;

public class CreateMainBranchWorkflow : Workflow
{
    public static Id Route(WorkspaceCreated e) => e.WorkspaceId;

    protected void When(WorkspaceCreated e) =>
        ThenEnqueue(new CreateMainBranch(e.WorkspaceId, e.Link));
}
