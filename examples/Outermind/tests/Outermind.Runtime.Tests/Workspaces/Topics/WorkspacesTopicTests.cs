using System.Net;

namespace Outermind.Workspaces.Topics;

public class WorkspacesTopicTests : TopicTests<WorkspacesTopic>
{
    readonly string _name = "test";

    [Fact]
    public void CreateWorkspace_causes_WorkspaceCreated_over_HTTP()
    {
        var context = HttpContext.CallWhen(new CreateWorkspace(_name));

        var e = ExpectEvent<WorkspaceCreated>();

        e.WorkspaceId.Should().Be(TopicType.SingleInstanceId!.DeriveId(_name));
        e.Name.Should().Be(_name);

        context.ResponseCode.Should().Be(HttpStatusCode.Created);
        context.ResponseHeaders["Location"].Should().Be(e.Link);
    }

    [Fact]
    public void CreateWorkspace_causes_WorkspaceCreated_locally()
    {
        LocalContext.CallWhen(new CreateWorkspace(_name));

        var e = ExpectEvent<WorkspaceCreated>();

        e.WorkspaceId.Should().Be(TopicType.SingleInstanceId!.DeriveId(_name));
        e.Name.Should().Be(_name);
    }
}
