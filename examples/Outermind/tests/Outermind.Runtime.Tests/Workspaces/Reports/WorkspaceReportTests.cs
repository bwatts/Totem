namespace Outermind.Workspaces.Reports;

public class WorkspaceReportTests : ReportTests<WorkspaceReport, WorkspaceRow>
{
    [Fact]
    public void WorkspaceCreated_routes_to_workspace()
    {
        var e = TestEvents.WorkspaceCreated();

        ExpectRoutesTo(e.WorkspaceId, e);
    }

    [Fact]
    public void MainBranchCreated_routes_to_workspace()
    {
        var e = TestEvents.MainBranchCreated();

        ExpectRoutesTo(e.WorkspaceId, e);
    }

    [Fact]
    public void WorkspaceCreated_sets_name_and_link()
    {
        var e = TestEvents.WorkspaceCreated();

        CallWhen(e);

        Row.Id.Should().Be(e.WorkspaceId);
        Row.Name.Should().Be(e.Name);
        Row.Link.Should().Be(e.Link);
        Row.Branches.Should().BeEmpty();
    }

    [Fact]
    public void MainBranchCreated_adds_branch()
    {
        var e = TestEvents.MainBranchCreated();

        CallWhen(e);

        Row.Branches.Should().HaveCount(1);

        var branch = Row.Branches[0];
        branch.Id.Should().Be(e.BranchId);
        branch.Name.Should().Be(e.Name);
        branch.Link.Should().Be(e.Link);
    }
}
