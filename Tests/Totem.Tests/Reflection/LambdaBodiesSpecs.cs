using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Reflection
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Reflection.LambdaBodies"/> class
	/// </summary>
	public class LambdaBodiesSpecs : Specs
  {
		public static readonly object Field;

		public static object Property { get; private set; }

		public static object Method()
		{
			return null;
		}

		FieldInfo _fieldInfo = typeof(LambdaBodiesSpecs).GetField("Field");
		PropertyInfo _propertyInfo = typeof(LambdaBodiesSpecs).GetProperty("Property");
		MethodInfo _methodInfo = typeof(LambdaBodiesSpecs).GetMethod("Method");

		Expression<Func<object>> _getField = () => Field;
		Expression<Func<object>> _getProperty = () => Property;
		Expression<Func<object>> _callMethod = () => Method();

		//
		// Member info
		//

		void GetFieldMemberInfoForField()
		{
			Expect(_getField.GetMemberInfo()).Is(_fieldInfo);
		}

		void GetMemberInfoForProperty()
		{
			Expect(_getProperty.GetMemberInfo()).Is(_propertyInfo);
		}

		void GetMemberInfoForMethod()
		{
			ExpectThrows<ExpectException>(() => _callMethod.GetMemberInfo());
		}

		void GetMemberInfoForMethodNonStrict()
		{
			Expect(_callMethod.GetMemberInfo(strict: false)).IsNull();
		}

		//
		// Field info
		//

		void GetFieldInfo()
		{
			Expect(_getField.GetFieldInfo()).Is(_fieldInfo);
		}

		void GetFieldInfoForProperty()
		{
			ExpectThrows<ExpectException>(() => _getProperty.GetFieldInfo());
		}

		void GetFieldInfoForPropertyNonStrict()
		{
			Expect(_getProperty.GetFieldInfo(strict: false)).IsNull();
		}

		//
		// Property info
		//

		void GetPropertyInfo()
		{
			Expect(_getProperty.GetPropertyInfo()).Is(_propertyInfo);
		}

		void GetPropertyInfoForField()
		{
			ExpectThrows<ExpectException>(() => _getField.GetPropertyInfo());
		}

		void GetPropertyInfoForFieldNonStrict()
		{
			Expect(_getField.GetPropertyInfo(strict: false)).IsNull();
		}

		//
		// Method info
		//

		void GetMethodInfo()
		{
			Expect(_callMethod.GetMethodInfo()).Is(_methodInfo);
		}

		void GetMethodInfoForField()
		{
			ExpectThrows<ExpectException>(() => _getField.GetMethodInfo());
		}

		void GetMethodInfoForFieldNonStrict()
		{
			Expect(_getField.GetMethodInfo(strict: false)).IsNull();
		}

		//
		// Member names
		//

		void GetMemberNameForField()
		{
			Expect(_getField.GetMemberName()).Is(_fieldInfo.Name);
		}

		void GetMemberNameForProperty()
		{
			Expect(_getProperty.GetMemberName()).Is(_propertyInfo.Name);
		}

		void GetMemberNameForMethod()
		{
			ExpectThrows<ExpectException>(() => _callMethod.GetMemberName());
		}

		void GetMemberNameForMethodNonStrict()
		{
			Expect(_callMethod.GetMemberName(strict: false)).IsNull();
		}

		//
		// Field names
		//

		void GetFieldName()
		{
			Expect(_getField.GetFieldName()).Is(_fieldInfo.Name);
		}

		void GetFieldNameForProperty()
		{
			ExpectThrows<ExpectException>(() => _getProperty.GetFieldName());
		}

		void GetFieldNameForPropertyNonStrict()
		{
			Expect(_getProperty.GetFieldName(strict: false)).IsNull();
		}

		//
		// Property names
		//

		void GetPropertyName()
		{
			Expect(_getProperty.GetPropertyName()).Is(_propertyInfo.Name);
		}

		void GetPropertyNameForField()
		{
			ExpectThrows<ExpectException>(() => _getField.GetPropertyName());
		}

		void GetPropertyNameForFieldNonStrict()
		{
			Expect(_getField.GetPropertyName(strict: false)).IsNull();
		}

		//
		// Method names
		//

		void GetMethodName()
		{
			Expect(_callMethod.GetMethodName()).Is(_methodInfo.Name);
		}

		void GetMethodNameForProperty()
		{
			ExpectThrows<ExpectException>(() => _getProperty.GetMethodName());
		}

		void GetMethodNameForPropertyNonStrict()
		{
			Expect(_getProperty.GetMethodName(strict: false)).IsNull();
		}
	}
}