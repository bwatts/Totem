using System;
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
    public static bool TryGetMember(this LambdaExpression lambda, out MemberInfo member)
    {
      member = (lambda.Body as MemberExpression)?.Member;

      return member != null;
    }

    public static bool TryGetField(this LambdaExpression lambda, out FieldInfo field)
    {
      field = lambda.TryGetMember(out var member) ? member as FieldInfo : null;

      return field != null;
    }

    public static bool TryGetProperty(this LambdaExpression lambda, out PropertyInfo property)
    {
      property = lambda.TryGetMember(out var member) ? member as PropertyInfo : null;

      return property != null;
    }

    public static bool TryGetMethod(this LambdaExpression lambda, out MethodInfo method)
    {
      method = (lambda.Body as MethodCallExpression)?.Method;

      return method != null;
    }

    public static MemberInfo GetMember(this LambdaExpression lambda)
    {
      if(!lambda.TryGetMember(out var member))
      {
        throw new Exception($"Failed to extract field or property access from lambda: {lambda}");
      }

      return member;
    }

    public static FieldInfo GetField(this LambdaExpression lambda)
    {
      if(!lambda.TryGetField(out var field))
      {
        throw new Exception($"Failed to extract field access from lambda: {lambda}");
      }

      return field;
    }

    public static PropertyInfo GetProperty(this LambdaExpression lambda)
    {
      if(!lambda.TryGetProperty(out var property))
      {
        throw new Exception($"Failed to extract property access from lambda: {lambda}");
      }

      return property;
    }

    public static MethodInfo GetMethod(this LambdaExpression lambda)
    {
      if(!lambda.TryGetMethod(out var method))
      {
        throw new Exception($"Failed to extract property access from lambda: {lambda}");
      }

      return method;
    }
  }
}