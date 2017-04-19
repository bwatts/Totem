using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing an instance that persists between usages.
	/// 
	/// In practice so far, this means decorated with [Durable].
	/// </summary>
	public class DurableType : RuntimeType
	{
		internal DurableType(RuntimeTypeRef type) : base(type)
		{}

		public object CreateToDeserialize()
		{
			var instance = FormatterServices.GetUninitializedObject(DeclaredType);

      var binding = instance as Binding;

      if(binding != null)
      {
        binding.Fields = new Fields(binding);
      }

      return instance;
		}
	}
}