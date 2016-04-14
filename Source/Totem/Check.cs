using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Enables boolean queries of target values
	/// </summary>
	public static class Check
	{
		public static Check<T> True<T>(T target)
		{
			return new Check<T>(target, true);
		}

		public static Check<T> False<T>(T target)
		{
			return new Check<T>(target, false);
		}
	}

	/// <summary>
	/// A boolean query of a value of the specified type
	/// </summary>
	/// <typeparam name="T">The type of target value</typeparam>
	public sealed class Check<T>
	{
		private readonly bool _expectedResult;
		private readonly Check<T> _baseCheck;
		private readonly Func<T, bool> _next;
		private readonly bool _expectedNext;

		internal Check(T target, bool expectedResult)
		{
			Target = target;
			_expectedResult = expectedResult;
		}

		private Check(T target, bool expectedResult, Func<T, bool> next, bool expectedNext)
		{
			Target = target;
			_expectedResult = expectedResult;
			_next = next;
			_expectedNext = expectedNext;
		}

		private Check(Check<T> baseCheck, Func<T, bool> next, bool expectedNext)
		{
			Target = baseCheck.Target;
			_expectedResult = baseCheck._expectedResult;
			_baseCheck = baseCheck;
			_next = next;
			_expectedNext = expectedNext;
		}

		public T Target { get; }

		public bool Apply() => ApplyRoot() == _expectedResult;
		private bool ApplyRoot() => ApplyBase() && ApplyNext();
		private bool ApplyBase() => _baseCheck == null || _baseCheck;
		private bool ApplyNext() => _next == null || _next(Target) == _expectedNext;
		
		public static implicit operator bool?(Check<T> check) => check?.Apply();
		public static implicit operator bool(Check<T> check) => check?.Apply() ?? false;

		public Check<T> IsTrue(Func<T, bool> next)
		{
			return _next == null
				? new Check<T>(Target, _expectedResult, next, true)
				: new Check<T>(this, next, true);
		}

		public Check<T> IsFalse(Func<T, bool> next)
		{
			return _next == null
				? new Check<T>(Target, _expectedResult, next, false)
				: new Check<T>(this, next, false);
		}
	}
}