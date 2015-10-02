using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Totem.Reflection
{
	/// <summary>
	/// Extends <see cref="System.Type"/> with text as it would appear in source
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TypeSourceText
	{
		public static string ToSourceText(this Type type)
		{
			Text unqualifiedType;

			return type.TryReadUnqualifiedType(out unqualifiedType)
				? unqualifiedType
				: type.ReadQualifiedType();
		}

		private static bool TryReadUnqualifiedType(this Type type, out Text unqualifiedType)
		{
			unqualifiedType = "";

			if(type.IsEnum)
			{
				return false;
			}

			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				unqualifiedType = type.GetGenericArguments().Single().ToSourceText() + "?";

				return true;
			}

			var primitiveType = type.ReadPrimitiveType();

			var isPrimitiveType = primitiveType != "";

			if(isPrimitiveType)
			{
				unqualifiedType = primitiveType;
			}

			return isPrimitiveType;
		}

		private static string ReadPrimitiveType(this Type type)
		{
			switch(Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return "bool";
				case TypeCode.Char:
					return "char";
				case TypeCode.Byte:
					return "byte";
				case TypeCode.Int16:
					return "short";
				case TypeCode.Int32:
					return "int";
				case TypeCode.Int64:
					return "long";
				case TypeCode.Single:
					return "float";
				case TypeCode.Double:
					return "double";
				case TypeCode.Decimal:
					return "decimal";
				case TypeCode.DateTime:
					return "DateTime";
				case TypeCode.String:
					return "string";
				default:
					return "";
			}
		}

		private static Text ReadQualifiedType(this Type type)
		{
			var name = type.Name;

			if(type.IsNested)
			{
				name = Text.Of("{0}.{1}", type.DeclaringType.ToSourceText(), name);
			}

			var backtickIndex = name.IndexOf('`');

			if(backtickIndex == -1)
			{
				return name;
			}
			else
			{
				return Text.Of(
					"{0}<{1}>",
					name.Substring(0, backtickIndex),
					type.GetGenericArguments().ToTextSeparatedBy(", ", arg => arg.ToSourceText()));
			}
		}
	}
}