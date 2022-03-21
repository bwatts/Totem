namespace Outermind.Workspaces;

public static class TestEvents
{
    public static WorkspaceCreated WorkspaceCreated(Id? id = null, string name = "test", string link = "/test") =>
        new(id ?? Id.NewId(), name, link);

    public static MainBranchCreated MainBranchCreated(Id? workspaceId = null, Id? branchId = null, string name = "main", string link = "/main") =>
        new(workspaceId ?? Id.NewId(), branchId ?? Id.NewId(), name, link);
}
