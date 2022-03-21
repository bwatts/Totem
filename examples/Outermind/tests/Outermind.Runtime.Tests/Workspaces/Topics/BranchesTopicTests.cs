namespace Outermind.Workspaces.Topics;

public class BranchesTopicTests : TopicTests<BranchesTopic>
{
    readonly Id _workspaceId = Id.NewId();
    readonly string _workspaceLink = "/...";

    [Fact]
    public void CreateMainBranch_routes_to_workspace() =>
        ExpectRoutesTo(_workspaceId, new CreateMainBranch(_workspaceId, _workspaceLink));

    [Fact]
    public void CreateMainBranch_causes_MainBranchCreated()
    {
        QueueContext.CallWhen(new CreateMainBranch(_workspaceId, _workspaceLink));

        var e = ExpectEvent<MainBranchCreated>();

        e.WorkspaceId.Should().Be(_workspaceId);
        e.BranchId.Should().Be(BranchesTopic.IdNamespace.DeriveId(BranchesTopic.MainBranchName));
        e.Name.Should().Be(BranchesTopic.MainBranchName);
    }

    [Fact]
    public void CreateMainBranch_twice_causes_error()
    {
        var command = new CreateMainBranch(_workspaceId, _workspaceLink);

        QueueContext.CallWhen(command);
        QueueContext.CallWhen(command);

        ExpectError(WorkspaceErrors.MainBranchAlreadyCreated);
    }
}
