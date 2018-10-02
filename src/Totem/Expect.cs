using System;
using System.Diagnostics;

namespace Totem
{
  /// <summary>
  /// Creates instances of <see cref="Expect{T}"/> for various scenarios
  /// </summary>
  public static class Expect
  {
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static Expect<T> That<T>(T target) =>
      new Expect<T>(target, true);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static Expect<T> Not<T>(T target) =>
      new Expect<T>(target, false);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void True(bool result, Text issue = null) =>
      That(result).IsTrue(issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void False(bool result, Text issue = null) =>
      That(result).IsFalse(issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Throws(Func<object> func, Text issue = null) =>
      Throws<Exception>(func, issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Throws<TException>(Func<object> func, Text issue = null) where TException : Exception =>
      Throws<TException>(() => { func(); }, issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Throws(Action action, Text issue = null) =>
      Throws<Exception>(action, issue);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Throws<TException>(Action action, Text issue = null) where TException : Exception
    {
      try
      {
        action();
      }
      catch(TException)
      {
        return;
      }
      catch(Exception error)
      {
        var message = (issue ?? "Unexpected exception type")
          .WriteTwoLines()
          .WriteLine("Expected:")
          .WriteLine(Text.OfType<TException>().Indent(retainWhitespace: true))
          .WriteLine()
          .WriteLine("Received:")
          .WriteLine(Text.OfType(error).Indent(retainWhitespace: true));

        throw new ExpectException(message, error);
      }

      var notThrownMessage = (issue ?? "Expected exception was not thrown")
        .WriteTwoLines()
        .WriteLine("Expected:")
        .WriteLine(Text.OfType<TException>().Indent(retainWhitespace: true))
        .WriteLine()
        .WriteLine("Received:")
        .WriteLine(Text.Of("None").Indent(retainWhitespace: true));

      throw new ExpectException(notThrownMessage);
    }
  }

  /// <summary>
  /// An assertable value of the specified type
  /// </summary>
  /// <typeparam name="T">The type of target value</typeparam>
  public sealed class Expect<T>
  {
    readonly bool _expectedResult;

    internal Expect(T target, bool expectedResult)
    {
      Target = target;
      _expectedResult = expectedResult;
    }

    public T Target { get; }

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public Expect<T> IsTrue(Func<T, bool> check, Func<T, string> message) =>
      Is(true, check, message);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public Expect<T> IsFalse(Func<T, bool> check, Func<T, string> message) =>
      Is(false, check, message);

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    Expect<T> Is(bool expectedResult, Func<T, bool> check, Func<T, string> message)
    {
      bool result;

      try
      {
        result = check(Target);
      }
      catch(Exception error)
      {
        throw new ExpectException(message(Target), error);
      }

      if(result != expectedResult)
      {
        throw new ExpectException(message(Target));
      }

      return this;
    }
  }
}