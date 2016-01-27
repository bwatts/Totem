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
		public static Check<T> That<T>(T target)
		{
			return new Check<T>(target, t => true);
		}
	}

	/// <summary>
	/// A boolean query of a value of the specified type
	/// </summary>
	/// <typeparam name="T">The type of target value</typeparam>
	public sealed class Check<T>
	{
		private readonly Func<T, bool> _apply;

		internal Check(T target, Func<T, bool> apply)
		{
			Target = target;
			_apply = apply;
		}

		public T Target { get; }

		public bool Apply()
		{
			return _apply(Target);
		}

		public static implicit operator bool(Check<T> check)
		{
			return check?.Apply() ?? false;
		}

		public Check<T> That(Func<T, bool> next)
		{
			return new Check<T>(Target, t => Apply() && next(t));
		}
	}
}