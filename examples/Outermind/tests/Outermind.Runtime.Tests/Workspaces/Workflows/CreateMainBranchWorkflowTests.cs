namespace Outermind.Workspaces.Workflows;

public class CreateMainBranchWorkflowTests : WorkflowTests<CreateMainBranchWorkflow>
{
    [Fact]
    public void WorkspaceCreated_routes_to_workspace()
    {
        var e = TestEvents.WorkspaceCreated();

        ExpectRoutesTo(e.WorkspaceId, e);
    }

    [Fact]
    public void WorkspaceCreated_causes_CreateMainBranch()
    {
        var e = TestEvents.WorkspaceCreated();

        CallWhen(e);

        var command = ExpectCommand<CreateMainBranch>();

        command.WorkspaceId.Should().Be(e.WorkspaceId);
    }
}
