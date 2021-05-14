using System.Reflection;

namespace Dream
{
    public static class RuntimeInfo
    {
        public static Assembly Assembly => typeof(RuntimeInfo).Assembly;
    }
}