using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reflection;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A named value (field or property) read and/or written in a runtime
	/// </summary>
	public class RuntimeStatePart : Notion
	{
		public RuntimeStatePart(MemberInfo member)
		{
			Member = member;
			Name = member.Name;

			AsField = member as FieldInfo;
			AsProperty = member as PropertyInfo;

			IsField = AsField != null;
			IsProperty = AsProperty != null;

			if(IsField)
			{
				ValueType = AsField.FieldType;

				var canWrite = !AsField.IsInitOnly;

				IsPublic = AsField.IsPublic;
				IsPublicSet = IsPublic && canWrite;

				IsInternal = AsField.IsFamily;
				IsInternalSet = IsInternal && canWrite;

				IsProtected = AsField.IsFamily;
				IsProtectedSet = IsProtected && canWrite;

				IsPrivate = AsField.IsPrivate;
				IsPrivateSet = IsPrivate && canWrite;
			}
			else
			{
				ValueType = AsProperty.PropertyType;
				
				var getter = AsProperty.GetGetMethod(nonPublic: true);
				var setter = AsProperty.GetSetMethod(nonPublic: true);

				IsPublic = getter != null && getter.IsPublic;
				IsPublicSet = setter != null && setter.IsPublic;

				IsInternal = getter != null && getter.IsAssembly;
				IsInternalSet = setter != null && setter.IsAssembly;

				IsProtected = getter != null && getter.IsFamily;
				IsProtectedSet = setter != null && setter.IsFamily;

				IsPrivate = getter != null && getter.IsPrivate;
				IsPrivateSet = setter != null && setter.IsPrivate;
			}
		}

		public readonly MemberInfo Member;
		public readonly string Name;
		public readonly Type ValueType;
		public readonly bool IsField;
		public readonly bool IsProperty;
		public readonly FieldInfo AsField;
		public readonly PropertyInfo AsProperty;
		public readonly bool IsPublic;
		public readonly bool IsPublicSet;
		public readonly bool IsInternal;
		public readonly bool IsInternalSet;
		public readonly bool IsProtected;
		public readonly bool IsProtectedSet;
		public readonly bool IsPrivate;
		public readonly bool IsPrivateSet;

		public override Text ToText()
		{
			return Text.Of("{0} {1}", ValueType.ToSourceText(), Name);
		}
	}
}