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

    public FolderLink Up(int count = 1, bool strict = true) =>
      new FolderLink(Root, Resource.Up(count, strict));

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

    public static FolderLink From(LinkText root, FolderResource resource) =>
      new FolderLink(root, resource);

    public static FolderLink From(LinkText root, string resource, bool strict = true)
    {
      var parsedResource = FolderResource.From(resource, strict);

      return parsedResource == null ? null : From(root, resource);
    }

    public static FolderLink From(string value, bool strict = true)
    {
      var folder = FromUnc(value, strict: false) ?? FromLocal(value, strict: false);

      Expect.False(strict && folder == null, "Cannot parse folder link");

      return folder;
    }

    public static FolderLink FromUnc(string value, bool strict = true)
    {
      if(!String.IsNullOrEmpty(value) && value.StartsWith(@"\\"))
      {
        var parsedFolder = FolderResource.From(value.Substring(2), strict);

        if(parsedFolder != null)
        {
          var root = @"\\" + parsedFolder.Path.Segments[0].ToString();

          var path = parsedFolder.Path.Segments.Count == 1
            ? LinkPath.Root
            : LinkPath.From(parsedFolder.Path.Segments.Skip(1));

          return new FolderLink(root, FolderResource.From(path), isUnc: true);
        }
      }

      Expect.False(strict, "Cannot parse UNC link: " + value);

      return null;
    }

    public static FolderLink FromLocal(string value, bool strict = true)
    {
      var parsedFolder = FolderResource.From(value, strict);

      if(parsedFolder != null && parsedFolder.Path.Segments.Count > 0)
      {
        var path = parsedFolder.Path.Segments.Count == 1
          ? LinkPath.Root
          : LinkPath.From(parsedFolder.Path.Segments.Skip(1));

        return new FolderLink(parsedFolder.Path.Segments[0], FolderResource.From(path));
      }

      Expect.True(strict, "Cannot parse folder link: " + value);

      return null;
    }

    public new sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}