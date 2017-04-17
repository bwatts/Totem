using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
  /// <summary>
  /// A change in the value of a field in a bindable object
  /// </summary>
  public class FieldChange
  {
    public FieldChange(IBindable binding, Field field, object oldContent, object newContent)
    {
      Binding = binding;
      Field = field;
      OldContent = oldContent;
      NewContent = newContent;
    }

    public readonly IBindable Binding;
    public readonly Field Field;
    public readonly object OldContent;
    public readonly object NewContent;

    public override string ToString() => $"{Field.FullName}: {OldContent} => {NewContent}";
  }
}