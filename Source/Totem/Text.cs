using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Totem
{
	/// <summary>
	/// Content in a write to <see cref="System.IO.TextWriter"/>
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class Text : IWritable
	{
		private readonly bool _isNone;
		private readonly Text _before;
		private readonly char? _charValue;
		private readonly string _stringValue;
		private readonly IWritable _writableValue;
		private readonly Action<TextWriter> _actionValue;
		private readonly Text _textValue;

		private Text()
		{
			_isNone = true;
		}

		//
		// Scalar
		//

		private Text(char value)
		{
			_charValue = value;
		}

		private Text(string value)
		{
			_stringValue = value;
		}

		private Text(IWritable value)
		{
			_writableValue = value;
		}

		private Text(Action<TextWriter> value)
		{
			_actionValue = value;
		}

		private Text(Text value)
		{
			_textValue = value;
		}

		//
		// Concatenated
		//

		private Text(Text left, char right)
		{
			_before = left;
			_charValue = right;
		}

		private Text(Text left, string right)
		{
			_before = left;
			_stringValue = right;
		}

		private Text(Text left, IWritable right)
		{
			_before = left;
			_writableValue = right;
		}

		private Text(Text left, Action<TextWriter> right)
		{
			_before = left;
			_actionValue = right;
		}

		private Text(Text left, Text right)
		{
			_before = left;
			_textValue = right;
		}

		public override string ToString() => ToBuilder().ToString();
		public Text ToText() => this;

		//
		// ToBuilder
		//

		public StringBuilder ToBuilder()
		{
			var builder = new StringBuilder();

			ToBuilder(builder);

			return builder;
		}

		public void ToBuilder(StringBuilder builder)
		{
			if(_isNone)
			{
				return;
			}

			if(_before != null)
			{
				_before.ToBuilder(builder);
			}

			if(_actionValue != null)
			{
				using(var writer = new StringWriter())
				{
					_actionValue(writer);

					builder.Append(writer);
				}
			}
			else if(_stringValue != null)
			{
				builder.Append(_stringValue);
			}
			else if(_textValue != null)
			{
				_textValue.ToBuilder(builder);
			}
			else if(_writableValue != null)
			{
				builder.Append(_writableValue);
			}
			else
			{
				if(_charValue != null)
				{
					builder.Append(_charValue.Value);
				}
			}
		}

		//
		// ToWriter
		//

		public Writer ToWriter()
		{
			var writer = new Writer();

			ToWriter(writer);

			return writer;
		}

		public void ToWriter(TextWriter writer)
		{
			if(_isNone)
			{
				return;
			}

			if(_before != null)
			{
				_before.ToWriter(writer);
			}

			if(_actionValue != null)
			{
				_actionValue(writer);
			}
			else if(_stringValue != null)
			{
				writer.Write(_stringValue);
			}
			else if(_textValue != null)
			{
				_textValue.ToWriter(writer);
			}
			else if(_writableValue != null)
			{
				writer.Write(_writableValue);
			}
			else
			{
				if(_charValue != null)
				{
					writer.Write(_charValue.Value);
				}
			}
		}

		//
		// ToStream
		//

		public void ToStream(Stream stream, Encoding encoding, bool seekBegin = false)
		{
			if(_isNone)
			{
				return;
			}

			var writer = new StreamWriter(stream, encoding);

			ToWriter(writer);

			writer.Flush();

			if(seekBegin)
			{
				stream.Seek(0, SeekOrigin.Begin);
			}
		}

		public void ToStream(Stream stream, bool seekBegin = false)
		{
			ToStream(stream, Encoding.Default, seekBegin);
		}

		public Stream ToStream(Encoding encoding, bool seekBegin = false)
		{
			var stream = new MemoryStream();

			ToStream(stream, encoding, seekBegin);

			return stream;
		}

		public Stream ToStream(bool seekBegin = false)
		{
			return ToStream(Encoding.Default, seekBegin);
		}

		//
		// Conversions
		//

		public static implicit operator string(Text text)
		{
			return text == null ? "" : text.ToString();
		}

		public static implicit operator Text(char value)
		{
			return new Text(value.ToString());
		}

		public static implicit operator Text(string value)
		{
			return new Text(value);
		}

		public static implicit operator Text(Action<TextWriter> value)
		{
			return new Text(value);
		}

		//
		// Writes
		//

		public Text Write(char value)
		{
			return _isNone ? new Text(value) : new Text(this, value);
		}

		public Text Write(string value)
		{
			return _isNone ? new Text(value) : new Text(this, value);
		}

		public Text Write(IWritable value)
		{
			return _isNone ? new Text(value) : new Text(this, value);
		}

		public Text Write(Action<TextWriter> value)
		{
			return _isNone ? new Text(value) : new Text(this, value);
		}

		public Text Write(Func<Text> value)
		{
			Action<TextWriter> actionValue = writer => value().ToWriter(writer);

			return _isNone ? new Text(actionValue) : new Text(this, actionValue);
		}

		public Text Write(Text value)
		{
			return _isNone ? value : new Text(this, value);
		}

		public static Text operator +(Text text, char value)
		{
			return text.Write(value);
		}

		public static Text operator +(Text text, string value)
		{
			return text.Write(value);
		}

		public static Text operator +(Text text, IWritable value)
		{
			return text.Write(value);
		}

		public static Text operator +(Text text, Action<TextWriter> value)
		{
			return text.Write(value);
		}

		public static Text operator +(Text text, Text value)
		{
			return text.Write(value);
		}

		//
		// Values
		//

		public static readonly Text None = new Text();
		public static readonly Text Line = Environment.NewLine;
		public static readonly Text TwoLines = Line.Repeat(2);

		public const string TabIndent = "\t";
		public const string TwoSpaceIndent = "  ";
		public const string FourSpaceIndent = "    ";
		public const string Ellipsis = "…";

		//
		// Of
		//

		public static Text Of(char value)
		{
			return new Text(value);
		}

		public static Text Of(string value)
		{
			return new Text(value);
		}

		public static Text Of(IWritable value)
		{
			return new Text(value);
		}

		public static Text Of(Action<TextWriter> value)
		{
			return new Text(value);
		}

		public static Text Of(Func<Text> value)
		{
			return new Text(writer => value().ToWriter(writer));
		}

		public static Text Of(bool value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(char[] value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(char[] value, int index, int count)
		{
			return new Text(writer => writer.Write(value, index, count));
		}

		public static Text Of(decimal value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(double value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(float value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(int value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(long value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(object value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(uint value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(ulong value)
		{
			return new Text(writer => writer.Write(value));
		}

		public static Text Of(string format, object arg0)
		{
			return new Text(writer => writer.Write(format, arg0));
		}

		public static Text Of(string format, object arg0, object arg1)
		{
			return new Text(writer => writer.Write(format, arg0, arg1));
		}

		public static Text Of(string format, object arg0, object arg1, object arg2)
		{
			return new Text(writer => writer.Write(format, arg0, arg1, arg2));
		}

		public static Text Of(string format, params object[] args)
		{
			return new Text(writer => writer.Write(format, args));
		}

		//
		// OfMany
		//

		public static Text OfMany(IEnumerable<char> values)
		{
			return Text.Of(new string(values.ToArray()));
		}

		public static Text OfMany(IEnumerable<string> values)
		{
			return Text.OfMany(values, value => Text.Of(value));
		}

		public static Text OfMany(IEnumerable<IWritable> values)
		{
			return Text.OfMany(values, Text.Of);
		}

		public static Text OfMany(IEnumerable<Action<TextWriter>> values)
		{
			return Text.OfMany(values, Text.Of);
		}

		public static Text OfMany(IEnumerable<Text> values)
		{
			return values.Aggregate(Text.None, (text, value) => text + value);
		}

		public static Text OfMany<T>(IEnumerable<T> values, Func<T, Text> selectText)
		{
			return Text.OfMany(values.Select(selectText));
		}

		public static Text OfMany<T>(IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return Text.OfMany(values.Select(selectText));
		}

		public static Text OfMany<T>(IEnumerable<T> values)
		{
			return Text.OfMany(values, value => Text.Of(value));
		}

		//
		// OfType
		//

		public static Text OfType(object instance)
		{
			return instance == null ? Text.None : Text.Of(instance.GetType());
		}

		public static Text OfType<T>()
		{
			return Text.Of(typeof(T));
		}

		//
		// If
		//

		public static Text If(bool condition, Text whenTrue, Text whenFalse)
		{
			return condition ? whenTrue : whenFalse;
		}

		public static Text If(bool condition, Text whenTrue)
		{
			return If(condition, whenTrue, Text.None);
		}

		public static Text If(Func<bool> condition, Text whenTrue, Text whenFalse)
		{
			return Of(() => condition() ? whenTrue : whenFalse);
		}

		public static Text If(Func<bool> condition, Text whenTrue)
		{
			return If(condition, whenTrue, Text.None);
		}

    //
    // Counts
    //

		public static Text Pluralized(int count, Text singular, Text plural = null)
		{
			return If(count == 1, singular, plural ?? singular + "s");
		}

    public static Text Pluralized(long count, Text singular, Text plural = null)
    {
      return If(count == 1, singular, plural ?? singular + "s");
    }

    public static Text Count(int count, Text singular, Text plural = null)
		{
			return Of(count).Write(" ").WritePluralized(count, singular, plural);
		}

    public static Text Count(long count, Text singular, Text plural = null)
    {
      return Of(count).Write(" ").WritePluralized(count, singular, plural);
    }

    //
    // SeparatedBy
    //

    public static Text SeparatedBy(Text separator, IEnumerable<Text> values, Func<Text, Text> selectText)
		{
			return SeparatedBy(separator, values.Select(selectText));
		}

		public static Text SeparatedBy(Text separator, IEnumerable<Text> values, Func<Text, int, Text> selectText)
		{
			return SeparatedBy(separator, values.Select(selectText));
		}

		public static Text SeparatedBy(Text separator, IEnumerable<Text> values)
		{
			var text = Text.None;

			var wroteFirst = false;

			foreach(var value in values)
			{
				if(wroteFirst)
				{
					text += separator;
				}
				else
				{
					wroteFirst = true;
				}

				text += value;
			}

			return text;
		}

		public static Text SeparatedBy<T>(Text separator, IEnumerable<T> values, Func<T, Text> selectText)
		{
			return Text.SeparatedBy(separator, values.Select(selectText));
		}

		public static Text SeparatedBy<T>(Text separator, IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return Text.SeparatedBy(separator, values.Select(selectText));
		}

		public static Text SeparatedBy<T>(Text separator, IEnumerable<T> values)
		{
			return Text.SeparatedBy(separator, values.Select(value => Text.Of(value)));
		}

		//
		// EditDistance
		//

		public static int EditDistance(Text x, Text y, int substitutionCost = 1)
		{
			return CalculateEditDistance(x, y, substitutionCost);
		}

		/// <summary>
		/// Converts instances of <see cref="Text"/> to/from <see cref="System.String"/>
		/// </summary>
		public sealed class Converter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return true;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return Text.Of(value);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				return value == null ? "" : value.ToString();
			}
		}

		/// <summary>
		/// Builds an instance of <see cref="Text"/> through writes to <see cref="TextWriter"/>
		/// </summary>
		public class Writer : TextWriter, IWritable
		{
			private Text _text;
			private readonly Encoding _encoding;

			public Writer()
			{
				_text = Text.None;
				_encoding = Encoding.Default;
			}

			public Writer(Encoding encoding)
			{
				_text = Text.None;
				_encoding = encoding;
			}

			public Writer(Text text)
			{
				_text = text;
				_encoding = Encoding.Default;
			}

			public Writer(Text text, Encoding encoding)
			{
				_text = text;
				_encoding = encoding;
			}

			public Writer(Text text, Encoding encoding, IFormatProvider formatProvider) : base(formatProvider)
			{
				_text = text;
				_encoding = encoding;
			}

			public override Encoding Encoding
			{
				get { return _encoding; }
			}

			public sealed override string ToString()
			{
				return ToText();
			}

			public virtual Text ToText()
			{
				return _text;
			}

			public override void Write(bool value) { _text = _text.Write(value); }
			public override void Write(char value) { _text = _text.Write(value); }
			public override void Write(char[] buffer) { _text = _text.Write(buffer); }
			public override void Write(decimal value) { _text = _text.Write(value); }
			public override void Write(double value) { _text = _text.Write(value); }
			public override void Write(float value) { _text = _text.Write(value); }
			public override void Write(int value) { _text = _text.Write(value); }
			public override void Write(long value) { _text = _text.Write(value); }
			public override void Write(object value) { _text = _text.Write(value); }
			public override void Write(string value) { _text = _text.Write(value); }
			public override void Write(uint value) { _text = _text.Write(value); }
			public override void Write(ulong value) { _text = _text.Write(value); }
			public override void Write(string format, object arg0) { _text = _text.Write(format, arg0); }
			public override void Write(string format, params object[] arg) { _text = _text.Write(format, arg); }
			public override void Write(char[] buffer, int index, int count) { _text = _text.Write(buffer, index, count); }
			public override void Write(string format, object arg0, object arg1) { _text = _text.Write(format, arg0, arg1); }
			public override void Write(string format, object arg0, object arg1, object arg2) { _text = _text.Write(format, arg0, arg1, arg2); }

			public override void WriteLine() { _text = _text.WriteLine(); }
			public override void WriteLine(bool value) { _text = _text.WriteLine(value); }
			public override void WriteLine(char value) { _text = _text.WriteLine(value); }
			public override void WriteLine(char[] buffer) { _text = _text.WriteLine(buffer); }
			public override void WriteLine(decimal value) { _text = _text.WriteLine(value); }
			public override void WriteLine(double value) { _text = _text.WriteLine(value); }
			public override void WriteLine(float value) { _text = _text.WriteLine(value); }
			public override void WriteLine(int value) { _text = _text.WriteLine(value); }
			public override void WriteLine(long value) { _text = _text.WriteLine(value); }
			public override void WriteLine(object value) { _text = _text.WriteLine(value); }
			public override void WriteLine(string value) { _text = _text.WriteLine(value); }
			public override void WriteLine(uint value) { _text = _text.WriteLine(value); }
			public override void WriteLine(ulong value) { _text = _text.WriteLine(value); }
			public override void WriteLine(string format, object arg0) { _text = _text.WriteLine(format, arg0); }
			public override void WriteLine(string format, params object[] arg) { _text = _text.WriteLine(format, arg); }
			public override void WriteLine(char[] buffer, int index, int count) { _text = _text.WriteLine(buffer, index, count); }
			public override void WriteLine(string format, object arg0, object arg1) { _text = _text.WriteLine(format, arg0, arg1); }
			public override void WriteLine(string format, object arg0, object arg1, object arg2) { _text = _text.WriteLine(format, arg0, arg1, arg2); }
		}

		//
		// Edit distance (Levenshtein distance)
		//

		// https://github.com/NoelKennedy/MinimumEditDistance/blob/master/MinimumEditDistance/Levenshtein.cs

		private static int CalculateEditDistance(string x, string y, int substitutionCost = 1)
		{
			if(String.IsNullOrEmpty(x) || String.IsNullOrEmpty(y))
			{
				// Short-circuit

				return CalculateEditDistance(x, y, substitutionCost, new RectangularArray(0, 0));
			}
			else if(x.Length * y.Length > _maxStringProductLength)
			{
				// Large strings, use large data structure

				return CalculateEditDistance(x, y, substitutionCost, new JaggedArray(x.Length + 1, y.Length + 1));
			}
			else
			{
				// Small strings, can use rectangular array

				return CalculateEditDistance(x, y, substitutionCost, new RectangularArray(x.Length + 1, y.Length + 1));
			}
		}

		private static int CalculateEditDistance(string x, string y, int substitutionCost, MemoryStructure memory)
		{
			if(String.IsNullOrEmpty(x))
			{
				return String.IsNullOrEmpty(y) ? 0 : y.Length;
			}

			var m = x.Length + 1;
			var n = y.Length + 1;

			// Map empties to each other

			for(int i = 0; i < m; i++)
			{
				memory[i, 0] = i;
			}

			for(int i = 0; i < n; i++)
			{
				memory[0, i] = i;
			}

			for(int i = 1; i < m; i++)
			{
				for(int j = 1; j < n; j++)
				{
					if(x[i - 1] == y[j - 1])
					{
						// No cost, letters are the same

						memory[i, j] = memory[i - 1, j - 1];
					}
					else
					{
						var delete = memory[i - 1, j] + 1;
						var insert = memory[i, j - 1] + 1;
						var substitution = memory[i - 1, j - 1] + substitutionCost;

						memory[i, j] = Math.Min(delete, Math.Min(insert, substitution));
					}
				}
			}

			return memory[m - 1, n - 1];
		}

		private const int _maxStringProductLength = 536848900;

		private abstract class MemoryStructure
		{
			internal abstract int this[int i, int j] { get; set; }
		}

		private sealed class RectangularArray : MemoryStructure
		{
			private readonly int[,] _value;

			internal RectangularArray(int m, int n)
			{
				_value = new int[m, n];
			}

			internal override int this[int i, int j]
			{
				get { return _value[i, j]; }
				set { _value[i, j] = value; }
			}
		}

		private sealed class JaggedArray : MemoryStructure
		{
			private int[][] _value;

			internal JaggedArray(int m, int n)
			{
				_value = new int[m][];

				for(int i = 0; i < m; i++)
				{
					_value[i] = new int[n];
				}
			}

			internal override int this[int i, int j]
			{
				get { return _value[i][j]; }
				set { _value[i][j] = value; }
			}
		}
	}
}