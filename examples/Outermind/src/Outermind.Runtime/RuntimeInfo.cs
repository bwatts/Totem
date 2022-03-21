using System.Reflection;

namespace Outermind;

public static class RuntimeInfo
{
    public static Assembly Assembly => typeof(RuntimeInfo).Assembly;
}
