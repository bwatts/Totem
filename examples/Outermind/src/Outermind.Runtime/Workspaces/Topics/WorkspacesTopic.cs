namespace Outermind.Workspaces.Topics;

public class WorkspacesTopic : Topic
{
    readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase);

    protected void Given(WorkspaceCreated e) =>
        _names.Add(e.Name);

    protected void When(ILocalCommandContext<CreateWorkspace> context) =>
        ThenCreateIfNewName(context);

    protected void When(IHttpCommandContext<CreateWorkspace> context)
    {
        var link = ThenCreateIfNewName(context);

        if(!HasErrors)
        {
            context.RespondCreated(link);
        }
    }

    string ThenCreateIfNewName(ICommandContext<CreateWorkspace> context)
    {
        var name = context.Command.Name;

        if(_names.Contains(name))
        {
            ThenError(WorkspaceErrors.NameInUse);
            return "";
        }

        var workspaceId = Id.DeriveId(name);
        var link = $"/workspaces/{Uri.EscapeDataString(name)}";

        Then(new WorkspaceCreated(workspaceId, name, link));

        return link;
    }
}
