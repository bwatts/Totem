namespace Outermind.Workspaces;

public static class WorkspaceErrors
{
    public static readonly ErrorInfo BranchNameInUse = new(nameof(BranchNameInUse), ErrorLevel.Conflict);
    public static readonly ErrorInfo MainBranchAlreadyCreated = new(nameof(MainBranchAlreadyCreated), ErrorLevel.Conflict);
    public static readonly ErrorInfo MainBranchNotCreated = new(nameof(MainBranchNotCreated), ErrorLevel.Conflict);
    public static readonly ErrorInfo NameInUse = new(nameof(NameInUse), ErrorLevel.Conflict);
}
