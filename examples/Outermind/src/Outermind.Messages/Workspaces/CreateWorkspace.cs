namespace Outermind.Workspaces;

[HttpPostRequest("/workspaces")]
public class CreateWorkspace : IHttpCommand, ILocalCommand
{
    public CreateWorkspace(string name) =>
        Name = name ?? throw new ArgumentNullException(nameof(name));

    public string Name { get; }
}
