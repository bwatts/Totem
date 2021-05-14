using Totem;

namespace Dream
{
    public static class RuntimeErrors
    {
        public static readonly ErrorInfo InvalidZipUrlExtension = new(nameof(InvalidZipUrlExtension));
        public static readonly ErrorInfo VersionNotFound = new(nameof(VersionNotFound), ErrorLevel.NotFound);
    }
}