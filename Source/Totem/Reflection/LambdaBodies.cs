using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Reflection
{
	/// <summary>
	/// Extends lambda expressions with the ability to find members and methods referenced within
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class LambdaBodies
	{
		public static MemberInfo GetMemberInfo(this LambdaExpression lambda, bool strict = true)
		{
			var memberExpression = lambda.Body as MemberExpression;

			var member = memberExpression == null ? null : memberExpression.Member;

			Expect.That(strict && member == null).IsFalse(
				issue: Text.Of("Lambda does not access a field or property"),
				expected: "Lambda body of type " + Text.OfType<MemberExpression>(),
				actual: t => lambda.ToText());

			return member;
		}

		public static FieldInfo GetFieldInfo(this LambdaExpression lambda, bool strict = true)
		{
			var field = lambda.GetMemberInfo(strict: false) as FieldInfo;

			Expect.That(strict && field == null).IsFalse(
				issue: Text.Of("Lambda does not access a field"),
				expected: "Lambda body accessing a " + Text.OfType<FieldInfo>(),
				actual: t => lambda.ToText());

			return field;
		}

		public static PropertyInfo GetPropertyInfo(this LambdaExpression lambda, bool strict = true)
		{
			var property = lambda.GetMemberInfo(strict: false) as PropertyInfo;

			Expect.That(strict && property == null).IsFalse(
				issue: Text.Of("Lambda does not access a property"),
				expected: "Lambda body accessing a " + Text.OfType<PropertyInfo>(),
				actual: t => lambda.ToText());

			return property;
		}

		public static MethodInfo GetMethodInfo(this LambdaExpression lambda, bool strict = true)
		{
			var callExpression = lambda.Body as MethodCallExpression;

			var method = callExpression == null ? null : callExpression.Method;

			Expect.That(strict && method == null).IsFalse(
				issue: Text.Of("Lambda does not call a method"),
				expected: "Lambda body of type " + Text.OfType<MethodCallExpression>(),
				actual: t => lambda.ToText());

			return method;
		}

		private static Text ToText(this LambdaExpression lambda)
		{
			return Text.Of(lambda).Write(" ").WriteInParentheses(Text.Of(lambda.Body.NodeType));
		}

		//
		// Names
		//

		public static string GetMemberName(this LambdaExpression lambda, bool strict = true)
		{
			var member = lambda.GetMemberInfo(strict);

			return member == null ? null : member.Name;
		}

		public static string GetFieldName(this LambdaExpression lambda, bool strict = true)
		{
			var field = lambda.GetFieldInfo(strict);

			return field == null ? null : field.Name;
		}

		public static string GetPropertyName(this LambdaExpression lambda, bool strict = true)
		{
			var property = lambda.GetPropertyInfo(strict);

			return property == null ? null : property.Name;
		}

		public static string GetMethodName(this LambdaExpression lambda, bool strict = true)
		{
			var method = lambda.GetMethodInfo(strict);

			return method == null ? null : method.Name;
		}
	}
}