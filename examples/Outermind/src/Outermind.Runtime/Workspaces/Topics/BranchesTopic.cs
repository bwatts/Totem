namespace Outermind.Workspaces.Topics;

public class BranchesTopic : Topic
{
    public const string MainBranchName = "main";

    public static Id Route(CreateMainBranch command) => command.WorkspaceId;

    readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase);

    protected void Given(MainBranchCreated e) =>
        _names.Add(e.Name);

    protected void When(CreateMainBranch command)
    {
        if(_names.Count > 0)
        {
            ThenError(WorkspaceErrors.MainBranchAlreadyCreated);
            return;
        }

        var branchId = Id.DeriveId(MainBranchName);
        var link = $"{command.WorkspaceLink}/branches/{MainBranchName}";

        Then(new MainBranchCreated(command.WorkspaceId, branchId, MainBranchName, link));
    }
}
