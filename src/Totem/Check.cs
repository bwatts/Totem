using System;

namespace Totem
{
  /// <summary>
  /// Enables queries with a boolean result
  /// </summary>
  public static class Check
  {
    public static Check<T> That<T>(T target) =>
      new Check<T>(target, true);

    public static Check<T> Not<T>(T target) =>
      new Check<T>(target, false);
  }

  /// <summary>
  /// A boolean query of a value of the specified type
  /// </summary>
  /// <typeparam name="T">The type of queried value</typeparam>
  public sealed class Check<T>
  {
    readonly bool _expectedResult;
    readonly Check<T> _baseCheck;
    readonly Func<T, bool> _next;
    readonly bool _expectedNext;

    internal Check(T target, bool expectedResult)
    {
      Target = target;
      _expectedResult = expectedResult;
    }

    Check(T target, bool expectedResult, Func<T, bool> next, bool expectedNext)
    {
      Target = target;
      _expectedResult = expectedResult;
      _next = next;
      _expectedNext = expectedNext;
    }

    Check(Check<T> baseCheck, Func<T, bool> next, bool expectedNext)
    {
      Target = baseCheck.Target;
      _expectedResult = baseCheck._expectedResult;
      _baseCheck = baseCheck;
      _next = next;
      _expectedNext = expectedNext;
    }

    public T Target { get; }

    public bool Apply() => ApplyRoot() == _expectedResult;

    bool ApplyRoot() => ApplyBase() && ApplyNext();
    bool ApplyBase() => _baseCheck == null || _baseCheck;
    bool ApplyNext() => _next == null || _next(Target) == _expectedNext;
    
    public static implicit operator bool?(Check<T> check) => check?.Apply();
    public static implicit operator bool(Check<T> check) => check?.Apply() ?? false;

    public Check<T> IsTrue(Func<T, bool> next) =>
      _next == null
        ? new Check<T>(Target, _expectedResult, next, true)
        : new Check<T>(this, next, true);

    public Check<T> IsFalse(Func<T, bool> next) =>
      _next == null
        ? new Check<T>(Target, _expectedResult, next, false)
        : new Check<T>(this, next, false);
  }
}