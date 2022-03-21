namespace Outermind.Workspaces.Topics;

public class BranchesTopic : Topic
{
    public static readonly Id IdNamespace = (Id) "8955d5b6-c8a4-4dfd-9034-aae0361ccd2e";
    public const string MainBranchName = "main";

    public static Id Route(CreateMainBranch command) => command.WorkspaceId;

    readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase);

    protected void When(CreateMainBranch command)
    {
        if(_names.Count > 0)
        {
            ThenError(WorkspaceErrors.MainBranchAlreadyCreated);
            return;
        }

        var branchId = IdNamespace.DeriveId(MainBranchName);
        var link = $"{command.WorkspaceLink}/branches/{MainBranchName}";

        Then(new MainBranchCreated(command.WorkspaceId, branchId, MainBranchName, link));
    }

    protected void Given(MainBranchCreated e) =>
        _names.Add(e.Name);
}
