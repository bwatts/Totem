using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Reflection
{
  /// <summary>
  /// Extensions to <see cref="LambdaExpression"/>
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class LambdaExpressionExtensions
  {
    public static MemberInfo GetMemberInfo(this LambdaExpression lambda, bool strict = true)
    {
      var member = (lambda.Body as MemberExpression)?.Member;

      Expect.False(strict && member == null, "Lambda does not access a field or property");

      return member;
    }

    public static FieldInfo GetFieldInfo(this LambdaExpression lambda, bool strict = true)
    {
      var field = lambda.GetMemberInfo(strict: false) as FieldInfo;

      Expect.False(strict && field == null, "Lambda does not access a field");

      return field;
    }

    public static PropertyInfo GetPropertyInfo(this LambdaExpression lambda, bool strict = true)
    {
      var property = lambda.GetMemberInfo(strict: false) as PropertyInfo;

      Expect.False(strict && property == null, "Lambda does not access a property");

      return property;
    }

    public static MethodInfo GetMethodInfo(this LambdaExpression lambda, bool strict = true)
    {
      var method = (lambda.Body as MethodCallExpression)?.Method;

      Expect.False(strict && method == null, "Lambda does not call a method");

      return method;
    }

    static Text ToText(this LambdaExpression lambda) =>
      Text.Of(lambda).Write(" ").WriteInParentheses(Text.Of(lambda.Body.NodeType));

    //
    // Names
    //

    public static string GetMemberName(this LambdaExpression lambda, bool strict = true) =>
      lambda.GetMemberInfo(strict)?.Name;

    public static string GetFieldName(this LambdaExpression lambda, bool strict = true) =>
      lambda.GetFieldInfo(strict)?.Name;

    public static string GetPropertyName(this LambdaExpression lambda, bool strict = true) =>
      lambda.GetPropertyInfo(strict)?.Name;

    public static string GetMethodName(this LambdaExpression lambda, bool strict = true) =>
      lambda.GetMethodInfo(strict)?.Name;
  }
}