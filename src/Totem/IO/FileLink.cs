using System;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
  /// <summary>
  /// A rooted reference to a file resource
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class FileLink : IOLink, IEquatable<FileLink>
  {
    FileLink(FolderLink folder, FileName name)
    {
      Folder = folder;
      Name = name;
    }

    public readonly FolderLink Folder;
    public readonly FileName Name;

    public override bool IsTemplate =>
      Folder.IsTemplate || Name.IsTemplate;

    public override string ToString(bool altSlash = false) =>
      Folder.ToString(altSlash, trailing: true).ToText() + Name;

    public FileResource RelativeTo(FolderLink folder) =>
      FileResource.From(Folder.RelativeTo(folder), Name);

    public bool TryUp(out FileLink up, int count = 1)
    {
      up = Folder.TryUp(out var folderUp, count) ? new FileLink(folderUp, Name) : null;

      return up != null;
    }

    public FileLink Up(int count = 1) =>
      new FileLink(Folder.Up(count), Name);

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as FileLink);

    public bool Equals(FileLink other) =>
      Eq.Values(this, other).Check(x => x.Folder).Check(x => x.Name);

    public override int GetHashCode() =>
      HashCode.Combine(Folder, Name);

    public static bool operator ==(FileLink x, FileLink y) => Eq.Op(x, y);
    public static bool operator !=(FileLink x, FileLink y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static bool TryFrom(FolderLink folder, string file, out FileLink link)
    {
      link = FileResource.TryFrom(file, out var parsedFile) ? From(folder, parsedFile) : null;

      return link != null;
    }

    public static bool TryFrom(string folder, FileResource file, out FileLink link)
    {
      link = FolderLink.TryFrom(folder, out var parsedFolder) ? From(parsedFolder, file) : null;

      return link != null;
    }

    public static bool TryFrom(string folder, string file, out FileLink link)
    {
      link = null;

      if(FolderLink.TryFrom(folder, out var parsedFolder))
      {
        if(FileName.TryFrom(file, out var parsedFile))
        {
          link = new FileLink(parsedFolder, parsedFile);
        }
      }

      return link != null;
    }

    public static bool TryFrom(string value, out FileLink link, bool extensionOptional = false)
    {
      link = null;

      if(FolderLink.TryFrom(value, out var parsedFolder))
      {
        var file = parsedFolder.Resource.Path.Segments.LastOrDefault()?.ToString();

        if(file != null)
        {
          if(FileName.TryFrom(file, out var parsedFile, extensionOptional))
          {
            if(!parsedFolder.TryUp(out var folderUp))
            {
              folderUp = parsedFolder;
            }

            link = new FileLink(folderUp, parsedFile);
          }
        }
      }

      return link != null;
    }

    public static FileLink From(FolderLink folder, FileResource file) =>
      new FileLink(folder.Then(file.Folder), file.Name);

    public static FileLink From(FolderLink folder, string file)
    {
      if(!TryFrom(folder, file, out var link))
      {
        throw new FormatException($"Failed to parse file: {file}");
      }

      return link;
    }

    public static FileLink From(string folder, FileResource file)
    {
      if(!TryFrom(folder, file, out var link))
      {
        throw new FormatException($"Failed to parse folder: {folder}");
      }

      return link;
    }

    public static FileLink From(string folder, string file)
    {
      if(!TryFrom(folder, file, out var link))
      {
        throw new FormatException($"Failed to parse folder or file: {folder} {file}");
      }

      return link;
    }

    public new static FileLink From(string value, bool extensionOptional = false)
    {
      if(!TryFrom(value, out var link, extensionOptional))
      {
        throw new FormatException($"Failed to parse file: {value}");
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