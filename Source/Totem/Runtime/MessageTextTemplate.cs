using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reflection;

namespace Totem.Runtime
{
	/// <summary>
	/// Expands nested message types for text templates (.tt)
	/// </summary>
	public static class MessageTextTemplate
	{
		public static string Expand(Type declaringType)
		{
			var expansion = new Expansion(declaringType);

			var messages = expansion.ExpandMessages();

			return expansion
				.WriteUsingStatements()
				.WriteTwoLines()
				.WriteLine("namespace {0}", declaringType.Namespace)
				.WriteLine("{")
				.WriteIndented(messages, indent: Text.TabIndent, retainIndent: true, retainWhitespace: true)
				.WriteLine()
				.Write("}");
		}

		private static Text ExpandMessages(this Expansion expansion)
		{
			return expansion.DeclaringType
				.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(typeof(Delegate).IsAssignableFrom)
				.ToTextSeparatedBy(Text.TwoLines, type => expansion.ExpandMessage(type));
		}

		private static Text ExpandMessage(this Expansion expansion, Type delegateType)
		{
			var fields = new List<ExpansionField>();

			var invoke = delegateType.GetMethod("Invoke");

			foreach(var parameter in invoke.GetParameters())
			{
				expansion.AddNamespace(parameter.ParameterType, recurse: true);

				fields.Add(new ExpansionField(parameter));
			}

			return delegateType.ExpandComment(fields) + delegateType.ExpandClass(fields);
		}

		private static Text ExpandComment(this Type type, List<ExpansionField> fields)
		{
			return Text.None
				.WriteLine("/// <summary>")
				.WriteLine("/// {0}", fields.ToTextSeparatedBy(", ", field => field.Name))
				.WriteLine("/// </summary>");
		}

		private static Text ExpandClass(this Type type, List<ExpansionField> fields)
		{
			return Text.None
				.WriteLine("public class {0} : Event", type.Name)
				.WriteScoped(type.ExpandConstructor(fields), indent: Text.TabIndent);
		}

		private static Text ExpandConstructor(this Type type, List<ExpansionField> fields)
		{
			return Text
				.Of("public ")
				.Write(type.Name)
				.WriteInParentheses(fields.ExpandParameters())
				.WriteLine()
				.WriteScoped(fields.ExpandAssignments(), indent: Text.TabIndent)
				.WriteTwoLines()
				.Write(fields.ExpandDeclarations());
		}

		private static Text ExpandParameters(this List<ExpansionField> fields)
		{
			return fields.ToTextSeparatedBy(", ", field => field.ToParameterText());
		}

		private static Text ExpandAssignments(this List<ExpansionField> fields)
		{
			return fields.ToTextSeparatedBy(Text.Line, field => field.ToAssignmentText());
		}

		private static Text ExpandDeclarations(this List<ExpansionField> fields)
		{
			return fields.ToTextSeparatedBy(Text.Line, field => field.ToDeclarationText());
		}

		private static IEnumerable<Type> RecurseGenericTypes(this IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				yield return type;
				var generics = type.GenericTypeArguments;
				foreach (var generic in generics.RecurseGenericTypes())
				{
					yield return generic;
				}
			}
		}

		//
		// Expansion
		//

		private sealed class Expansion : Notion
		{
			private readonly SortedSet<ExpansionNamespace> _using = new SortedSet<ExpansionNamespace>();

			internal Expansion(Type declaringType)
			{
				DeclaringType = declaringType;

				AddNamespace(typeof(Event));
			}

			internal Type DeclaringType { get; private set; }

			internal void AddNamespace(Type type, bool recurse = false)
			{
				if(type.Namespace != DeclaringType.Namespace)
				{
					_using.Add(new ExpansionNamespace(type.Namespace));
				}

				if(!recurse)
				{
					return;
				}

				var genericTypes = (from genericType in type.GenericTypeArguments.RecurseGenericTypes()
														where genericType.Namespace != DeclaringType.Namespace
														select genericType).Distinct();

				foreach (var genericType in genericTypes)
				{
					_using.Add(new ExpansionNamespace(genericType.Namespace));
				}
			}

			internal Text WriteUsingStatements()
			{
				return _using.ToTextSeparatedBy(Text.Line);
			}
		}

		private sealed class ExpansionNamespace : IEquatable<ExpansionNamespace>, IComparable<ExpansionNamespace>
		{
			private readonly string _name;
			private readonly bool _nonSystem;

			internal ExpansionNamespace(string name)
			{
				_name = name;
				_nonSystem = !(name == "System" || name.StartsWith("System."));
			}

			public override string ToString()
			{
				return Text.Of("using {0};", _name);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as ExpansionNamespace);
			}

			public bool Equals(ExpansionNamespace other)
			{
				return other != null && _name == other._name;
			}

			public override int GetHashCode()
			{
				return _name.GetHashCode();
			}

			public int CompareTo(ExpansionNamespace other)
			{
				// System namespaces sort first (less than)
				// Non-system namespaces sort second (greater than)

				var systemResult = _nonSystem.CompareTo(other._nonSystem);

				return systemResult != 0 ? systemResult : _name.CompareTo(other._name);
			}
		}

		//
		// Expansion field
		//

		private sealed class ExpansionField
		{
			internal ExpansionField(ParameterInfo parameter)
			{
				Name = parameter.Name.ReadFieldName();
				Variable = parameter.Name;
				Type = parameter.ParameterType.ToSourceText();
			}

			internal string Name { get; private set; }
			internal string Variable { get; private set; }
			internal string Type { get; private set; }

			internal Text ToParameterText()
			{
				return Text.Of("{0} {1}", Type, Variable);
			}

			internal Text ToAssignmentText()
			{
				return Text.Of("{0} = {1};", Name, Variable);
			}

			internal Text ToDeclarationText()
			{
				return Text.Of("public readonly {0} {1};", Type, Name);
			}
		}

		private static string ReadFieldName(this string parameterName)
		{
			return Char.ToUpper(parameterName[0]) + parameterName.Substring(1);
		}
	}
}