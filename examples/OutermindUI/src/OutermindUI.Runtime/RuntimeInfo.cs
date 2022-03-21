using System.Reflection;

namespace OutermindUI;

public static class RuntimeInfo
{
    public static Assembly Assembly => typeof(RuntimeInfo).Assembly;
}
