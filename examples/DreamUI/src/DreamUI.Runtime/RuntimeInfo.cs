using System.Reflection;

namespace DreamUI;

public static class RuntimeInfo
{
    public static Assembly Assembly => typeof(RuntimeInfo).Assembly;
}
