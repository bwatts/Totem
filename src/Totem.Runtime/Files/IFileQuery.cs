namespace Totem.Files
{
    public interface IFileQuery
    {
        string Root { get; }
        string? Prefix { get; }

        bool IncludeKey(string key);
    }
}