using System.Reflection;

namespace Totem.Map;

public class ReportRowProperty
{
    internal ReportRowProperty(PropertyInfo info) =>
        Info = info;

    public PropertyInfo Info { get; }
    public string Name => Info.Name;
    public Type ValueType => Info.PropertyType;

    public override string ToString() =>
        Info.ToString() ?? "";
}
