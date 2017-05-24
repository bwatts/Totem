using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem
{
  /// <summary>
  /// The observable value of a field in a bindable object
  /// </summary>
  public sealed class FieldValue : ITextable
  {
    internal FieldValue(Field field)
    {
      Field = field;
      Content = field.ResolveDefault();
      IsUnset = true;
      IsSet = false;
    }

    internal FieldValue(Field field, object content)
    {
      Field = field;
      Content = content;
      IsUnset = false;
      IsSet = true;
    }

    public Field Field { get; }
    public object Content { get; private set; }
    public bool IsUnset { get; private set; }
    public bool IsSet { get; private set; }

    public sealed override string ToString() => ToText();
    public Text ToText() => Text.Of(Content);

    public void Set(object content)
    {
      Content = content;

      IsUnset = content == Field.UnsetValue;
      IsSet = !IsUnset;
    }
  }
}