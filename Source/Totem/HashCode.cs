using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Combines the hash codes of multiple objects
	/// </summary>
	public static class HashCode
	{
		public static int Combine(params object[] source)
		{
			return CombineItems(source as IEnumerable<object>);
		}

		public static int CombineItems<T>(IEnumerable<T> items)
		{
			int hash;

			var listSource = items as IList<T>;

			if(listSource != null && listSource.Count == 1)
			{
				hash = listSource[0].GetHashCode();
			}
			else
			{
				// Based on: http://blog.roblevine.co.uk/?cat=10

				hash = 23;	// Non-zero and prime

				foreach(var item in items)
				{
					hash = ((hash << 5) * 37) ^ (item == null ? 0 : item.GetHashCode());
				}
			}

			return hash;
		}
	}
}