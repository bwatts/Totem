using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Describes a value with expected characteristics
	/// </summary>
	/// <typeparam name="T">The type of value with expected characteristics</typeparam>
	public interface IExpect<T> : IFluent
	{
		IExpect<T> IsTrue(Func<T, bool> check, Text expected, Text because = null, Func<T, Text> actual = null);

		IExpect<T> IsFalse(Func<T, bool> check, Text expected, Text because = null, Func<T, Text> actual = null);
	}
}