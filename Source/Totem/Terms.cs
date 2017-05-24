using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// A space-separated list of descriptive text values
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class Terms : ITextable, IEquatable<Terms>, IReadOnlyList<string>
	{
		private readonly string[] _terms;

		private Terms(string[] terms)
		{
			_terms = terms;
		}

		public bool IsNone => _terms.Length == 0;
		public bool IsNotNone => _terms.Length != 0;
		public int Count => _terms.Length;
		public string this[int index] => _terms[index];

		public IEnumerator<string> GetEnumerator() => _terms.AsEnumerable().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override string ToString() => ToText();
		public Text ToText() => _terms.ToTextSeparatedBy(" ");

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as Terms);
		}

		public bool Equals(Terms other)
		{
			return !ReferenceEquals(other, null)
				&& _terms.Length == other._terms.Length
				&& !_terms.Except(other._terms).Any();
		}

		public override int GetHashCode()
		{
			return HashCode.CombineItems(_terms);
		}

		//
		// Union
		//

		public Terms Union(Terms terms)
		{
			return Union(terms._terms);
		}

		public Terms Union(IEnumerable<Terms> terms)
		{
			return Union(terms.SelectMany(subTerms => subTerms._terms));
		}

		public Terms Union(params Terms[] terms)
		{
			return Union(terms as IEnumerable<Terms>);
		}

		public Terms Union(IEnumerable<string> terms)
		{
			return new Terms(_terms.Union(terms).ToArray());
		}

		public Terms Union(params string[] terms)
		{
			return Union(terms as IEnumerable<string>);
		}

		//
		// Except
		//

		public Terms Except(Terms terms)
		{
			return Except(terms._terms);
		}

		public Terms Except(IEnumerable<Terms> terms)
		{
			return Except(terms.SelectMany(subTerms => subTerms._terms));
		}

		public Terms Except(params Terms[] terms)
		{
			return Except(terms as IEnumerable<Terms>);
		}

		public Terms Except(IEnumerable<string> terms)
		{
			return new Terms(_terms.Except(terms).ToArray());
		}

		public Terms Except(params string[] terms)
		{
			return Except(terms as IEnumerable<string>);
		}

		//
		// Operators
		//

		public static bool operator ==(Terms x, Terms y) => Eq.Op(x, y);
		public static bool operator !=(Terms x, Terms y) => Eq.OpNot(x, y);

		public static Terms operator +(Terms x, Terms y) => x.Union(y);
		public static Terms operator +(Terms x, IEnumerable<Terms> y) => x.Union(y);
		public static Terms operator +(Terms x, string y) => x.Union(y);
		public static Terms operator +(Terms x, IEnumerable<string> y) => x.Union(y);

		public static Terms operator -(Terms x, Terms y) => x.Except(y);
		public static Terms operator -(Terms x, IEnumerable<Terms> y) => x.Except(y);
		public static Terms operator -(Terms x, string y) => x.Except(y);
		public static Terms operator -(Terms x, IEnumerable<string> y) => x.Except(y);

		//
		// Factory
		//

		public static readonly Terms None = new Terms(new string[0]);

		public static string Escape(string term)
		{
			return term.Replace(' ', '+');
		}

		public static Terms From(IEnumerable<string> terms)
		{
			return new Terms(terms.Where(term => term != "").Select(Escape).ToArray());
		}

		public static Terms From(params string[] terms)
		{
			return From(terms as IEnumerable<string>);
		}

		public static Terms From(IEnumerable<Terms> terms)
		{
			return From(terms.SelectMany(subTerms => subTerms.AsEnumerable()));
		}

		public static Terms From(params Terms[] terms)
		{
			return From(terms as IEnumerable<Terms>);
		}

		public static Terms FromValue(string value)
		{
			return From(value.Split(' '));
		}

		public sealed class Converter : TypeConverter
		{
			//
			// ConvertFrom
			//

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string)
					|| sourceType == typeof(Text)
					|| typeof(IEnumerable<string>).IsAssignableFrom(sourceType)
					|| typeof(IEnumerable<Terms>).IsAssignableFrom(sourceType)
					|| base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if(value is string)
				{
					return FromValue(value as string);
				}
				else if(value is Text)
				{
					return FromValue(value.ToString());
				}
				else if(value is IEnumerable<string>)
				{
					return From(value as IEnumerable<string>);
				}
				else if(value is IEnumerable<Terms>)
				{
					return From(value as IEnumerable<Terms>);
				}
				else
				{
					return base.ConvertFrom(context, culture, value);
				}
			}

			//
			// ConvertTo
			//

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(string)
					|| destinationType == typeof(Text)
					|| destinationType == typeof(IList<string>)
					|| destinationType == typeof(List<string>)
					|| destinationType == typeof(string[])
					|| destinationType == typeof(IReadOnlyList<string>)
					|| base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				var terms = value as Terms;

				if(terms != null)
				{
					if(destinationType == typeof(string))
					{
						return terms.ToString();
					}
					else if(destinationType == typeof(Text))
					{
						return terms.ToText();
					}
					else if(destinationType == typeof(IList<string>)
						|| destinationType == typeof(List<string>)
						|| destinationType == typeof(IReadOnlyList<string>))
					{
						return terms.ToList();
					}
					else
					{
						if(destinationType == typeof(string[]))
						{
							return terms.ToArray();
						}
					}
				}

				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}