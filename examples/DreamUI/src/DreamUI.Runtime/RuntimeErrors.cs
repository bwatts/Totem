using Totem;

namespace DreamUI
{
    public static class RuntimeErrors
    {
        public static readonly ErrorInfo InstallationNotFound = new(nameof(InstallationNotFound), ErrorLevel.NotFound);
    }
}