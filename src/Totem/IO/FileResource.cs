using System;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
  /// <summary>
  /// A file targeted by a link
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class FileResource : IOResource, IEquatable<FileResource>
  {
    FileResource(FolderResource folder, FileName name)
    {
      Folder = folder;
      Name = name;
    }

    public readonly FolderResource Folder;
    public readonly FileName Name;

    public override bool IsTemplate =>
      Folder.IsTemplate || Name.IsTemplate;

    public override string ToString(bool altSlash = false, bool leading = false) =>
      Folder.ToString(altSlash, leading, trailing: true).ToText() + Name;

    public FileResource RelativeTo(FolderResource folder) =>
      new FileResource(Folder.RelativeTo(folder), Name);

    public FileResource Up(int count = 1, bool strict = true) =>
      new FileResource(Folder.Up(count, strict), Name);

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as FileResource);

    public bool Equals(FileResource other) =>
      Eq.Values(this, other).Check(x => x.Folder).Check(x => x.Name);

    public override int GetHashCode() =>
      HashCode.Combine(Folder, Name);

    public static bool operator ==(FileResource x, FileResource y) => Eq.Op(x, y);
    public static bool operator !=(FileResource x, FileResource y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static FileResource From(FolderResource folder, FileName name) =>
      new FileResource(folder, name);

    public static FileResource From(FileName name) =>
      new FileResource(FolderResource.Root, name);

    public static FileResource From(FolderResource folder, string name, bool strict = true, bool extensionOptional = false)
    {
      var parsedName = FileName.From(name, strict, extensionOptional);

      return parsedName == null ? null : new FileResource(folder, parsedName);
    }

    public static FileResource From(string folder, FileName name, bool strict = true)
    {
      var parsedFolder = FolderResource.From(folder, strict);

      return parsedFolder == null ? null : new FileResource(parsedFolder, name);
    }

    public static FileResource From(string folder, string name, bool strict = true, bool extensionOptional = false)
    {
      var parsedFolder = FolderResource.From(folder, strict);

      return parsedFolder == null ? null : From(parsedFolder, name, strict, extensionOptional);
    }

    public static FileResource From(string value, bool strict = true, bool extensionOptional = false)
    {
      var parsedFolder = FolderResource.From(value, strict);

      if(parsedFolder != null)
      {
        var fileSegment = parsedFolder.Path.Segments.LastOrDefault();

        if(fileSegment != null)
        {
          var parsedName = FileName.From(fileSegment.ToString(), strict, extensionOptional);

          if(parsedName != null)
          {
            return new FileResource(parsedFolder.Up(strict: false), parsedName);
          }
        }
      }

      Expect.False(strict, "Cannot parse file resource");

      return null;
    }

    public static FileResource FromRandom() =>
      From(FileName.FromRandom());

    public new sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}