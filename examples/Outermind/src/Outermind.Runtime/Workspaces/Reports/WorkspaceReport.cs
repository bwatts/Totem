namespace Outermind.Workspaces.Reports;

public class WorkspaceReport : Report<WorkspaceRow>
{
    public static Id Route(WorkspaceCreated e) => e.WorkspaceId;
    public static Id Route(MainBranchCreated e) => e.WorkspaceId;

    protected void When(WorkspaceCreated e)
    {
        Row.Name = e.Name;
        Row.Link = e.Link;
    }

    protected void When(MainBranchCreated e) =>
        Row.Branches.Add(new()
        {
            Id = e.BranchId,
            Name = e.Name,
            Link = e.Link
        });
}
