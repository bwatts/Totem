using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// A folder targeted by a link
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class FolderResource : IOResource, IEquatable<FolderResource>
  {
    public static readonly FolderResource Root = new FolderResource(LinkPath.Root);

    FolderResource(LinkPath path)
    {
      Path = path;
    }

    public readonly LinkPath Path;

    public override bool IsTemplate =>
      Path.IsTemplate;

    public override string ToString(bool altSlash = false, bool leading = false) =>
      ToString();

    public string ToString(bool altSlash = false, bool leading = false, bool trailing = false) =>
      Path.ToString(altSlash ? _separators[0] : _separators[1], leading, trailing);

    public FolderResource RelativeTo(FolderResource other) =>
      new FolderResource(Path.RelativeTo(other.Path));

    public bool TryUp(out FolderResource up, int count = 1)
    {
      up = Path.TryUp(out var pathUp, count) ? new FolderResource(pathUp) : null;

      return up != null;
    }

    public FolderResource Up(int count = 1) =>
      new FolderResource(Path.Up(count));

    public FolderResource Then(FolderResource folder) =>
      new FolderResource(Path.Then(folder.Path));

    public FileResource Then(FileResource file) =>
      FileResource.From(Then(file.Folder), file.Name);

    public FileResource Then(FileName file) =>
      FileResource.From(this, file);

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as FolderResource);

    public bool Equals(FolderResource other) =>
      Eq.Values(this, other).Check(x => x.Path);

    public override int GetHashCode() =>
      Path.GetHashCode();

    public static bool operator ==(FolderResource x, FolderResource y) => Eq.Op(x, y);
    public static bool operator !=(FolderResource x, FolderResource y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static FolderResource From(LinkPath path) =>
      new FolderResource(path);

    public new static FolderResource From(string value) =>
      new FolderResource(LinkPath.From(value, _separators));

    public static FolderResource FromRandom() =>
      From(System.IO.Path.GetRandomFileName());

    static readonly string[] _separators = new string[]
    {
      System.IO.Path.AltDirectorySeparatorChar.ToString(),
      System.IO.Path.DirectorySeparatorChar.ToString()
    };

    public new sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}