using System;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
  /// <summary>
  /// A rooted reference to a folder resource
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class FolderLink : IOLink, IEquatable<FolderLink>
  {
    FolderLink(LinkText root, FolderResource resource, bool isUnc = false)
    {
      Root = root;
      Resource = resource;
      IsUnc = isUnc;
    }

    public readonly LinkText Root;
    public readonly FolderResource Resource;
    public readonly bool IsUnc;

    public override bool IsTemplate =>
      Root.IsTemplate || Resource.IsTemplate;

    public override string ToString(bool altSlash = false) =>
      ToString();

    public string ToString(bool altSlash = false, bool trailing = false) =>
      Root.ToString() + Resource.ToString(altSlash, leading: true, trailing: trailing);

    public FolderResource RelativeTo(FolderLink other) =>
      Root != other.Root ? FolderResource.Root : Resource.RelativeTo(other.Resource);

    public bool TryUp(out FolderLink up, int count = 1)
    {
      up = Resource.TryUp(out var resourceUp, count) ? new FolderLink(Root, resourceUp, IsUnc) : null;

      return up != null;
    }

    public FolderLink Up(int count = 1) =>
      new FolderLink(Root, Resource.Up(count));

    public FolderLink Then(FolderResource folder) =>
      new FolderLink(Root, Resource.Then(folder));

    public FileLink Then(FileResource file) =>
      FileLink.From(this, file);

    public FileLink Then(FileName file) =>
      FileLink.From(this, FileResource.From(file));

    public FolderLink Then(Func<FolderResource, FolderResource> folderFromRoot) =>
      Then(folderFromRoot(FolderResource.Root));

    public FileLink Then(Func<FolderResource, FileResource> fileFromRoot) =>
      Then(fileFromRoot(FolderResource.Root));

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as FolderLink);

    public bool Equals(FolderLink other) =>
      Eq.Values(this, other).Check(x => x.Root).Check(x => x.Resource);

    public override int GetHashCode() =>
      HashCode.Combine(Root, Resource);

    public static bool operator ==(FolderLink x, FolderLink y) => Eq.Op(x, y);
    public static bool operator !=(FolderLink x, FolderLink y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public const string UncPrefix = @"\\";

    public static bool HasUncPrefix(LinkText root) =>
      root.ToString().StartsWith(UncPrefix);

    public static bool TryFrom(LinkText root, string resource, out FolderLink link)
    {
      link = From(root, FolderResource.From(resource));

      return true;
    }

    public static bool TryFrom(string value, out FolderLink link)
    {
      if(TryFromLocal(value, out var localLink))
      {
        link = localLink;
      }
      else if(TryFromUnc(value, out var uncLink))
      {
        link = uncLink;
      }
      else
      {
        link = null;
      }

      return link != null;
    }

    public static bool TryFromUnc(string value, out FolderLink link)
    {
      link = null;

      if(!String.IsNullOrEmpty(value) && HasUncPrefix(value))
      {
        var path = FolderResource.From(value.Substring(2)).Path;

        var root = $@"\\{path.Segments[0]}";

        var resource = path.Segments.Count == 1
          ? LinkPath.Root
          : LinkPath.From(path.Segments.Skip(1));

        link = new FolderLink(root, FolderResource.From(resource), isUnc: true);
      }

      return link != null;
    }

    public static bool TryFromLocal(string value, out FolderLink link)
    {
      link = null;

      var path = FolderResource.From(value).Path;

      if(path.Segments.Any())
      {
        var root = path.Segments[0];

        var resource = path.Segments.Count == 1
          ? LinkPath.Root
          : LinkPath.From(path.Segments.Skip(1));

        link = new FolderLink(root, FolderResource.From(resource));
      }

      return link != null;
    }

    public static FolderLink From(LinkText root, FolderResource resource) =>
      new FolderLink(root, resource);

    public static FolderLink From(LinkText root, string resource)
    {
      if(!TryFrom(root, resource, out var link))
      {
        throw new FormatException($"Failed to parse folder resource: {resource}");
      }

      return link;
    }

    public static FolderLink From(string value)
    {
      if(!TryFrom(value, out var link))
      {
        throw new FormatException($"Failed to parse folder link: {value}");
      }

      return link;
    }

    public static FolderLink FromUnc(string value)
    {
      if(!TryFromUnc(value, out var link))
      {
        throw new FormatException($"Failed to parse UNC folder link: {value}");
      }

      return link;
    }

    public static FolderLink FromLocal(string value)
    {
      if(!TryFromLocal(value, out var link))
      {
        throw new FormatException($"Failed to parse local folder link: {value}");
      }

      return link;
    }

    public new sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}