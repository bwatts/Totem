using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="System.Text"/> with core operations
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TextOperations
	{
		//
		// Write
		//

		public static Text Write(this Text text, bool value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, char[] value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, char[] value, int index, int count)
		{
			return text + Text.Of(value, index, count);
		}

		public static Text Write(this Text text, decimal value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, double value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, float value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, int value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, long value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, object value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, uint value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, ulong value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, string format, object arg0)
		{
			return text + Text.Of(format, arg0);
		}

		public static Text Write(this Text text, string format, object arg0, object arg1)
		{
			return text + Text.Of(format, arg0, arg1);
		}

		public static Text Write(this Text text, string format, object arg0, object arg1, object arg2)
		{
			return text + Text.Of(format, arg0, arg1, arg2);
		}

		public static Text Write(this Text text, string format, params object[] args)
		{
			return text + Text.Of(format, args);
		}

		//
		// WriteMany
		//

		public static Text WriteMany<T>(this Text text, IEnumerable<T> values, Func<T, Text> selectText)
		{
			return text + Text.OfMany(values, selectText);
		}

		public static Text WriteMany<T>(this Text text, IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return text + Text.OfMany(values, selectText);
		}

		public static Text WriteMany<T>(this Text text, IEnumerable<T> values)
		{
			return text + Text.OfMany(values);
		}

		public static Text WriteMany<T>(this Text text, params T[] values)
		{
			return text + Text.OfMany(values);
		}

		//
		// WriteLine
		//

		public static Text WriteLine(this Text text)
		{
			return text.Write(writer => writer.WriteLine());
		}

		public static Text WriteLine(this Text text, bool value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, char value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, char[] value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, char[] value, int index, int count)
		{
			return text.Write(writer => writer.WriteLine(value, index, count));
		}

		public static Text WriteLine(this Text text, decimal value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, double value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, float value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, int value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, long value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, object value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, string value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, uint value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, ulong value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, string format, object arg0)
		{
			return text.Write(writer => writer.WriteLine(format, arg0));
		}

		public static Text WriteLine(this Text text, string format, object arg0, object arg1)
		{
			return text.Write(writer => writer.WriteLine(format, arg0, arg1));
		}

		public static Text WriteLine(this Text text, string format, object arg0, object arg1, object arg2)
		{
			return text.Write(writer => writer.WriteLine(format, arg0, arg1, arg2));
		}

		public static Text WriteLine(this Text text, string format, params object[] args)
		{
			return text.Write(writer => writer.WriteLine(format, args));
		}

		//
		// WriteLines
		//

		public static Text WriteLines(this Text text, int count)
		{
			return text + Text.Line.Repeat(count);
		}

		public static Text WriteTwoLines(this Text text)
		{
			return text + Text.TwoLines;
		}

		public static Text WriteLines(this Text text, IEnumerable<Text> values, Func<Text, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines(this Text text, IEnumerable<Text> values, Func<Text, int, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines(this Text text, IEnumerable<Text> values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		public static Text WriteLines(this Text text, params Text[] values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		public static Text WriteLines<T>(this Text text, IEnumerable<T> values, Func<T, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines<T>(this Text text, IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines<T>(this Text text, IEnumerable<T> values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		public static Text WriteLines<T>(this Text text, params T[] values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		//
		// WriteSeparatedBy
		//

		public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values, Func<Text, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values, Func<Text, int, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		public static Text WriteSeparatedBy(this Text text, Text separator, params Text[] values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values, Func<T, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, params T[] values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		//
		// ToText
		//

		public static Text ToText(this string value)
		{
			return value;
		}

		public static Text ToText(this IEnumerable<Text> values, Func<Text, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText(this IEnumerable<Text> values, Func<Text, int, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText(this IEnumerable<Text> values)
		{
			return Text.OfMany(values);
		}

		public static Text ToText<T>(this IEnumerable<T> values, Func<T, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText<T>(this IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText<T>(this IEnumerable<T> values)
		{
			return Text.OfMany(values);
		}

		//
		// ToTextSeparatedBy
		//

		public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator, Func<Text, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator, Func<Text, int, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator)
		{
			return Text.SeparatedBy(separator, values);
		}

		public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator, Func<T, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator, Func<T, int, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator)
		{
			return Text.SeparatedBy(separator, values);
		}

		//
		// Conditions
		//

		public static Text WriteIf(this Text text, bool condition, Text whenTrue, Text whenFalse)
		{
			return text + Text.If(condition, whenTrue, whenFalse);
		}

		public static Text WriteIf(this Text text, bool condition, Text whenTrue)
		{
			return text + Text.If(condition, whenTrue);
		}

		public static Text WriteIf(this Text text, Func<bool> condition, Text whenTrue, Text whenFalse)
		{
			return text + Text.If(condition, whenTrue, whenFalse);
		}

		public static Text WriteIf(this Text text, Func<bool> condition, Text whenTrue)
		{
			return text + Text.If(condition, whenTrue);
		}
	}
}