using System;
using System.Diagnostics;

namespace Totem
{
  /// <summary>
  /// A test fixture with instance-based access to <see cref="Totem.Expect"/> methods
  /// </summary>
  public abstract class Expectations
  {
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static Expect<T> Expect<T>(T target) =>
      Totem.Expect.That(target);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static Expect<T> ExpectNot<T>(T target) =>
      Totem.Expect.Not(target);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static void ExpectThrows(Func<object> func, Text issue = null) =>
      Totem.Expect.Throws(func, issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static void ExpectThrows(Action action, Text issue = null) =>
      Totem.Expect.Throws(action, issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static void ExpectThrows<TException>(Func<object> func, Text issue = null) where TException : Exception =>
      Totem.Expect.Throws<TException>(func, issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    protected static void ExpectThrows<TException>(Action action, Text issue = null) where TException : Exception =>
      Totem.Expect.Throws<TException>(action, issue);
  }
}