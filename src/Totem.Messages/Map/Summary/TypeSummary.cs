namespace Totem.Map.Summary;

public class TypeSummary
{
    internal TypeSummary(Id id, string @namespace, string name, string fullName, string assemblyQualifiedName)
    {
        Id = id;
        Namespace = @namespace;
        Name = name;
        FullName = fullName;
        AssemblyQualifiedName = assemblyQualifiedName;
    }

    public Id Id { get; }
    public string Namespace { get; }
    public string Name { get; }
    public string FullName { get; }
    public string AssemblyQualifiedName { get; }
}
