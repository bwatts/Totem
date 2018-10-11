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

    public bool TryUp(out FileResource up, int count = 1)
    {
      up = Folder.TryUp(out var folderUp, count) ? new FileResource(folderUp, Name) : null;

      return up != null;
    }

    public FileResource Up(int count = 1) =>
      new FileResource(Folder.Up(count), Name);

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

    public static bool TryFrom(FolderResource folder, string name, out FileResource resource, bool extensionOptional = false)
    {
      resource = FileName.TryFrom(name, out var parsedName, extensionOptional) ? new FileResource(folder, parsedName) : null;

      return resource != null;
    }

    public static bool TryFrom(string folder, string name, out FileResource resource, bool extensionOptional = false)
    {
      resource = FileName.TryFrom(name, out var parsedName, extensionOptional)
        ? new FileResource(FolderResource.From(folder), parsedName)
        : null;

      return resource != null;
    }

    public static bool TryFrom(string value, out FileResource resource, bool extensionOptional = false)
    {
      resource = null;

      var parsedFolder = FolderResource.From(value);

      var file = parsedFolder.Path.Segments.LastOrDefault()?.ToString();

      if(file != null)
      {
        if(FileName.TryFrom(file, out var parsedFile, extensionOptional))
        {
          if(!parsedFolder.TryUp(out var folderUp))
          {
            folderUp = parsedFolder;
          }

          resource = new FileResource(folderUp, parsedFile);
        }
      }

      return resource != null;
    }

    public static FileResource From(FolderResource folder, FileName name) =>
      new FileResource(folder, name);

    public static FileResource From(FileName name) =>
      new FileResource(FolderResource.Root, name);

    public static FileResource From(string folder, FileName name) =>
      From(FolderResource.From(folder), name);

    public static FileResource From(FolderResource folder, string name, bool extensionOptional = false)
    {
      if(!TryFrom(folder, name, out var resource, extensionOptional))
      {
        throw new FormatException($"Failed to parse file name: {name}");
      }

      return resource;
    }

    public static FileResource From(string folder, string name, bool extensionOptional = false)
    {
      if(!TryFrom(folder, name, out var resource))
      {
        throw new FormatException($"Failed to parse file resource: {folder} {name}");
      }

      return resource;
    }

    public static FileResource From(string value, bool extensionOptional = false)
    {
      if(!TryFrom(value, out var resource))
      {
        throw new FormatException($"Failed to parse file resource: {value}");
      }

      return resource;
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