namespace Totem.InternalApi;

internal static class InternalApiRoot
{
    internal const string Path = "/totem";

    internal static string Combine(string path)
    {
        if(path.StartsWith('/'))
        {
            path = path[1..];
        }

        return Path + path;
    }
}
