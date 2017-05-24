using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem
{
  /// <summary>
	/// A type with a text representation whose discovery experience excludes definitions from <see cref="System.Object"/>
	/// </summary>
  public abstract class Clean : ITextable
  {
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public override string ToString() => ToText();

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public virtual Text ToText() => base.ToString();

    public static explicit operator string(Clean clean) => clean?.ToString() ?? "";

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static Check<T> Check<T>(T target)
    {
      return Totem.Check.True(target);
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static Check<T> CheckNot<T>(T target)
    {
      return Totem.Check.False(target);
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static Expect<T> Expect<T>(T target)
    {
      return Totem.Expect.True(target);
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static Expect<T> ExpectNot<T>(T target)
    {
      return Totem.Expect.False(target);
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Expect(bool result)
    {
      Totem.Expect.True(result);
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Expect(bool result, Text issue)
    {
      Totem.Expect.True(result, issue);
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void ExpectNot(bool result)
    {
      Totem.Expect.False(result);
    }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void ExpectNot(bool result, Text issue)
    {
      Totem.Expect.False(result, issue);
    }
  }
}